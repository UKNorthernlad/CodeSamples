using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebApplication1.Models;

using Azure.Storage.Blobs;

using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace WebApplication1.Controllers
{

    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            TelemetryClient telemetry = new TelemetryClient();
            telemetry.TrackEvent("StartofAnewFlow");

            var sample = new MetricTelemetry();
            sample.Name = "usercomputertime";
            sample.Sum = 45;
            telemetry.TrackMetric(sample);

            CloudStorageAccount storageAccount;
            // Get information about a storage account from a connection string.
            storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=myjumlstorage;AccountKey=JlEoLSkdSvL0z4S73JPjp+AdMipXwt3sMYKB7hW6Kl4Kp3IYVa6Kaf9C9uFI6kdZh6vJx7D3gj5N73cWEk5rCg==;EndpointSuffix=core.windows.net");
            
            // Create a local client for working with the storage account.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            
            // Get a reference to a container in the storage account.
            CloudBlobContainer container = blobClient.GetContainerReference("jums2");
            await container.CreateIfNotExistsAsync();

            // Create a page blob in the newly created container.  
            Guid g = Guid.NewGuid();
            string name = "myblob-" + g.ToString() + ".jpg";
            CloudPageBlob pageBlob = container.GetPageBlobReference(name);

            
            await pageBlob.CreateAsync(512 * 2 /*size*/); // size needs to be multiple of 512 bytes

            // Write data to the new blob.
            byte[] samplePagedata = new byte[512];
            Random random = new Random();
            random.NextBytes(samplePagedata);
            await pageBlob.UploadFromByteArrayAsync(samplePagedata, 0, samplePagedata.Length);




            return View();
        }

        public async Task<IActionResult> Privacy()
        {

            int x = 0;
            x = x + 1;
            x--;
            int y = 10 / x;


            // Get information about a storage account from a connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=myjumlstorage;AccountKey=JlEoLSkdSvL0z4S73JPjp+AdMipXwt3sMYKB7hW6Kl4Kp3IYVa6Kaf9C9uFI6kdZh6vJx7D3gj5N73cWEk5rCg==;EndpointSuffix=core.windows.net");

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = new CloudTableClient(storageAccount.TableEndpoint, storageAccount.Credentials);

            CloudTable table =  tableClient.GetTableReference("blah");

            await table.CreateIfNotExistsAsync();

            CustomerEntity customer = new CustomerEntity("Harp", "Walter")
            {
                Email = "Walter@contoso.com",
                PhoneNumber = "425-555-0101"
            };

            // Create the InsertOrReplace table operation
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(customer);

            // Execute the operation.
            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
            CustomerEntity insertedCustomer = result.Result as CustomerEntity;

            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
