using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DBTableAttribute : Attribute
    {
        public string Name { get; }

        public DBTableAttribute(string name)
        {
            Name = name;
        }
    }
}
