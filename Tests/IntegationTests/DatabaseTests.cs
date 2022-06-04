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
        private static TestDatabase testDB = new TestDatabase(DatabaseProvider.TestDir);

        [TestClass]
        public class Initializers
        {
            [TestMethod]
            public void InitializesDatabaseWithNoExceptions()
            {
                TestDatabase empty = new TestDatabase(DatabaseProvider.TestDir);
                empty.Initialize();
            }

            [TestMethod]
            public void EmptyTableReturnsEmptyCollection()
            {
                TestDatabase empty = new TestDatabase(DatabaseProvider.TestDir);
                empty.Initialize();
                List<Person> people = empty.ReadAll<Person>();
                Assert.AreEqual(0, people.Count);
            }
        }

        [TestClass]
        public class Reads
        {

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

        [TestClass]
        public class Inserts
        {
            [TestInitialize]
            public void Initialize()
            {
                testDB.Initialize();
            }

            [TestCleanup]
            public void Cleanup()
            {
                DatabaseProvider.Cleanup();
            }

            [TestMethod]
            public void InsertsCollection()
            {
                List<Person> tPeople = DatabaseProvider.People.OrderBy(x => x.ID).ToList();
                testDB.InsertAll(tPeople);

                List<Person> rPeople = testDB.ReadAll<Person>().OrderBy(x => x.ID).ToList();

                for(int i = 0; i < tPeople.Count; i++)
                {
                    tPeople[i].AssertAreEquivalent(rPeople[i]);
                }
            }

            [TestMethod]
            public void InsertsSingleTable()
            {
                Person tP = DatabaseProvider.People[0];
                testDB.Insert(tP);
                Person rP = testDB.ReadByID<Person>(tP.ID);
                tP.AssertAreEquivalent(rP);
            }

            [TestMethod]
            public void UpdatesIDOnInsert()
            {
                Person tP = new Person()
                {
                    ID = -10
                };

                testDB.Insert(tP);

                Assert.AreEqual(1, tP.ID);
            }

            [TestMethod]
            public void FailsToInsertNonExistantForeignKey()
            {
                Assert.ThrowsException<Exception>(() =>
                {
                    testDB.InsertAll(DatabaseProvider.Dogs.ToList());
                });
            }
        }

        [TestClass]
        public class Updates
        {
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
            public void UpdatesSingleEntry()
            {
                List<Person> people = testDB.ReadAll<Person>();
                int newAge = 10;
                people[0].Age = newAge;
                testDB.Update(people[0]);
                Person p = testDB.ReadByID<Person>(people[0].ID);
                Assert.AreEqual(newAge, p.Age);
            }

            [TestMethod]
            public void UpdatesCollection()
            {
                List<Person> people = testDB.ReadAll<Person>();
                int newAge = 10;
                foreach(Person p in people)
                {
                    p.Age = newAge;
                }
                testDB.UpdateAll(people);

                List<Person> uPeople = testDB.ReadAll<Person>();

                foreach(Person p in uPeople)
                {
                    Assert.AreEqual(newAge, p.Age);
                }
            }
        }

        [TestClass]
        public class Deletes
        {
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
            public void DeletesSingleEntry()
            {
                List<Dog> dogs = testDB.ReadAll<Dog>();
                testDB.Delete(dogs[0]);
                Dog? dDog = testDB.ReadAll<Dog>().Where(x => x.ID == dogs[0].ID).FirstOrDefault();
                Assert.IsNull(dDog);
            }

            [TestMethod]
            public void DeletesCollection()
            {
                testDB.DeleteAll(testDB.ReadAll<Dog>());
                List<Dog> dDogs = testDB.ReadAll<Dog>();
                Assert.AreEqual(0, dDogs.Count);
            }
        }
    }
}
