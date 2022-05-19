using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.UnitTests
{
    [TestClass]
    public class SQLWriter
    {
        private readonly Person person = new Person()
        {
            FirstName = "Joe",
            LastName = "Shmo",
            Age = 25,
            Weight = 182.6
        };
        private readonly Dog dog = new Dog()
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

        [TestMethod]
        public void WritesCreateWithForeignKey()
        {
            string create = "CREATE TABLE IF NOT EXISTS Dog (" +
                "ID INTEGER PRIMARY KEY NOT NULL, " +
                "PersonID INTEGER NOT NULL, " +
                "Name STRING NOT NULL, " +
                "Age INT NOT NULL, " +
                "Weight DOUBLE NOT NULL, " +
                "FOREIGN KEY(PersonID) REFERENCES Person(ID))";
            Assert.AreEqual(create, dog.GetCreate());
        }

        [TestMethod]
        public void WritesReadWithForeignKey()
        {
            string read = "SELECT * FROM Dog";
            Assert.AreEqual(read, dog.GetRead());
        }

        [TestMethod]
        public void WritesUpdateWithForeignKey()
        {
            string update = "UPDATE Dog SET " +
                "PersonID = {1}, " +
                "Name = '{2}', " +
                "Age = {3}, " +
                "Weight = {4} " +
                "WHERE ID = {0}";
            Assert.AreEqual(string.Format(update, dog.ID, dog.PersonID, dog.Name, dog.Age, dog.Weight),
                dog.GetUpdate());
        }

        [TestMethod]
        public void WritesInsertWithForeignKey()
        {
            string insert = "INSERT INTO Dog (" +
                "PersonID, " +
                "Name, " +
                "Age, " +
                "Weight) " +
                "VALUES ({0}, '{1}', {2}, {3})";
            Assert.AreEqual(string.Format(insert, dog.PersonID, dog.Name, dog.Age, dog.Weight), dog.GetInsert());
        }

        [TestMethod]
        public void WritesDeleteWithForeignKey()
        {
            string delete = "DELETE FROM Dog WHERE ID = {0}";
            Assert.AreEqual(string.Format(delete, dog.ID), dog.GetDelete());
        }
    }
}
