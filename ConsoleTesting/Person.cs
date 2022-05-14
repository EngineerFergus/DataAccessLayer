using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataAccessLayer.Attributes;

namespace ConsoleTesting
{
    [DBTable("Person")]
    internal class Person : DBTable<Person>
    {
        [DBColumn("FirstName")]
        public string FirstName { get; set; }

        [DBColumn("LastName")]
        public string LastName { get; set; }

        [DBColumn("Age")]
        public int Age { get; set; }

        [DBColumn("Weight")]
        public double Weight { get; set; }

        public Person()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        public override void SetData(DbDataReader reader)
        {
            ID = (long)reader["ID"];
            FirstName = (string)reader["FirstName"];
            LastName = (string)reader["LastName"];
            Age = (int)reader["Age"];
            Weight = (double)reader["Weight"];
        }

        public override string FormatUpdate()
        {
            throw new NotImplementedException();
        }

        public override string FormatInsert()
        {
            throw new NotImplementedException();
        }
    }
}
