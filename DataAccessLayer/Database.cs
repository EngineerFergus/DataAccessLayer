using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace DataAccessLayer
{
    public abstract class Database
    {
        public string Dir { get; private set; }

        public bool FileExists => File.Exists(Dir);

        public Database(string dir)
        {
            Dir = dir;
        }

        public virtual void Initialize()
        {
            if (FileExists)
            {
                return;
            }
            else
            {
                SQLiteConnection.CreateFile(Dir);
            }
        }

        public void CreateTable<T>() where T : IDBTable, new()
        {
            T empty = new();

            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                using(SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = empty.GetCreate();
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public void Insert(IDBTable table)
        {
            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = table.GetInsert();
                    cmd.ExecuteNonQuery();
                    table.ID = conn.LastInsertRowId;
                }

                conn.Close();
            }
        }

        public void InsertAll<T>(List<T> tables) where T : IDBTable
        {
            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand cmd = conn.CreateCommand())
                        {
                            for (int i = 0; i < tables.Count; i++)
                            {
                                cmd.CommandText = tables[i].GetInsert();
                                cmd.ExecuteNonQuery();
                                tables[i].ID = conn.LastInsertRowId;
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction?.Rollback();
                    }
                }

                conn.Close();
            }
        }

        public void Update(IDBTable table)
        {
            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = table.GetUpdate();
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public void UpdateAll<T>(List<T> tables) where T : IDBTable
        {
            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand cmd = conn.CreateCommand())
                        {
                            for (int i = 0; i < tables.Count; i++)
                            {
                                cmd.CommandText = tables[i].GetUpdate();
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction?.Rollback();
                    }
                }

                conn.Close();
            }
        }

        public List<T> ReadAll<T>() where T : IDBTable, new()
        {
            List<T> collection = new();
            T empty = new();

            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = empty.GetRead();
                            using(SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    T table = new();
                                    table.SetData(reader);
                                    collection.Add(table);
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction?.Rollback();
                    }
                }

                conn.Close();
            }

            return collection;
        }

        public void Delete(IDBTable table)
        {
            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = table.GetDelete();
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public void DeleteAll<T>(List<T> tables) where T : IDBTable
        {
            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                using (SQLiteTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (SQLiteCommand cmd = conn.CreateCommand())
                        {
                            for (int i = 0; i < tables.Count; i++)
                            {
                                cmd.CommandText = tables[i].GetDelete();
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction?.Rollback();
                    }
                }

                conn.Close();
            }
        }

        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(@"Data Source= " + Dir);
        }
    }
}
