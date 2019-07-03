using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateSchema
{
    /*
    The first thing to notice is that each public property of Hotel corresponds to a field in the index definition, 
    but with one crucial difference: The name of each field starts with a lower-case letter("camel case"), 
    while the name of each public property of Hotel starts with an upper-case letter("Pascal case"). 
    This is a common scenario in .NET applications that perform data-binding where the target schema is outside the
    control of the application developer.Rather than having to violate the .NET naming guidelines by making property
    names camel-case, you can tell the SDK to map the property names to camel-case automatically 
    with the [SerializePropertyNamesAsCamelCase] attribute.
    */


 [SerializePropertyNamesAsCamelCase]
    public partial class Hotel
    {
        public string HotelId { get; set; }

        public double? BaseRate { get; set; }

        public string Description { get; set; }

        [JsonProperty("description_fr")]
        public string DescriptionFr { get; set; }

        public string HotelName { get; set; }

        public string Category { get; set; }

        public string[] Tags { get; set; }

        public bool? ParkingIncluded { get; set; }

        public bool? SmokingAllowed { get; set; }

        public DateTimeOffset? LastRenovationDate { get; set; }

        public int? Rating { get; set; }

        public GeographyPoint Location { get; set; }

        // ToString() method omitted for brevity...
    }
}
