using System;

namespace lambaexamples
{
    class Program
    {
        // Remember that a delegate is a pointer to a method that has a specific signature (input parameters and return value).
        // Here we define a certain pattern which we'll refer to later as simply "SomePattern".
        delegate int SomePattern(int a, int b);

        static void Main(string[] args)
        {
            int x = 10;
            int y = 20;

            // Calling a named method.
            int r = AddNumbers(x, y);
            Console.WriteLine(r);

            // Calling a Lambda Expression.
            // Dear Compiler, write me a new method (I don't care about it's real name, keep that to yourself - but I'll just refer to it as "blah") that does the following:-
            //  1. Has the same method signature as "SomePattern", i.e. takes two integers and returns an integer.
            //  2. You can call the input parameters (p1 & p2).
            //  3. Now write a body for the method (the bit after the =>) that adds those two parameters together.
            //  4. Now somehow convert the result of that body into an int and return that.           
            SomePattern blah = (p1, p2) =>  p1 + p2;
         
            // Hey Compiler, you know that funtion you created earlier that I called "blah"? Let me call it passing in a couple of values:-
            int r1 = blah(x, y);
            Console.WriteLine(r1);

            // We can also include intermediate steps if need be.
            // Try to keep these sorts of lamda short.
            SomePattern blah2 = (x, y) =>
            {
                int aa = 2 * x;
                int bb = 3 * y;
                return aa * bb;
            };

            int r2 = blah2(x, y);
            Console.WriteLine(r2);



            // In order to use Lambdas, you need to first define a delegate e.g. SomePattern as seen in line 9.
            // However the .Net Framework defines a whole load for you, e.g.
            // public delegate TResult Func<TResult>();
            // public delegate TResult Func<T,TResult>(T arg);
            // public delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
            // all the way up to:-
            // public delegate TResult Func<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
            // 
            // This means you can use Lambdas without having to define your own delegate type, e.g.
            Func<int,int,int> blah3 = (p1, p2) => p1 + p2;
            int r3 = blah3(x, y);
            Console.WriteLine(r3);

            // There are other system supplied delegates:-
            // A Predicate takes one input and returns a bool.
            // public delegate bool Predicate<T>(T arg);
            Predicate<DateTime> p = input => input.DayOfWeek == DayOfWeek.Tuesday;
            bool todayIsTuesday = p(DateTime.Now);
            Console.WriteLine("Today is Tuesday = {0}",todayIsTuesday);

            // There are also "Action" delegates, these work just like Func but always return void (i.e. don't have a TResult), e.g.
            // public delegate void Action<T>(T arg);
            Action<string> blah4 = (s1) => Console.WriteLine("You passed the string: {0}", s1);
            blah4("hello world");

        }

        static int AddNumbers(int a, int b)
        {
            return a + b;
        }

    }
}
