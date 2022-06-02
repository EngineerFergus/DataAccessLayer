using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;

namespace Tests
{
    internal class TestDatabase : Database
    {
        public TestDatabase(string directory) : base(directory) { }

        public override void Initialize()
        {
            base.Initialize();
            CreateTable<Person>();
            CreateTable<Dog>();
        }
    }
}
