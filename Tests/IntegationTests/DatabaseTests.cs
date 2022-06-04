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
        private readonly static TestDatabase TestDB = new(DatabaseProvider.TestDir);

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
                List<Person> people = TestDB.ReadAll<Person>();
                Assert.AreEqual(DatabaseProvider.People.Length, people.Count);
            }

            [TestMethod]
            public void ReadsPersonCollectionWithCorrectValues()
            {
                List<Person> people = TestDB.ReadAll<Person>().OrderBy(x => x.ID).ToList();
                List<Person> truePeople = DatabaseProvider.People.OrderBy(x => x.ID).ToList();

                for(int i = 0; i < people.Count; i++)
                {
                    truePeople[i].AssertAreEquivalent(people[i]);
                }
            }

            [TestMethod]
            public void ReadsDogCollectionOfCorrectSize()
            {
                List<Dog> dogs = TestDB.ReadAll<Dog>();
                Assert.AreEqual(DatabaseProvider.Dogs.Length, dogs.Count);
            }

            [TestMethod]
            public void ReadsDogWithCorrectValues()
            {
                List<Dog> dogs = TestDB.ReadAll<Dog>().OrderBy(x => x.ID).ToList();
                List<Dog> trueDogs = DatabaseProvider.Dogs.OrderBy(x => x.ID).ToList();

                for(int i = 0; i < dogs.Count; i++)
                {
                    trueDogs[i].AssertAreEqual(dogs[i]);
                }
            }

            [TestMethod]
            public void ReadsByID()
            {
                Person p = TestDB.ReadByID<Person>(1);
                Person? trueP = DatabaseProvider.People.Where(x => x.ID == 1).FirstOrDefault();
                if(trueP == null) { throw new Exception(); }

                trueP.AssertAreEquivalent(p);
            }

            [TestMethod]
            public void ThrowsExceptionForReadByIDWithNonExistant()
            {
                Assert.ThrowsException<InvalidOperationException>(() =>
                {
                    Person p = TestDB.ReadByID<Person>(10);
                });
            }

            [TestMethod]
            public void TryReadByIDReturnsTrueOnSuccessfulRead()
            {
                bool success = TestDB.TryReadByID(1, out Person p);
                Assert.IsTrue(success);
            }

            [TestMethod]
            public void TryReadByIDReturnsFalseOnFailedRead()
            {
                bool success = TestDB.TryReadByID(10, out Person p);
                Assert.IsFalse(success);
            }

            [TestMethod]
            public void TryReadByIDReturnsCorrectValues()
            {
                bool success = TestDB.TryReadByID(1, out Person p);
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
                TestDB.Initialize();
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
                TestDB.InsertAll(tPeople);

                List<Person> rPeople = TestDB.ReadAll<Person>().OrderBy(x => x.ID).ToList();

                for(int i = 0; i < tPeople.Count; i++)
                {
                    tPeople[i].AssertAreEquivalent(rPeople[i]);
                }
            }

            [TestMethod]
            public void InsertsSingleTable()
            {
                Person tP = DatabaseProvider.People[0];
                TestDB.Insert(tP);
                Person rP = TestDB.ReadByID<Person>(tP.ID);
                tP.AssertAreEquivalent(rP);
            }

            [TestMethod]
            public void UpdatesIDOnInsert()
            {
                Person tP = new Person()
                {
                    ID = -10
                };

                TestDB.Insert(tP);

                Assert.AreEqual(1, tP.ID);
            }

            [TestMethod]
            public void FailsToInsertNonExistantForeignKey()
            {
                Assert.ThrowsException<Exception>(() =>
                {
                    TestDB.InsertAll(DatabaseProvider.Dogs.ToList());
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
                List<Person> people = TestDB.ReadAll<Person>();
                int newAge = 10;
                people[0].Age = newAge;
                TestDB.Update(people[0]);
                Person p = TestDB.ReadByID<Person>(people[0].ID);
                Assert.AreEqual(newAge, p.Age);
            }

            [TestMethod]
            public void UpdatesCollection()
            {
                List<Person> people = TestDB.ReadAll<Person>();
                int newAge = 10;
                foreach(Person p in people)
                {
                    p.Age = newAge;
                }
                TestDB.UpdateAll(people);

                List<Person> uPeople = TestDB.ReadAll<Person>();

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
                List<Dog> dogs = TestDB.ReadAll<Dog>();
                TestDB.Delete(dogs[0]);
                Dog? dDog = TestDB.ReadAll<Dog>().Where(x => x.ID == dogs[0].ID).FirstOrDefault();
                Assert.IsNull(dDog);
            }

            [TestMethod]
            public void DeletesCollection()
            {
                TestDB.DeleteAll(TestDB.ReadAll<Dog>());
                List<Dog> dDogs = TestDB.ReadAll<Dog>();
                Assert.AreEqual(0, dDogs.Count);
            }
        }
    }
}
