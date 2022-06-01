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
        }
    }
}
