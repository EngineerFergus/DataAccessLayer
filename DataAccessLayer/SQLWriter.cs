using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using DataAccessLayer.Attributes;

namespace DataAccessLayer
{
    public static class SQLWriter
    {
        public static string WriteCreateString(Type type)
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(type);
            PropertyInfo[] fields = type.GetProperties();

            return "Create";
        }

        public static string WriteReadString(Type type)
        {
            return "Read";
        }

        public static string WriteUpdateString(Type type)
        {
            return "Update";
        }

        public static string WriteInsertString(Type type)
        {
            return "Insert";
        }

        public static string WriteDeleteString(Type type)
        {
            return "Delete";
        }
    }
}
