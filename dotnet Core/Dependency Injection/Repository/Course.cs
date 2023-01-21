using System;
using System.Collections.Generic;
using System.Text;

namespace Ebor.Repository
{
    public class Course
    {
        public string Name { get; set; }

        private Guid guid;
        public Guid ID
        {
            get { return guid; }
        }

        public Course()
        {
            guid = Guid.NewGuid();
        }
    }
}
