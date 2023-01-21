// Main docs start: https://docs.microsoft.com/en-us/azure/media-services/previous/media-services-dotnet-get-started
// Good sample repo: https://docs.microsoft.com/en-gb/samples/azure-samples/media-services-v3-dotnet/azure-media-services-v3-samples-using-net-50/
// This code is based on https://github.com/Azure-Samples/media-services-v3-dotnet/tree/main/VideoEncoding/Encoding_H264
// See also https://github.com/Azure-Samples/media-services-v3-dotnet/blob/main/VideoEncoding/Encoding_H264/Program.cs lines 101 to 108 for details on how you might create unique job and asset names etc.

using System;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using System.Linq;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.IO;

using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Uploader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Video Asset Uploader v1.0");

            #region Get Configuration
            Console.WriteLine("Getting configuration from appsettings.json");
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //.AddEnvironmentVariables() // parses the values from the optional .env file at the solution root
                .Build();
            #endregion

            #region Authenticate to AzureAD
            Console.WriteLine("Authenticating to AzureAD.");
            var scopes = new[] { config["ArmAadAudience"] + "/.default" };

            var app = ConfidentialClientApplicationBuilder.Create(config["AadClientId"])
                .WithClientSecret(config["AadSecret"])
                .WithAuthority(AzureCloudInstance.AzurePublic, config["AadTenantId"])
               .Build();

            // Probably this needs cleaning up!
            var authResult = app.AcquireTokenForClient(scopes).ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            ServiceClientCredentials tokenCreds = new TokenCredentials(authResult.AccessToken, "Bearer");
            #endregion

            #region Create Azure Media Client
            Console.WriteLine("Creating an Azure Media Client object.");
            var amsClient = new AzureMediaServicesClient(new Uri(config["ArmEndpoint"]), tokenCreds)
            {
                SubscriptionId = config["SubscriptionId"],
            };
            #endregion

            #region Create Input Asset
            Console.WriteLine("Creating Input Asset.");
            Asset inputAsset = amsClient.Assets.CreateOrUpdateAsync(config["ResourceGroup"], config["AccountName"], config["InputAssetName"], new Asset()).GetAwaiter().GetResult();

            // Use Media Services API to get back a response that contains
            // SAS URL for the Asset container into which to upload blobs.
            // That is where you would specify read-write permissions 
            // and the expiration time for the SAS URL.
            Console.WriteLine("Getting SAS token to upload video to BLOB storage container.");
            var response = amsClient.Assets.ListContainerSasAsync(
                config["ResourceGroup"],
                config["AccountName"],
                config["InputAssetName"],
                permissions: AssetContainerPermission.ReadWrite,
                expiryTime: DateTime.UtcNow.AddMinutes(5).ToUniversalTime())
                .GetAwaiter().GetResult();

            var sasUri = new Uri(response.AssetContainerSasUrls.First());
            Console.WriteLine("Token: " + sasUri);

            // Use Storage API to get a reference to the Asset container
            // that was created by calling Asset's CreateOrUpdate method.  
            BlobContainerClient container = new BlobContainerClient(sasUri);
            BlobClient blob = container.GetBlobClient(Path.GetFileName(config["TestVideoToUpload"]));

            // Use Storage API to upload the file into the container in storage.
            Console.WriteLine("Uploading video to the container.");
            var uploadResult = blob.UploadAsync(config["TestVideoToUpload"],true).GetAwaiter().GetResult();

            #endregion

            #region Create Transform
            Console.WriteLine("Creating Transform.");
            // Ensure that you have customized encoding Transform.  This is really a one time setup operation.
            Transform transform = CreateCustomTransform(amsClient, config["ResourceGroup"], config["AccountName"], "Custom_H264_3Layer").GetAwaiter().GetResult();
            #endregion

            #region Create Output Asset
            Console.WriteLine("Creating Output Asset");
            // Output from the Job must be written to an Asset, so let's create one
            Asset outputAsset = amsClient.Assets.CreateOrUpdateAsync(config["ResourceGroup"], config["AccountName"], config["OutputAssetName"], new Asset()).GetAwaiter().GetResult();
            #endregion

            #region Create a Job to process the Transform
            Console.WriteLine("Creating Job to run Transform.");

            Job transformJob;
            string transformJobName = config["InputAssetName"] + "-Encoding-" + Guid.NewGuid().ToString();

            JobOutput jobOutput = new JobOutputAsset()
            {
                AssetName = outputAsset.Name,
            };

            SubmitJobAsync(amsClient, config["ResourceGroup"], config["AccountName"], transform.Name, transformJobName, config["InputAssetName"], jobOutput).GetAwaiter().GetResult();


            #endregion

            #region Poll for job complete status
            Console.WriteLine("Polling:");
            const int SleepIntervalMs = 5 * 1000;

            do
            {
                transformJob = amsClient.Jobs.GetAsync(config["ResourceGroup"], config["AccountName"], config["CustomTransformName"], transformJobName).GetAwaiter().GetResult();

                Console.WriteLine($"Job is '{transformJob.State}'.");
                for (int i = 0; i < transformJob.Outputs.Count; i++)
                {
                    JobOutput output = transformJob.Outputs[i];
                    Console.Write($"\tJobOutput[{i}] is '{output.State}'.");
                    if (output.State == JobState.Processing)
                    {
                        Console.Write($"  Progress: '{output.Progress}'.");
                    }

                    Console.WriteLine();
                }

                if (transformJob.State != JobState.Finished && transformJob.State != JobState.Error && transformJob.State != JobState.Canceled)
                {
                    Task.Delay(SleepIntervalMs).GetAwaiter().GetResult(); ;
                }
            }
            while (transformJob.State != JobState.Finished && transformJob.State != JobState.Error && transformJob.State != JobState.Canceled);

            #endregion

            #region Create Streaming EndPoint
            Console.WriteLine("Creating Streaming EndPoint....");
            string locatorName = config["LocatorName"];

            StreamingLocator locator = amsClient.StreamingLocators.CreateAsync(config["ResourceGroup"], config["AccountName"], locatorName,
                new StreamingLocator
                {
                    AssetName = config["OutputAssetName"],
                    StreamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly
                }).GetAwaiter().GetResult();


            StreamingEndpoint streamingEndpoint = amsClient.StreamingEndpoints.GetAsync(config["ResourceGroup"], config["AccountName"], config["DefaultStreamingEndpointName"]).GetAwaiter().GetResult();

            if (streamingEndpoint.ResourceState != StreamingEndpointResourceState.Running)
            {
                Console.WriteLine("Streaming Endpoint was Stopped, restarting now..");
                amsClient.StreamingEndpoints.StartAsync(config["ResourceGroup"], config["AccountName"], config["DefaultStreamingEndpointName"]).GetAwaiter().GetResult();

                // Since we started the endpoint, we should stop it in cleanup.
                //stopEndpoint = true;
            }


            #endregion

            #region Get Streaming URLs
            Console.WriteLine("Getting Streaming URLs....");
            //IList<string> streamingUrls = new List<string>();

            ListPathsResponse paths = amsClient.StreamingLocators.ListPathsAsync(config["ResourceGroup"], config["AccountName"], locatorName).GetAwaiter().GetResult();

            foreach (StreamingPath path in paths.StreamingPaths)
            {
                Console.WriteLine($"The following formats are available for {path.StreamingProtocol.ToString().ToUpper()}:");
                foreach (string streamingFormatPath in path.Paths)
                {
                    UriBuilder uriBuilder = new()
                    {
                        Scheme = "https",
                        Host = streamingEndpoint.HostName,

                        Path = streamingFormatPath
                    };
                    Console.WriteLine($"\t{uriBuilder}");
                    //streamingUrls.Add(uriBuilder.ToString());
                }
                Console.WriteLine();
            }

            //foreach (string url in streamingUrls)
            //{
            //    Console.WriteLine("URL =====" + url);
            //}

            #endregion

            Console.WriteLine("\nTo try streaming, copy and paste the Streaming URL into the Azure Media Player at 'http://aka.ms/azuremediaplayer'.");
        }

        #region Support Methods

        private static async Task<Transform> CreateCustomTransform(IAzureMediaServicesClient client, string resourceGroupName, string accountName, string transformName)
        {

            Console.WriteLine("Creating a custom transform...");

            // Create a new Transform Outputs array - this defines the set of outputs for the Transform
            TransformOutput[] outputs = new TransformOutput[]
            {
                    // Create a new TransformOutput with a custom Standard Encoder Preset
                    // This demonstrates how to create custom codec and layer output settings

                  new TransformOutput(
                        new StandardEncoderPreset(
                            codecs: new Codec[]
                            {
                                // Add an AAC Audio layer for the audio encoding
                                new AacAudio(
                                    channels: 2,
                                    samplingRate: 48000,
                                    bitrate: 128000,
                                    profile: AacAudioProfile.AacLc
                                ),
                                // Next, add a H264Video for the video encoding
                               new H264Video (
                                    // Set the GOP interval to 2 seconds for all H264Layers
                                    keyFrameInterval:TimeSpan.FromSeconds(2),
                                     // Add H264Layers. Assign a label that you can use for the output filename
                                    layers:  new H264Layer[]
                                    {
                                        new H264Layer (
                                            bitrate: 3600000, // Units are in bits per second and not kbps or Mbps - 3.6 Mbps or 3,600 kbps
                                            width: "1280",
                                            height: "720",
                                            label: "HD-3600kbps" // This label is used to modify the file name in the output formats
                                        ),
                                        new H264Layer (
                                            bitrate: 1600000, // Units are in bits per second and not kbps or Mbps - 1.6 Mbps or 1600 kbps
                                            width: "960",
                                            height: "540",
                                            label: "SD-1600kbps" // This label is used to modify the file name in the output formats
                                        ),
                                        new H264Layer (
                                            bitrate: 600000, // Units are in bits per second and not kbps or Mbps - 0.6 Mbps or 600 kbps
                                            width: "640",
                                            height: "360",
                                            label: "SD-600kbps" // This label is used to modify the file name in the output formats
                                        ),
                                    }
                                ),
                                // Also generate a set of PNG thumbnails
                                new PngImage(
                                    start: "25%",
                                    step: "25%",
                                    range: "80%",
                                    layers: new PngLayer[]{
                                        new PngLayer(
                                            width: "50%",
                                            height: "50%"
                                        )
                                    }
                                )
                            },
                            // Specify the format for the output files - one for video+audio, and another for the thumbnails
                            formats: new Format[]
                            {
                                // Mux the H.264 video and AAC audio into MP4 files, using basename, label, bitrate and extension macros
                                // Note that since you have multiple H264Layers defined above, you have to use a macro that produces unique names per H264Layer
                                // Either {Label} or {Bitrate} should suffice
                                 
                                new Mp4Format(
                                    filenamePattern:"Video-{Basename}-{Label}-{Bitrate}{Extension}"
                                ),
                                new PngFormat(
                                    filenamePattern:"Thumbnail-{Basename}-{Index}{Extension}"
                                )
                            }
                        ),
                        onError: OnErrorType.StopProcessingJob,
                        relativePriority: Priority.Normal
                    )
            };

            string description = "A simple custom encoding transform with 2 MP4 bitrates";

            // Does a Transform already exist with the desired name? This method will just overwrite (Update) the Transform if it exists already. 
            // In production code, you may want to be cautious about that. It really depends on your scenario.
            Transform transform = await client.Transforms.CreateOrUpdateAsync(resourceGroupName, accountName, transformName, outputs, description);

            return transform;
        }

        private static async Task<Job> SubmitJobAsync(IAzureMediaServicesClient client, string resourceGroupName, string accountName, string transformName, string jobName, string inputAssetName, JobOutput jobOutput)
        {
            JobInput jobInput = new JobInputAsset(assetName: inputAssetName);

            JobOutput[] jobOutputs =
            {
                jobOutput
            };

            // In this example, we are assuming that the job name is unique.
            //
            // If you already have a job with the desired name, use the Jobs.Get method
            // to get the existing job. In Media Services v3, Get methods on entities returns ErrorResponseException 
            // if the entity doesn't exist (a case-insensitive check on the name).
            Job job;
            try
            {
                job = await client.Jobs.CreateAsync(
                         resourceGroupName,
                         accountName,
                         transformName,
                         jobName,
                         new Job
                         {
                             Input = jobInput,
                             Outputs = jobOutputs,
                         });
            }
            catch (Exception exception)
            {
                if (exception.GetBaseException() is ErrorResponseException apiException)
                {
                    Console.Error.WriteLine(
                        $"ERROR: API call failed with error code '{apiException.Body.Error.Code}' and message '{apiException.Body.Error.Message}'.");
                }
                throw;
            }

            return job;
        }


        #endregion

    }
}
