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
            s.DataChanged += DataUpdate;

            s.SetData("anothervalue");
        }

        // The Event handler code.
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

            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                // Providing we have at least one event listener attached, go-ahead and invoke it, passing in a reference to the object that raised the event + any data (EventArgs e) that it might want.
                handler?.Invoke(this, e);
            }            
        }

    }

}
