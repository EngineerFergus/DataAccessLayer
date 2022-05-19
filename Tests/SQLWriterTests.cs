using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class SQLWriterTests
    {
        private Person person = new Person()
        {
            FirstName = "Joe",
            LastName = "Shmo",
            Age = 25,
            Weight = 182.6
        };
        private Dog dog = new Dog()
        {
            PersonID = 0,
            Name = "Flash",
            Age = 4,
            Weight = 35.2
        };

        [TestMethod]
        public void WritesCreate()
        {
            string create = "CREATE TABLE IF NOT EXISTS Person (" +
                "ID INTEGER PRIMARY KEY NOT NULL, " +
                "FirstName STRING NOT NULL, " +
                "LastName STRING NOT NULL, " +
                "Age INT NOT NULL, " +
                "Weight DOUBLE NOT NULL)";
            Assert.AreEqual(create, person.GetCreate());
        }

        [TestMethod]
        public void WritesRead()
        {
            string read = "SELECT * FROM Person";
            Assert.AreEqual(read, person.GetRead());
        }

        [TestMethod]
        public void WritesInsert()
        {
            string insert = "INSERT INTO Person (" +
                "FirstName, " +
                "LastName, " +
                "Age, " +
                "Weight) " +
                "VALUES ('{0}', '{1}', {2}, {3})";
            Assert.AreEqual(string.Format(insert, person.FirstName, person.LastName, person.Age, person.Weight), 
                person.GetInsert());
        }

        [TestMethod]
        public void WritesUpdate()
        {
            string update = "UPDATE Person SET " +
                "FirstName = '{1}', " +
                "LastName = '{2}', " +
                "Age = {3}, " +
                "Weight = {4} " +
                "WHERE ID = {0}";
            Assert.AreEqual(string.Format(update, person.ID, person.FirstName, person.LastName, person.Age, person.Weight),
                person.GetUpdate());
        }

        [TestMethod]
        public void WritesDelete()
        {
            string delete = "DELETE FROM Person WHERE ID = {0}";
            Assert.AreEqual(string.Format(delete, person.ID), person.GetDelete());
        }
    }
}
