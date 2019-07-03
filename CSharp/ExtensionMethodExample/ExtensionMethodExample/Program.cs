using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethodExample
{
    static class Program
    {
        static void Main(string[] args)
        {
            DateTime date = DateTime.Parse("1/5/2025");
            DateTime time = DateTime.Parse("1/1/0001 9:55pm");

            DateTime combined2 = date.Combine(time);
            Console.WriteLine(combined2.ToString());
        }

        static DateTime Combine(this DateTime datePart, DateTime timePart)
        {
            return new DateTime(
                datePart.Year,
                datePart.Month,
                datePart.Day,
                timePart.Hour,
                timePart.Minute,
                timePart.Second);
        }

    }



}
