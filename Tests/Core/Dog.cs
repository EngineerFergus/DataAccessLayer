using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;

namespace Tests
{
    [DBTable("Dog")]
    internal class Dog : DBTable<Dog>
    {
        [DBColumn("PersonID", 1), DBForeignKey("Person")]
        public long PersonID { get; set; }

        [DBColumn("Name", 2)]
        public string Name { get; set; }

        [DBColumn("Age", 3)]
        public int Age { get; set; }

        [DBColumn("Weight", 4)]
        public double Weight { get; set; }

        public Dog()
        {
            Name = string.Empty;
        }

        public override void SetData(DbDataReader reader)
        {
            ID = (long)reader["ID"];
            PersonID = (long)reader["PersonID"];
            Name = (string)reader["Name"];
            Age = (int)reader["Age"];
            Weight = (double)reader["Weight"];
        }

        protected override string FormatDelete()
        {
            return string.Format(DeleteString, ID);
        }

        protected override string FormatInsert()
        {
            return string.Format(InsertString, PersonID, Name, Age, Weight);
        }

        protected override string FormatUpdate()
        {
            return string.Format(UpdateString, ID, PersonID, Name, Age, Weight);
        }

        protected override void OnIDChanged()
        {
            return;
        }

        public override void UpdateForeignKey(long key)
        {
            PersonID = key;
        }

        public void AssertAreEqual(Dog compare)
        {
            if (ID != compare.ID) { Throw("IDs did not match"); }
            if (Name != compare.Name) { Throw("Names did not match"); }
            if (PersonID != compare.PersonID) { Throw("PersonIDs did not match"); }
            if (Age != compare.Age) { Throw("Ages did not match"); }
            double err = Math.Abs(Weight - compare.Weight);
            if (err >= 0.0001 * Weight) { Throw("Weights did not meet tolerance"); }
        }

        private void Throw(string message)
        {
            throw new Exception($"Exception in {nameof(Dog)}, {message}");
        }
    }
}
