using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace UKNorthernlad.BuildTasks
{
    public class DoCoolThing : Task
    {

        [Microsoft.Build.Framework.Required]
        public string Message
        {
            get;
            set;
        }

        [Output]
        public string TheDate
        {
            get { return DateTime.Now.ToString(); }
        }


        public override bool Execute()
        {
            Console.WriteLine();
            Console.Write(Message);
            Console.WriteLine();
            return true;
        }
    }
}