using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

// Taken from https://docs.microsoft.com/en-us/azure/cosmos-db/tutorial-develop-documentdb-dotnet
// Examples at https://docs.microsoft.com/en-us/azure/cosmos-db/documentdb-dotnet-samples
// Worked project examples at https://github.com/Azure/azure-documentdb-dotnet/

// Cosmos DB datamigration tool: https://github.com/azure/azure-documentdb-datamigrationtool
// Compiled version: https://www.microsoft.com/en-us/download/details.aspx?id=46436
// Connection string: AccountEndpoint=https://elvis99.documents.azure.com:443/;AccountKey=V1HSSfNb7HVX2rYMGXeCkAmd3SKZuVWqAeSV8NX7xcubCideGt6FnxAlznDV8eKj8StlgaPGpLT5SkhgqsTQjQ==;Database=MyApp

namespace CosmosDemo1
{
    class Program
    {
        private const string EndpointUrl = "https://blah99.documents.azure.com:443/";
        private const string PrimaryKey = "qkSU7N7163mmumpVQVQTHU6khNFF6Q7bwhmTVCATY2xqQoNx0OXicUzdmS6Fa43qjYO7Ql4OxOcTwca6ZVYXIw==";
        private const string DbId = "MyApp";
        private const string ColId = "Users";
        private const string KeyPath = "/deviceId";

        static void Main(string[] args)
        {
            Task t = Task.Run(async () =>
            {
                DocumentClient client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

                // Create database.
                var dbresult = await client.CreateDatabaseIfNotExistsAsync(new Database { Id = DbId });
                Console.WriteLine(dbresult.StatusCode.ToString());

                // Create collection.
                // Here the JSON property deviceId is used as the partition key to spread across partitions. Configured for 2500 RU/s throughput and an indexing policy that supports sorting against any  number or string property. .
                DocumentCollection myCollection = new DocumentCollection();
                myCollection.Id = ColId;
                myCollection.PartitionKey.Paths.Add(KeyPath);
                var colresult = await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DbId), myCollection, new RequestOptions { OfferThroughput = 400 });
                Console.WriteLine(colresult.StatusCode.ToString());

                // Create a document.
                var doccreateresult = await client.CreateDocumentAsync(
                     UriFactory.CreateDocumentCollectionUri(DbId, ColId),
                     new DeviceReading
                     {
                         Id = "XMS-001-FE24C",
                         DeviceId = "XMS-0001",
                         MetricType = "Temperature",
                         MetricValue = 32,
                         Unit = "Celsius",
                         ReadingTime = DateTime.UtcNow
                     });
                Console.WriteLine(doccreateresult.StatusCode);

                // Read document. Needs the partition key and the Id to be specified.
                Document result = await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DbId, ColId, "XMS-001-FE24C"), new RequestOptions { PartitionKey = new PartitionKey("XMS-0001") });
                DeviceReading reading = (DeviceReading)(dynamic)result;
                Console.WriteLine(reading.MetricValue);

                // Update the document. Partition key is not required, again extracted from the document
                reading.MetricValue = 40;
                reading.ReadingTime = DateTime.UtcNow;
                var updateresult = await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DbId, ColId, "XMS-001-FE24C"), reading);
                Console.WriteLine(updateresult.StatusCode);

                // Query to find all documents which match a pattern. Cosmos DB automatically routes the query to the partitions corresponding to the partition key values specified in the filter (if there are any).
                IQueryable<DeviceReading> partitionQuery = client.CreateDocumentQuery<DeviceReading>(UriFactory.CreateDocumentCollectionUri(DbId, ColId)).Where(m => m.MetricType == "Temperature" && m.DeviceId == "XMS-0001");

                foreach (DeviceReading devreading in partitionQuery.ToList<DeviceReading>())
                {
                    Console.WriteLine(devreading.MetricValue);
                }

                // Query to find all documents which match a pattern - no partition key has been specified therefore the query automatically fans out across all partitions. Costs more RU/s.
                IQueryable<DeviceReading> crossPartitionQuery = client.CreateDocumentQuery<DeviceReading>(UriFactory.CreateDocumentCollectionUri(DbId, ColId), new FeedOptions { EnableCrossPartitionQuery = true }).Where(m => m.MetricType == "Temperature" && m.MetricValue > 25);

                foreach (DeviceReading devreading in crossPartitionQuery.ToList<DeviceReading>())
                {
                    Console.WriteLine(devreading.MetricValue);
                }


                // Delete a document. The partition key is required.
                //var deleteresult = await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DbId, ColId, "XMS-001-FE24C"), new RequestOptions { PartitionKey = new PartitionKey("XMS-0001") });
                //Console.WriteLine(deleteresult.StatusCode);

                //Delete the collection
                //await client.DeleteDocumentCollectionAsync(colresult.Resource.SelfLink, null);

                // Delete the database
                //await client.DeleteDatabaseAsync(dbresult.Resource.SelfLink, null);
            });
            t.Wait();
        }

    }
}
