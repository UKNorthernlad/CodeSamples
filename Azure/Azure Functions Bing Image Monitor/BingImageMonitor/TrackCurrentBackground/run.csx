#r "Newtonsoft.Json"

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

const string urlbase = "http://www.bing.com";

public static void Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

    string[] markets = new string[] {
    "ar-XA", "en-XA", "es-AR", "en-AU", "de-AT", "nl-BE", "fr-BE", "pt-BR",
    "en-CA", "fr-CA", "es-CL", "da-DK", "fi-FI", "fr-FR", "de-DE", "zh-HK",
    "en-IN", "en-WW", "en-IE", "it-IT", "ja-JP", "ko-KR", "es-XL", "es-MX",
    "nl-NL", "en-NZ", "nb-NO", "zh-CN", "pt-PT", "en-PH", "ru-RU", "en-SG",
     "es-ES", "sv-SE", "fr-CH", "de-CH", "zh-TW", "en-GB", "en-US", "es-US"
     };

    foreach (string market in markets)
    {
        string current = GetCurrentUrl(market);
        string saved = GetSaved(market);

        // Check to see if it's a new image.
        if (current != saved)
        {
            log.Info($"New Image Found - {current}");

            //Send notification
            //TODO;

            UpsertTable(market, current);
        }
        else
        {
            log.Info($"No changes found for {market}");
        }
    }
}

public static CloudTable GetTable()
{
    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage"));
    //mainLog.Info($"DEBUG: {storageAccount}");

    // Create the table client.
    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

    // Retrieve a reference to the table.
    CloudTable table = tableClient.GetTableReference("images");

    // Create the table if it doesn't exist.
    table.CreateIfNotExists();

    return table;
}

public static void UpsertTable(string market, string fullurl)
{
    CloudTable table = GetTable();

    // Store the new image Url back into the table
    ImageEntity image = new ImageEntity(market);
    image.Url = fullurl;

    // Create the TableOperation object that inserts the customer entity.
    TableOperation insertOperation = TableOperation.InsertOrReplace(image);

    // Execute the insert operation.
    table.Execute(insertOperation);
}

public static string GetSaved(string market)
{
    string retrieved = String.Empty;

    CloudTable table = GetTable();

    // Get the currently stored image location
    TableOperation retrieveOperation = TableOperation.Retrieve<ImageEntity>(market, "current");

    // Execute the retrieve operation.
    TableResult retrievedResult = table.Execute(retrieveOperation);

    if (retrievedResult.Result != null)
    {
        retrieved = ((ImageEntity)retrievedResult.Result).Url;
        //mainLog.Info($"DEBUG - Retrieved Url: {retrieved}");
    }

    return retrieved;
}

public class ImageEntity : TableEntity
{
    public ImageEntity(string market)
    {
        this.PartitionKey = market;
        this.RowKey = "current";
    }

    public ImageEntity() { }

    public string Url { get; set; }
}

public static string GetCurrentUrl(string market)
{
    string imageurl;
    string url = urlbase + "/HPImageArchive.aspx?format=js&idx=0&n=10&video=1&setmkt=" + market;

    using (StreamReader reader = new StreamReader(WebRequest.Create(url).GetResponse().GetResponseStream()))
    {
        string json = reader.ReadToEnd();

        JObject o = JObject.Parse(json);
        JArray images = (JArray)o["images"];

        imageurl = (string)images[0]["url"];

        //mainLog.Info($"Image URL: {fullurl}");
    }

    return urlbase + imageurl;
}
