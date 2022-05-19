using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DBColumnAttribute : Attribute
    {
        public string Name { get; }
        public int Number { get; }

        public DBColumnAttribute(string name, int number)
        {
            Name = name;
            Number = number;
        }
    }
}
