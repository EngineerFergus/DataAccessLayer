using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using DataAccessLayer.Attributes;

namespace Tests
{
    [DBTable("Person")]
    internal class Person : DBTable<Person>
    {
        [DBColumn("FirstName", 1)]
        public string FirstName { get; set; }

        [DBColumn("LastName", 2)]
        public string LastName { get; set; }

        [DBColumn("Age", 3)]
        public int Age { get; set; }

        [DBColumn("Weight", 4)]
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

        protected override string FormatUpdate()
        {
            return string.Format(UpdateString, ID, FirstName, LastName, Age, Weight);
        }

        protected override string FormatInsert()
        {
            return string.Format(InsertString, FirstName, LastName, Age, Weight);
        }

        protected override string FormatDelete()
        {
            return string.Format(DeleteString, ID);
        }
    }
}
