using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DBPrimaryKeyAttribute : Attribute
    {
        public DBPrimaryKeyAttribute()
        {

        }
    }
}
