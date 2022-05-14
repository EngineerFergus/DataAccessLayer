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
        public int Number { get; }

        public DBColumnAttribute(bool isParentKey, string name, int number)
        {
            IsParentKey = isParentKey;
            Name = name;
            Number = number;
        }

        public DBColumnAttribute(string name, int number)
        {
            IsParentKey = false;
            Name = name;
            Number = number;
        }
    }
}
