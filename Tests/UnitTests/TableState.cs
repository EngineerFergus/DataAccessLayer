using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.UnitTests
{
    [TestClass]
    public class TableState
    {
        [TestMethod]
        public void UpdatesIDOnParentIDChanged()
        {
            Person person = new Person()
            {
                ID = 1
            };

            Dog dog = new Dog()
            {
                PersonID = 1,
                ID = 1
            };

            person.Dogs.Add(dog);

            person.ID = 2;

            Assert.AreEqual(2, dog.PersonID);
        }
    }
}
