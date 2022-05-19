using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DBForeignKeyAttribute : Attribute
    {
        public string ForeignTableName { get; }

        public DBForeignKeyAttribute(string foreignTableName)
        {
            ForeignTableName = foreignTableName;
        }
    }
}
