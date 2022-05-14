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
        public bool IsParentKey { get; }
        public string Name { get; }

        public DBColumnAttribute(bool isParentKey, string name)
        {
            IsParentKey = isParentKey;
            Name = name;
        }

        public DBColumnAttribute(string name)
        {
            IsParentKey = false;
            Name = name;
        }
    }
}
