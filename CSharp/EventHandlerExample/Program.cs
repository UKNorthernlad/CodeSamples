using System;

namespace EventHandlerExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Stock s = new Stock("blah");

            s.SetData("newvalue");

            // When data is changed in the object "s", it raises an event which calls DataUpdate and passes in any relevent data.
            s.DataChanged += DataUpdate; // Call a named method "DataUpdate" to handle the event.
           //s.DataChanged += new EventHandler(DataUpdate); // An older way of writing the above line.

            //// This is the "modern" way - an line delegage - no need for a separate DateUpdate method to clutter up the code.
            ////Note the anonymous method signature still requires the same parameters as the standlone DateUpdate method.
            //s.DataChanged += delegate (object sender, EventArgs e)
            //{
            //    Console.WriteLine("*EventRaised* - A change was made to the object.");
            //};

            s.SetData("anothervalue");
        }

        // The Event handler code - only gets called once when defining the s.DataChanged handler above.
        private static void DataUpdate(object sender, EventArgs e)
        {
            Console.WriteLine("*EventRaised* - A change was made to the object.");
        }
    }

    public class Stock
    {
        // This class can raise an event called "DataChanged" - when it does, it supplies data to any listening delegages via an EventHandler instanace.
        public event EventHandler DataChanged;

        private string _data;
        public Stock(string data)
        {
            this._data = data;
        }

        public void SetData(string data)
        {
            _data = data;

            // When the thing of interest happens (_data is changed in this case) call OnTick passing in any data relevent to the event.     
            OnTick(new EventArgs());
        }


        // This is the 
        // Public and virtual means that sub-classes can override this logic and decide themselves when the listening delegates should be called.
        public virtual void OnTick(EventArgs e)
        {
            // Gets a reference to any methods (aka delegatges) which are attached and listening for events.
            EventHandler handler = DataChanged;
            handler?.Invoke(this, e);

            //// Once upon a time, you might have see the above written as:
            //if (DataChanged != null) //i.e we have some listeners for the event.
            //{
            //    DataChanged(this, e); // call delegate chain.
            //}

            //// You can set logic to decide if despite the event happening, wether the delegate chain should be called.
            //if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            //{
            //    // Providing we have at least one event listener attached, go-ahead and invoke it, passing in a reference to the object that raised the event + any data (EventArgs e) that it might want.
            //    handler?.Invoke(this, e);
            //}
        }

    }

}
