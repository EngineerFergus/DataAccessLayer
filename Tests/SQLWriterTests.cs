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
        private Person person = new Person();

        [TestMethod]
        public void WritesCreate()
        {
            string create = "CREATE TABLE IF NOT EXISTS Person(" +
                ")";
            Assert.AreEqual(create, person.GetCreate());
        }

        [TestMethod]
        public void WritesRead()
        {
            string read = "This has not been written yet";
            Assert.AreEqual(read, person.GetRead());
        }

        [TestMethod]
        public void WritesInsert()
        {
            string insert = "This has not been written yet";
            Assert.AreEqual(insert, person.GetInsert());
        }

        [TestMethod]
        public void WritesUpdate()
        {
            string update = "Not written";
            Assert.AreEqual(update, person.GetUpdate());
        }

        [TestMethod]
        public void WritesDelete()
        {
            string delete = "not written";
            Assert.AreEqual(delete, person.GetDelete());
        }
    }
}
