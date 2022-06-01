using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;

namespace Tests
{
    internal static class DatabaseProvider
    {
        public static readonly string TestDir = "C:\\temp\\testdb.sqlite";

        public static readonly Person[] People = new Person[]
        {
            new Person()
            {
                ID = 1,
                FirstName = "Billy",
                LastName = "Bob",
                Age = 21,
                Weight = 220.1
            },
            
            new Person()
            {
                ID = 2,
                FirstName = "Ricky",
                LastName = "Bobby",
                Age = 44,
                Weight = 196.7
            },

            new Person()
            {
                ID = 2,
                FirstName = "Keanu",
                LastName = "Reeves",
                Age = 52,
                Weight = 180.0
            }
        };

        public static readonly Dog[] Dogs = new Dog[]
        {
            new Dog()
            {
                PersonID = 1,
                Name = "Sparky",
                Age = 12,
                Weight = 50.2
            },

            new Dog()
            {
                PersonID = 1,
                Name = "Flash",
                Age = 4,
                Weight = 35.6
            },

            new Dog()
            {
                PersonID = 3,
                Name = "Bruce",
                Age = 7,
                Weight = 47.1
            }
        };

        public static void Setup()
        {
            string? folder = Path.GetDirectoryName(TestDir);
            if(folder is null) { throw new Exception(); }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            SQLiteConnection.CreateFile(TestDir);

            string createPerson = "CREATE TABLE IF NOT EXISTS Person (" +
                "ID INTEGER PRIMARY KEY NOT NULL, " +
                "FirstName TEXT NOT NULL, " +
                "LastName TEXT NOT NULL, " +
                "Age INT NOT NULL, " +
                "Weight DOUBLE NOT NULL)";

            string createDog = "CREATE TABLE IF NOT EXISTS Dog (" +
                "ID INTEGER PRIMARY KEY NOT NULL, " +
                "PersonID INTEGER NOT NULL, " +
                "Name TEXT NOT NULL, " +
                "Age INT NOT NULL, " +
                "Weight DOUBLE NOT NULL, " +
                "FOREIGN KEY(PersonID) REFERENCES Person(ID))";

            string insertPerson = "INSERT INTO Person (" +
                "FirstName, " +
                "LastName, " +
                "Age, " +
                "Weight) " +
                "VALUES ('{0}', '{1}', {2}, {3})";

            string insertDog = "INSERT INTO Dog (" +
                "PersonID, " +
                "Name, " +
                "Age, " +
                "Weight) " +
                "VALUES ({0}, '{1}', {2}, {3})";

            using (SQLiteConnection conn = CreateConnection())
            {
                using (SQLiteCommand command = conn.CreateCommand())
                {
                    command.CommandText = createPerson;
                    command.ExecuteNonQuery();
                    command.CommandText = createDog;
                    command.ExecuteNonQuery();

                    foreach(Person p in People)
                    {
                        command.CommandText = string.Format(insertPerson, p.FirstName, p.LastName, p.Age, p.Weight);
                        command.ExecuteNonQuery();
                    }

                    foreach(Dog d in Dogs)
                    {
                        command.CommandText = string.Format(insertDog, d.PersonID, d.Name, d.Age, d.Weight);
                        command.ExecuteNonQuery();
                    }
                }
            }

        }

        public static void Cleanup()
        {
            if (File.Exists(TestDir))
            {
                File.Delete(TestDir);
            }
        }

        private static SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(@"Data Source= " + TestDir);
        }
    }
}
