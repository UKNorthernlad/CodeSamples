﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using System.Threading;

namespace CreateSchema
{
    class Program
    {
        static void Main(string[] args)
        {
            // Useful tool for working with search service: https://azsearch.azurewebsites.net/

            string searchServiceName = "ebornorth";
            string indexname = "hotels";
            string adminApiKey = "AF04406350DA2FA54BD24674A1F8B411";
            string queryApiKey = "CEA041884F40DAC1F9B0D457067C9C7D";

            #region Create Schema
            Console.WriteLine("Defining the schema.....");

            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));

            var definition = new Index()
            {
                Name = indexname,
                Fields = new[]
                {   // Every entry need a unique ID, just like an primary key. The IsKey=true denotes this.
                    new Field("hotelId", DataType.String)                       { IsKey = true, IsFilterable = true },
                    new Field("baseRate", DataType.Double)                      { IsFilterable = true, IsSortable = true, IsFacetable = true },
                    new Field("description", DataType.String)                   { IsSearchable = true },
                    //new Field("description_fr", AnalyzerName.FrLucene), // needed for multi-language support
                    new Field("hotelName", DataType.String)                     { IsSearchable = true, IsFilterable = true, IsSortable = true },
                    new Field("category", DataType.String)                      { IsSearchable = true, IsFilterable = true, IsSortable = true, IsFacetable = true },
                    new Field("tags", DataType.Collection(DataType.String))     { IsSearchable = true, IsFilterable = true, IsFacetable = true },
                    new Field("parkingIncluded", DataType.Boolean)              { IsFilterable = true, IsFacetable = true },
                    new Field("smokingAllowed", DataType.Boolean)               { IsFilterable = true, IsFacetable = true },
                    new Field("lastRenovationDate", DataType.DateTimeOffset)    { IsFilterable = true, IsSortable = true, IsFacetable = true },
                    new Field("rating", DataType.Int32)                         { IsFilterable = true, IsSortable = true, IsFacetable = true },
                    new Field("location", DataType.GeographyPoint)              { IsFilterable = true, IsSortable = true }
                }
            };

            // Now create the index.
            //serviceClient.Indexes.Create(definition);

            #endregion

            #region Upload data
            Console.WriteLine("Populating the index....");

            SearchIndexClient indexClient = serviceClient.Indexes.GetClient("hotels");

            var actions = new IndexAction<Hotel>[]
            {
                IndexAction.Upload(
                    new Hotel {
                        HotelId = "1",
                        BaseRate = 199.0,
                        Description = "Best hotel in town",
                        //DescriptionFr = "Meilleur hôtel en ville",
                        HotelName = "Fancy Stay",
                        Category = "Luxury",
                        Tags = new[] { "pool", "view", "wifi", "concierge" },
                        ParkingIncluded = false,
                        SmokingAllowed = false,
                        LastRenovationDate = new DateTimeOffset(2010, 6, 27, 0, 0, 0, TimeSpan.Zero),
                        Rating = 5,
                        Location = GeographyPoint.Create(47.678581, -122.131577)
                    }),
                IndexAction.Upload(
                    new Hotel()
                    {
                        HotelId = "2",
                        BaseRate = 79.99,
                        Description = "Cheapest hotel in town",
                        //DescriptionFr = "Hôtel le moins cher en ville",
                        HotelName = "Roach Motel",
                        Category = "Budget",
                        Tags = new[] { "motel", "budget" },
                        ParkingIncluded = true,
                        SmokingAllowed = true,
                        LastRenovationDate = new DateTimeOffset(1982, 4, 28, 0, 0, 0, TimeSpan.Zero),
                        Rating = 1,
                        Location = GeographyPoint.Create(49.678581, -122.131577)
                    }),
                IndexAction.MergeOrUpload(
                    new Hotel()
                    {
                        HotelId = "3",
                        BaseRate = 129.99,
                        Description = "Close to town hall and the river"
                    }),
                IndexAction.Delete(new Hotel() { HotelId = "6" })
            };

            var batch = IndexBatch.New(actions);

            try
            {
                indexClient.Documents.Index(batch);
            }
            catch (IndexBatchException e)
            {
                // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                // the batch. Depending on your application, you can take compensating actions like delaying and
                // retrying. For this simple demo, we just log the failed document keys and continue.
                Console.WriteLine(
                    "Failed to index some of the documents: {0}",
                    String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }

            Console.WriteLine("Waiting for documents to be indexed...\n");
            Thread.Sleep(2000);

            #endregion

            #region Perform a query

            SearchIndexClient indexqueryClient = new SearchIndexClient(searchServiceName, indexname, new SearchCredentials(queryApiKey));

            //Perform a query.
            // See https://azure.microsoft.com/en-gb/documentation/articles/search-query-dotnet/ for more examples


            Console.Write("Search the entire index, order by a specific field (lastRenovationDate) ");
            Console.Write("in descending order, take the top two results, and show only hotelName and ");
            Console.WriteLine("lastRenovationDate:\n");

            SearchParameters parameters;
            DocumentSearchResult<Hotel> results;

            parameters =
                new SearchParameters()
                {
                    OrderBy = new[] { "lastRenovationDate desc" },
                    Select = new[] { "hotelName", "lastRenovationDate" },
                    Top = 2
                };

            results = indexClient.Documents.Search<Hotel>("*", parameters);

            WriteDocuments(results);


            #endregion

            Console.ReadKey();
        }

        private static void WriteDocuments(DocumentSearchResult<Hotel> searchResults)
        {
            foreach (SearchResult<Hotel> result in searchResults.Results)
            {
                Console.WriteLine("Hotel name: {0}, Last Renovated:{1}", result.Document.HotelName,result.Document.LastRenovationDate);
            }

            Console.WriteLine();
        }
    }
}
