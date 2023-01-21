using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StackExchange.Redis;
using Newtonsoft.Json;

namespace Redis
{
    // https://docs.microsoft.com/en-us/azure/redis-cache/cache-dotnet-how-to-use-azure-redis-cache
    // other examples at:
    // http://rickrainey.com/2015/02/19/azure-redis-cache/

    class Program
    {

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect("wagwan.redis.cache.windows.net:6380,password=/6o5HPslokJDhxtZWpyZCnhpRZJJi/NJMv57fPCqyPU=,ssl=True,abortConnect=False");
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        static void Main(string[] args)
        {
            IDatabase cache = Connection.GetDatabase();

            // Perform cache operations using the cache object...
            // Simple put of integral data types into the cache
            //cache.StringSet("key1", "value");
            //cache.StringSet("key2", 25);

            // Simple get of data types from the cache
            string key1 = cache.StringGet("key1");
            int key2 = (int)cache.StringGet("key2");

            // Did you know?
            // Azure Redis caches have a configurable number of databases (default of 16) that can be used to logically separate the data within a Redis cache.

            string value = cache.StringGet("key3");
            if (value == null)
            {
                // The item keyed by "key3" is not in the cache. Obtain
                // it from the desired data source and add it to the cache.
                //value = GetValueFromDataSource();
                value = "Hello world";

                // Add a value to the cache which expires in 5 mins.
                cache.StringSet("key3", value, TimeSpan.FromMinutes(5));
            }

            string result = cache.StringGet("key3");

            //Working with .net objects
            // Store to cache
            //cache.StringSet("e25", JsonConvert.SerializeObject(new Employee(25, "Billy Bunter")));

            // Retrieve from cache
            Employee e25 = JsonConvert.DeserializeObject<Employee>(cache.StringGet("e25"));

        }
    }
}
