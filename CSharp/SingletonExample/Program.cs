using System;
using System.Threading;

namespace SingletonExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //SingletonClass c = new SingletonClass();

            SingletonClass c1 = SingletonClass.TheRealClass;
            SingletonClass c2 = SingletonClass.TheRealClass;

            Console.WriteLine(c1.TheTime.ToString());
            Console.WriteLine(c1.Guid.ToString());
            Console.WriteLine(c2.TheTime.ToString());
            Console.WriteLine(c2.Guid.ToString());
            Thread.Sleep(5000);
            Console.WriteLine(c1.TheTime.ToString());
            Console.WriteLine(c1.Guid.ToString());
            Console.WriteLine(c2.TheTime.ToString());
            Console.WriteLine(c2.Guid.ToString());

        }
    }

    public class SingletonClass
    {
        //private static readonly SingletonClass _instance = new SingletonClass();

        private Guid guid;

        private DateTime creationTime;

        private static SingletonClass me = new SingletonClass();

        public static SingletonClass TheRealClass
        {
            get
            {
                return me;
            }
        }

        public DateTime TheTime
        {
            get
            {
                return creationTime;
            }
        }

        public Guid Guid
        {
            get
            {
                return guid;
            }
        }

        private SingletonClass()
        {
            guid = Guid.NewGuid();
            creationTime = DateTime.Now;
        }
    }
}
