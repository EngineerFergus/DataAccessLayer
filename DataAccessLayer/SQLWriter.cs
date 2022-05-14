﻿using System;
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
        public const string CREATE = "CREATE TABLE IF NOT EXISTS";

        public static string WriteCreateString(Type type)
        {
            DBTableAttribute? dBTableAttribute = type.GetCustomAttribute<DBTableAttribute>();

            if(dBTableAttribute is null)
            {
                throw new Exception($"Excpetion in {nameof(WriteCreateString)}, provided type did not contain {nameof(DBTableAttribute)}.");
            }

            DBPrimaryKeyAttribute? primaryKey = null;

            PropertyInfo[] properties = type.GetProperties();

            List<Tuple<Type, DBColumnAttribute>> columnTypes = new List<Tuple<Type, DBColumnAttribute>>();

            foreach(PropertyInfo property in properties)
            {
                DBColumnAttribute? attribute = property.GetCustomAttribute<DBColumnAttribute>();
                if (attribute != null)
                {
                    columnTypes.Add(new Tuple<Type, DBColumnAttribute>(property.PropertyType, attribute));
                }

                if(primaryKey is null)
                {
                    primaryKey = property.GetCustomAttribute<DBPrimaryKeyAttribute>();
                }
            }

            if (columnTypes.Count == 0)
            {
                throw new Exception($"Exception in {nameof(WriteCreateString)}, provided type did not contain any columns.");
            }

            if(primaryKey == null)
            {
                throw new Exception($"Exception in {nameof(WriteCreateString)}, provided type did not contain a primary key.");
            }

            columnTypes = columnTypes.OrderBy(x => x.Item2.Number).ToList();

            StringBuilder sb = new StringBuilder();

            sb.Append($"CREATE TABLE IF NOT EXISTS {dBTableAttribute.Name} (ID INTEGER PRIMARY KEY NOT NULL, ");

            for(int i = 0; i < columnTypes.Count; i++)
            {
                var columnType = columnTypes[i];

                if(columnType.Item1.GetType() == typeof(int))
                {
                    sb.Append($"{columnType.Item2.Name} INT NOT NULL");
                }
                else if(columnType.Item1.GetType() == typeof(string))
                {
                    sb.Append($"{columnType.Item2.Name} STRING NOT NULL");
                }
                else if(columnType.Item1.GetType() == typeof(double))
                {
                    sb.Append($"{columnType.Item2.Name} DOUBLE NOT NULL");
                }

                if(i < columnTypes.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(')');

            return sb.ToString();
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
