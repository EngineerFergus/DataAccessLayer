using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;

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

        public List<Dog> Dogs { get; }

        public Person()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Dogs = new List<Dog>();
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

        protected override void OnIDChanged()
        {
            foreach(Dog dog in Dogs)
            {
                dog.UpdateForeignKey(ID);
            }
        }

        public override void UpdateForeignKey(long key)
        {
            return;
        }

        public void AssertAreEquivalent(Person compare)
        {
            if(compare.ID != ID) { Throw("ID were not equal"); }
            if (compare.FirstName != FirstName) { Throw("FirstName did not equal"); }
            if (compare.LastName != LastName) { Throw("LastName did not equal"); }
            if(compare.Age != Age) { Throw("Age did not equal"); }
            double dx = Math.Abs(Weight - compare.Weight);
            if(dx >= 0.0001 * Weight) { Throw("Weight did not meet tolerance"); }
        }

        private void Throw(string message)
        {
            throw new Exception($"Exception in {nameof(Person)}, {message}");
        }
    }
}
