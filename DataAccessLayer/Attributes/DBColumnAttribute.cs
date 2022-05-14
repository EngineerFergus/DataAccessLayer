using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DBColumnAttribute : Attribute
    {
        public bool IsPrimaryKey { get; }
        public bool IsParentKey { get; }
        public string Name { get; }

        public DBColumnAttribute(bool isPrimaryKey, bool isParentKey, string name)
        {
            if(isPrimaryKey && isParentKey)
            {
                throw new Exception($"Exception in {nameof(DBColumnAttribute)}, column cannot be both primary key and parent key.");
            }

            IsPrimaryKey = isPrimaryKey;
            IsParentKey = isParentKey;
            Name = name;
        }

        public DBColumnAttribute(string name)
        {
            IsParentKey = false;
            IsPrimaryKey = false;
            Name = name;
        }
    }
}
