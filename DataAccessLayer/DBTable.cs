using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public abstract class DBTable<T> : IDBTable
    {
        private static string? CreateString;
        private static string? DeleteString;
        private static string? InsertString;
        private static string? ReadString;
        private static string? UpdateString;

        public string GetCreate()
        {
            if(CreateString is null)
            {
                CreateString = SQLWriter.WriteCreateString(GetType());
            }

            return CreateString;
        }

        public string GetDelete()
        {
            if(DeleteString is null)
            {
                DeleteString = SQLWriter.WriteDeleteString(GetType());
            }

            return DeleteString;
        }

        public string GetInsert()
        {
            if(InsertString is null)
            {
                InsertString = SQLWriter.WriteInsertString(GetType());
            }

            return InsertString;
        }

        public string GetRead()
        {
            if(ReadString is null)
            {
                ReadString = SQLWriter.WriteReadString(GetType());
            }

            return ReadString;
        }

        public string GetUpdate()
        {
            if(UpdateString is null)
            {
                UpdateString = SQLWriter.WriteUpdateString(GetType());
            }

            return UpdateString;
        }

        public abstract void SetData(DbDataReader reader);
    }
}
