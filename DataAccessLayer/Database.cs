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
                using (SQLiteConnection conn = CreateConnection())
                {
                    conn.Open();
                    using (SQLiteCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "PRAGMA FOREIGN_KEYS = ON";
                        command.ExecuteNonQuery();
                    }
                    conn.Close();
                }
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

        public void Insert<T>(T table) where T : IDBTable, new()
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

        public void InsertAll<T>(List<T> tables) where T : IDBTable, new()
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
                    catch (Exception e)
                    {
                        transaction?.Rollback();
                        throw new Exception($"Exception in {nameof(Database)}.{nameof(InsertAll)} with type {typeof(T).FullName}, " +
                            $"{e}");
                    }
                }

                conn.Close();
            }
        }

        public void Update<T>(T table) where T : IDBTable, new()
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

        public void UpdateAll<T>(List<T> tables) where T : IDBTable, new()
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
                    catch(Exception e)
                    {
                        transaction?.Rollback();
                        throw new Exception($"Exception in {nameof(Database)}.{nameof(UpdateAll)} with type {typeof(T).FullName}, " +
                            $"{e}");
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
                    catch (Exception e)
                    {
                        transaction?.Rollback();
                        throw new Exception($"Exception in {nameof(Database)}.{nameof(ReadAll)} with type {typeof(T).FullName}, " +
                            $"{e}");
                    }
                }

                conn.Close();
            }

            return collection;
        }

        public T ReadByID<T>(long id) where T : IDBTable, new()
        {
            T table = new();

            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = string.Format(table.GetReadByID(), id);

                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        table.SetData(reader);
                    }
                }

                conn.Close();
            }

            return table;
        }

        public bool TryReadByID<T>(long id, out T table) where T : IDBTable, new()
        {
            bool wasSuccessful = false;
            table = new();

            using (SQLiteConnection conn = CreateConnection())
            {
                conn.Open();

                try
                {
                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = string.Format(table.GetReadByID(), id);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            table.SetData(reader);
                        }
                    }

                    wasSuccessful = true;
                }
                catch
                {
                    wasSuccessful = false;
                }

                conn.Close();
            }

            return wasSuccessful;
        }

        public void Delete<T>(T table) where T : IDBTable, new()
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

        public void DeleteAll<T>(List<T> tables) where T : IDBTable, new()
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
                    catch (Exception e)
                    {
                        transaction?.Rollback();
                        throw new Exception($"Exception in {nameof(Database)}.{nameof(DeleteAll)} with type {typeof(T).FullName}, " +
                            $"{e}");
                    }
                }

                conn.Close();
            }
        }

        private SQLiteConnection CreateConnection()
        {
            if (!FileExists)
            {
                throw new Exception($"Exception in {nameof(Database)}.{nameof(CreateConnection)}, " +
                    $"sqlite file did not exist at {Dir}");
            }

            return new SQLiteConnection(@"Data Source= " + Dir);
        }
    }
}
