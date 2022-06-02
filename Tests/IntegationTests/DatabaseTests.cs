using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.IntegationTests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestClass]
        public class Initializers
        {
            [TestMethod]
            public void InitializesDatabaseWithNoExceptions()
            {
                TestDatabase testDB = new TestDatabase(DatabaseProvider.TestDir);
                testDB.Initialize();
            }
        }

        [TestClass]
        public class Reads
        {
            private static TestDatabase testDB = new TestDatabase(DatabaseProvider.TestDir);

            [TestInitialize]
            public void Initialize()
            {
                DatabaseProvider.Setup();
            }

            [TestCleanup]
            public void Cleanup()
            {
                DatabaseProvider.Cleanup();
            }

            [TestMethod]
            public void ReadsPersonCollectionOfCorrectSize()
            {
                List<Person> people = testDB.ReadAll<Person>();
                Assert.AreEqual(DatabaseProvider.People.Length, people.Count);
            }

            [TestMethod]
            public void ReadsPersonCollectionWithCorrectValues()
            {
                List<Person> people = testDB.ReadAll<Person>().OrderBy(x => x.ID).ToList();
                List<Person> truePeople = DatabaseProvider.People.OrderBy(x => x.ID).ToList();

                for(int i = 0; i < people.Count; i++)
                {
                    truePeople[i].AssertAreEquivalent(people[i]);
                }
            }

            [TestMethod]
            public void ReadsDogCollectionOfCorrectSize()
            {
                List<Dog> dogs = testDB.ReadAll<Dog>();
                Assert.AreEqual(DatabaseProvider.Dogs.Length, dogs.Count);
            }

            [TestMethod]
            public void ReadsDogWithCorrectValues()
            {
                List<Dog> dogs = testDB.ReadAll<Dog>().OrderBy(x => x.ID).ToList();
                List<Dog> trueDogs = DatabaseProvider.Dogs.OrderBy(x => x.ID).ToList();

                for(int i = 0; i < dogs.Count; i++)
                {
                    trueDogs[i].AssertAreEqual(dogs[i]);
                }
            }

            [TestMethod]
            public void ReadsByID()
            {
                Person p = testDB.ReadByID<Person>(1);
                Person? trueP = DatabaseProvider.People.Where(x => x.ID == 1).FirstOrDefault();
                if(trueP == null) { throw new Exception(); }

                trueP.AssertAreEquivalent(p);
            }

            [TestMethod]
            public void ThrowsExceptionForReadByIDWithNonExistant()
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    Person p = testDB.ReadByID<Person>(10);
                });
            }

            [TestMethod]
            public void TryReadByIDReturnsTrueOnSuccessfulRead()
            {
                bool success = testDB.TryReadByID(1, out Person p);
                Assert.IsTrue(success);
            }

            [TestMethod]
            public void TryReadByIDReturnsFalseOnFailedRead()
            {
                bool success = testDB.TryReadByID(10, out Person p);
                Assert.IsFalse(success);
            }

            [TestMethod]
            public void TryReadByIDReturnsCorrectValues()
            {
                bool success = testDB.TryReadByID(1, out Person p);
                Person? trueP = DatabaseProvider.People.Where(x => x.ID == 1).FirstOrDefault();
                Assert.IsNotNull(trueP);
                trueP.AssertAreEquivalent(p);
            }
        }
    }
}
