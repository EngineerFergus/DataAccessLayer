using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Attributes;

namespace DataAccessLayer
{
    public abstract class DBTable<T> : IDBTable
    {
        private static string? _CreateString;
        protected static string CreateString
        {
            get
            {
                if(_CreateString == null) 
                { 
                    _CreateString = SQLWriter.WriteCreateString(typeof(DBTable<T>));
                }

                return _CreateString;
            }
        }

        private static string? _DeleteString;
        protected static string DeleteString
        {
            get
            {
                if(_DeleteString is null)
                {
                    _DeleteString = SQLWriter.WriteDeleteString(typeof(DBTable<T>));
                }

                return _DeleteString;
            }
        }

        private static string? _InsertString;
        protected static string InsertString
        {
            get
            {
                if(_InsertString is null)
                {
                    _InsertString = SQLWriter.WriteInsertString(typeof(DBTable<T>));
                }

                return _InsertString;
            }
        }

        private static string? _ReadString;
        protected static string ReadString
        {
            get
            {
                if(_ReadString is null)
                {
                    _ReadString = SQLWriter.WriteReadString(typeof(DBTable<T>));
                }

                return _ReadString;
            }
        }

        private static string? _UpdateString;
        protected static string UpdateString
        {
            get
            {
                if(_UpdateString is null)
                {
                    _UpdateString = SQLWriter.WriteUpdateString(typeof(DBTable<T>));
                }

                return _UpdateString;
            }
        }


        [DBPrimaryKey]
        public long ID { get; set; }
        public string GetCreate()
        {
            return CreateString;
        }

        public string GetDelete()
        {
            return DeleteString;
        }

        public string GetInsert()
        {
            return FormatInsert();
        }

        public string GetRead()
        {
            return ReadString;
        }

        public string GetUpdate()
        {
            return FormatUpdate();
        }

        public abstract void SetData(DbDataReader reader);
        protected abstract string FormatInsert();
        protected abstract string FormatUpdate();
    }
}
