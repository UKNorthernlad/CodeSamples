using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Net.Http;

namespace WriteEventLog
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the source, if it does not already exist.
            if (!EventLog.SourceExists("MyApp"))
            {
                //An event log source should not be created and immediately used.
                //There is a latency time to enable the source, it should be created
                //prior to executing the application that uses the source.
                //Execute this sample a second time to use the new source.
                EventLog.CreateEventSource("MyApp", "Application");
                Console.WriteLine("CreatedEventSource");
                Console.WriteLine("Exiting, execute the application a second time to use the source.");
                // The source is created.  Exit the application to allow it to be registered.
                return;
            }

            EventLogEntryType etype = EventLogEntryType.Information;
            string message = "This is the default message";

            switch (args[0])
            {
                case "1":
                    etype = EventLogEntryType.Information;
                    break;
                case "2":
                    etype = EventLogEntryType.Warning;
                    break;
                case "3":
                    etype = EventLogEntryType.Error;
                    break;
                default:
                    etype = EventLogEntryType.Information;
                    break;
            }

            if (args[1] != null) { message = args[1]; }

            // Create an EventLog instance and assign its source.
            EventLog myLog = new EventLog();
            // Write an informational entry to the event log. 
            myLog.Source = "MyApp";
            
            myLog.WriteEntry(message, etype);

        }
    }
}
