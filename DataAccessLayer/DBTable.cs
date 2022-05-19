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
        private static string? _CreateString;
        protected string CreateString
        {
            get
            {
                if(_CreateString == null) 
                { 
                    _CreateString = SQLWriter.WriteCreateString(GetType());
                }

                return _CreateString;
            }
        }

        private static string? _DeleteString;
        protected string DeleteString
        {
            get
            {
                if(_DeleteString is null)
                {
                    _DeleteString = SQLWriter.WriteDeleteString(GetType());
                }

                return _DeleteString;
            }
        }

        private static string? _InsertString;
        protected string InsertString
        {
            get
            {
                if(_InsertString is null)
                {
                    _InsertString = SQLWriter.WriteInsertString(GetType());
                }

                return _InsertString;
            }
        }

        private static string? _ReadString;
        protected string ReadString
        {
            get
            {
                if(_ReadString is null)
                {
                    _ReadString = SQLWriter.WriteReadString(GetType());
                }

                return _ReadString;
            }
        }

        private static string? _UpdateString;
        protected string UpdateString
        {
            get
            {
                if(_UpdateString is null)
                {
                    _UpdateString = SQLWriter.WriteUpdateString(GetType());
                }

                return _UpdateString;
            }
        }

        private long _ID;
        [DBPrimaryKey]
        public long ID
        {
            get { return _ID; }
            set
            {
                if(value != _ID)
                {
                    _ID = value;
                    OnIDChanged();
                }
            }
        }

        public string GetCreate()
        {
            return CreateString;
        }

        public string GetDelete()
        {
            return FormatDelete();
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
        protected abstract string FormatDelete();
        protected abstract void OnIDChanged();
        public abstract void UpdateForeignKey(long key);
    }
}
