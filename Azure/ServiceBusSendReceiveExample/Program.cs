using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ServiceBus;
using Microsoft.WindowsAzure;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using Microsoft.Azure;

namespace ServiceBusSendReceiveExample
{
    class Program
    {
        private static string queueName = "TestQueue";
        private static string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
        static void Main(string[] args)
        {
            // Service bus introduction
            // http://azure.microsoft.com/en-us/documentation/services/service-bus/
            // Creating service bus queues and sending messages
            // http://azure.microsoft.com/en-us/documentation/articles/service-bus-dotnet-how-to-use-queues/
            // How to video series starts with part 1 at
            // http://channel9.msdn.com/Blogs/Subscribe/Getting-Started-with-Service-Bus-Part-1-The-Portal
            // Service Bus Explorer
            // http://code.msdn.microsoft.com/windowsazure/Service-Bus-Explorer-f2abca5a

            CreateQueue();
            //CreateQueueCustom(5120,1);
            SendMessage();
            ReceiveMessage();

            // Advanced stuff
            //CreateTopicAndSubscriptionAndSendAndReceive();

            // You can also do a BrokeredMessage.Defer() if you get a message which you aren't ready to process yet.
            // See http://channel9.msdn.com/Blogs/Subscribe/Getting-Started-with-Service-Bus-Part-4-Queues for a video of this.

            // See what messages you have in the queue without taking them out or locking them.
            // BrowseMessages();

            // Programatically stop sending and receiving to queues etc.
            // See http://channel9.msdn.com/Blogs/Subscribe/Whats-new-in-the-Service-Bus-NET-SDK-20

            // Auto-delete of unsused entities (queues, topics and subscriptions.
            // Useful when say a worker role which creates it's own subscription dies and is then respawned and creates a new subscription.
            // The old subscription will be removed automatically after X minutes of inactivity.
            // See http://channel9.msdn.com/Blogs/Subscribe/Whats-new-in-the-Service-Bus-NET-SDK-20
            // Example also in CreateQueueCustom()

            // Rather than write your own message loop to receive message, you can use the OnMessage() function
            // See http://channel9.msdn.com/Blogs/Subscribe/Whats-new-in-the-Service-Bus-NET-SDK-20
            // MessagePumpExample();

            // Send and receive from a paritioned queue. Paritioning guarentees that all messages with the same key are sent to the same "fragment" and that ordering of messages will be preserved.
            // PartitionedExample();

         }

        private static void MessagePumpExample()
        {
            QueueClient client = QueueClient.CreateFromConnectionString(connectionString, queueName, ReceiveMode.PeekLock);
           
            // This will now start a loop which runs asynchronously in the background...
            // ....picking up messages when they arrive and processing them.
            // This is also an OnMessageAsync should you need it.
            client.OnMessage((message) => {
                // This code will be called for each message in the queue.
                // You don't need to do anything in a loop yourself, e.g. in a while(true) type thing.
                Console.WriteLine("Label: " + message.Label);
                Console.WriteLine("Body: " + message.GetBody<string>());
                Console.WriteLine("MessageID: " + message.MessageId);  
            });

            Console.WriteLine("Press any key to return from MessagePumpExample()");
            Console.ReadLine();
        }

        private static void BrowseMessages()
        {
            //Send 5 messages 1 second a part
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Sending message " + i);
                SendMessage();
                Thread.Sleep(1000);
            }
            QueueClient browseClient = QueueClient.CreateFromConnectionString(connectionString, queueName);

            BrokeredMessage message = browseClient.Peek(); // gets the first message but does not lock it.
            BrokeredMessage message2 = browseClient.Peek(3); // gets message with sequencenumber 3 but does not lock it.
            IEnumerable<BrokeredMessage> messages = browseClient.PeekBatch(5); // gets the first 5 messages but does not lock them. 
            IEnumerable<BrokeredMessage> messages2 = browseClient.PeekBatch(3, 5); // gets the first 5 messages starting from sequencenumber 3 but does not lock them.
        }

        private static void CreateQueue()
        {
            // Create the queue if it does not exist already
            
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists(queueName))
            {
                Console.WriteLine("Creating queue.");
                namespaceManager.CreateQueue(queueName);
                Console.WriteLine("Queue created.");
            }
        }

        /// <summary>
        /// Create a new Service Bus queue with custom settings
        /// </summary>
        /// <param name="maxsize">Maximum message size in MB</param>
        /// <param name="minutes">Numbers of minutes before messages expire in the queue</param>
        private static void CreateQueueCustom(int maxsize, int minutes)
        {
            // Configure Queue Settings
            QueueDescription qd = new QueueDescription(queueName);
            qd.MaxSizeInMegabytes = maxsize;
            qd.DefaultMessageTimeToLive = new TimeSpan(0, minutes, 0);

            qd.AutoDeleteOnIdle = TimeSpan.FromMinutes(5);


            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!namespaceManager.QueueExists(queueName))
            {
                namespaceManager.CreateQueue(qd);
            }
        }

        private static void SendMessage()
        {
            QueueClient client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            //Client.Send(new BrokeredMessage());
            //Create message, passing a string message for the body
            BrokeredMessage message = new BrokeredMessage("This is the body");
            // Set some additional custom app-specific properties
            message.Label = "This is the label - " + DateTime.UtcNow.ToString();
            message.Properties["TestProperty"] = "TestValue";
            
            bool messageSent = false;
            // Send message to the queue

            while (!messageSent)
            {
                try
                {
                    Console.WriteLine("Trying to send message...");
                    client.Send(message); // a SendAsync() is also available.
                    messageSent = true;
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Console.WriteLine("None transient error sending message.");
                        Console.WriteLine(e.Message);
                        throw;
                    }
                    else
                    {
                        Console.WriteLine("Transient error sending message.");
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Will retry sending again in 2 seconds");
                        Thread.Sleep(2000);     
                    }
                }
            }
            Console.WriteLine(string.Format("Message sent: Id = {0}, Label = {1}, Body = {2}", message.MessageId, message.Label, message.GetBody<string>()));

            

        }

        private static void ReceiveMessage()
        {
            // You can receive messages in one of two ways:-
            // 1. ReceiveAndDelete - get the message in one shot and remove it from the queue - messages are lost if the receiver crashes.
            // 2. PeekLock - two stage process where a message is locked, processed and released once it's completed.
            // The 3rd parameter on the QueueClient sets the mode (PeekLock by default).
            QueueClient client = QueueClient.CreateFromConnectionString(connectionString, queueName, ReceiveMode.PeekLock);

            // Continuously process messages sent to the queue 
            while (true)
            {
                Console.WriteLine("Waiting...");
                BrokeredMessage message = client.Receive();

                if (message != null)
                {
                    try
                    {
                        Console.WriteLine("Body: " + message.GetBody<string>());
                        Console.WriteLine("MessageID: " + message.MessageId);
                        Console.WriteLine("Test Property: " + message.Properties["TestProperty"]);

                        // Remove message from queue
                        message.Complete();
                        break;
                    }
                    catch (Exception)
                    {
                        // Indicate a problem, unlock message in queue
                        message.Abandon();
                    }
                }
            }
        }

        private static void CreateTopicAndSubscriptionAndSendAndReceive()
        {
            const string topic = "TestTopic";
            const string subscription = "MySubscription";
            var nsm = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!nsm.TopicExists(topic))
            {
                // This sets the settings you want the topic to have
                TopicDescription td = new TopicDescription("TestTopic");
                td.RequiresDuplicateDetection = true;
                // tdActual is what was actaully created, i.e. it contains all the settings added by the server including read-only values.
                TopicDescription tdActual = nsm.CreateTopic(td);
                Console.WriteLine("Topic created.");
            }

            TopicDescription topicDesc = nsm.GetTopic(topic);

            if (!nsm.SubscriptionExists(topicDesc.Path, subscription))
            {
                // Create a subscription to receive the data back
                nsm.CreateSubscription(topicDesc.Path, subscription);
                Console.WriteLine("Subscription created.");
            }

            // Send and receive a message
            TopicClient topicSend = TopicClient.CreateFromConnectionString(connectionString, "TestTopic");
            BrokeredMessage messageSend = new BrokeredMessage("This is the body");
            // Set some addtional custom app-specific properties
            topicSend.Send(messageSend);
            Console.WriteLine("Sent message.");

            SubscriptionClient clientReceive = SubscriptionClient.CreateFromConnectionString(connectionString, topicDesc.Path, "MySubscription", ReceiveMode.ReceiveAndDelete);
            BrokeredMessage messageReceive = clientReceive.Receive();
            Console.WriteLine("Received message");

            if (messageReceive != null)
            {
                Console.WriteLine("Body: " + messageReceive.GetBody<string>());
                Console.WriteLine("MessageID: " + messageReceive.MessageId);
            }

        }

        private static void PartitionedExample()
        {
            // Create a new queue
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists("TestQueue2"))
            {
                Console.WriteLine("Creating queue.");
                QueueDescription qd = new QueueDescription("TestQueue2");
                qd.MaxSizeInMegabytes = 5120;
                qd.DefaultMessageTimeToLive = new TimeSpan(0, 1, 0);
                qd.EnablePartitioning = true; // enabled by default so not really needed
                qd.AutoDeleteOnIdle = TimeSpan.FromMinutes(5);
                namespaceManager.CreateQueue(qd);
                Console.WriteLine("Queue created.");
            }

            // Send a message to a specific parition.
            QueueClient client = QueueClient.CreateFromConnectionString(connectionString, "TestQueue2");
            BrokeredMessage message = new BrokeredMessage("This is the body");
            // All messages that use the same partition key are assigned to the same fragment. If the fragment is temporarily unavailable, Service Bus returns an error.
            // For advice on what parition key to use see: https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-partitioning
            message.PartitionKey = "OutdoorDepartment";
            // Set some additional custom app-specific properties
            message.Label = "This is the label - " + DateTime.UtcNow.ToString();
            message.Properties["TestProperty"] = "TestValue";
            client.Send(message);

            // Receive the message. You do not normally specify a parition from which you want to receive (but it can be done). 
            QueueClient clientReceive = QueueClient.CreateFromConnectionString(connectionString, "TestQueue2", ReceiveMode.ReceiveAndDelete);
            Console.WriteLine("Waiting...");
            BrokeredMessage messageReceive = clientReceive.Receive();

            if (message != null)
            {
                Console.WriteLine("Received message....");
                Console.WriteLine();
                Console.WriteLine("Partition key: " + messageReceive.PartitionKey);
                Console.WriteLine("Body: " + messageReceive.GetBody<string>());
                Console.WriteLine("MessageID: " + messageReceive.MessageId);
                Console.WriteLine("Test Property: " + messageReceive.Properties["TestProperty"]);
            }
        }
    }
}
