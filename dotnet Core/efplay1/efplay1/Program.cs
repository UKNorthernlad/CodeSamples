using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace efplay1
{
    class Program
    {
        static void Main(string[] args)
        {
            Person a = new Person { FirstName = "wibble", LastName = "blah", Speciality = "Stuff" };

            using (var context = new OurDatabaseContext())
            {
                //context.People.Add(a);
                //context.SaveChanges();

                var result = context.People.Find(2);
                Console.WriteLine("Firstname={0}, Lastname={1}, Speciality={2}",result.FirstName,result.LastName,result.Speciality);

                // .Where results in a "select * from people" query - not efficient.
                var result2 = context.People.Where(x => x.Speciality == "Stuff");
                Console.WriteLine(context.People.Sql);
                foreach (var p in result2)
                {
                    Console.WriteLine("Firstname={0}, Lastname={1}, Speciality={2}", p.FirstName, p.LastName, p.Speciality);
                }

                var result3 = context.People.



            }
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Speciality { get; set; }
    }

    public class OurDatabaseContext : DbContext
    {
        public DbSet<Person> People { get; set; }
    }


}
