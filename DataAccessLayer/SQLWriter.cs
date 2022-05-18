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
            DBTableAttribute dBTableAttribute = SearchForDBTable(type);

            PropertyInfo[] properties = type.GetProperties();
            DBPrimaryKeyAttribute dBPrimaryKeyAttribute = SearchForPrimaryKey(properties);
            List<Tuple<Type, DBColumnAttribute>> columnTypes = SearchForDBColumns(properties);

            columnTypes = columnTypes.OrderBy(x => x.Item2.Number).ToList();

            StringBuilder sb = new();

            sb.Append($"CREATE TABLE IF NOT EXISTS {dBTableAttribute.Name} (ID INTEGER PRIMARY KEY NOT NULL, ");

            for(int i = 0; i < columnTypes.Count; i++)
            {
                var columnType = columnTypes[i];
                Type objType = columnType.Item1;
                objType = Nullable.GetUnderlyingType(objType) ?? objType;
                bool isValidType = false;

                if(objType == typeof(int))
                {
                    sb.Append($"{columnType.Item2.Name} INT NOT NULL");
                    isValidType = true;
                }
                else if(objType == typeof(string))
                {
                    sb.Append($"{columnType.Item2.Name} STRING NOT NULL");
                    isValidType = true;
                }
                else if(objType == typeof(double))
                {
                    sb.Append($"{columnType.Item2.Name} DOUBLE NOT NULL");
                    isValidType = true;
                }

                if (i < columnTypes.Count - 1 && isValidType)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(')');

            return sb.ToString();
        }

        public static string WriteReadString(Type type)
        {
            DBTableAttribute dBTableAttribute = SearchForDBTable(type);

            return $"SELECT * FROM {dBTableAttribute.Name}";
        }

        public static string WriteUpdateString(Type type)
        {
            DBTableAttribute dBTableAttribute = SearchForDBTable(type);
            PropertyInfo[] properties = type.GetProperties();

            DBPrimaryKeyAttribute primaryKey = SearchForPrimaryKey(properties);
            List<Tuple<Type, DBColumnAttribute>> columns = SearchForDBColumns(properties).OrderBy(x => x.Item2.Number).ToList();

            StringBuilder sb = new StringBuilder();
            sb.Append($"UPDATE {dBTableAttribute.Name} SET ");

            for(int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                Type objType = column.Item1;
                objType = Nullable.GetUnderlyingType(objType) ?? objType;
                bool isString = objType == typeof(string);

                if (isString)
                {
                    sb.Append($"{column.Item2.Name} = '{{{i + 1}}}'");
                }
                else
                {
                    sb.Append($"{column.Item2.Name} = {{{i + 1}}}");
                }

                if (i < columns.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(" WHERE ID = {0}");

            return sb.ToString();
        }

        public static string WriteInsertString(Type type)
        {
            DBTableAttribute dBTable = SearchForDBTable(type);
            PropertyInfo[] properties = type.GetProperties();

            DBPrimaryKeyAttribute primaryKey = SearchForPrimaryKey(properties);
            List<Tuple<Type, DBColumnAttribute>> columnTypes = SearchForDBColumns(properties);
            columnTypes = columnTypes.OrderBy(x => x.Item2.Number).ToList();

            StringBuilder fullInsert = new StringBuilder();
            StringBuilder tableNames = new StringBuilder();
            StringBuilder formatters = new StringBuilder();

            fullInsert.Append($"INSERT INTO {dBTable.Name} (");

            for(int i = 0; i < columnTypes.Count; i++)
            {
                Type columnType = columnTypes[i].Item1;
                DBColumnAttribute columnAttribute = columnTypes[i].Item2;

                tableNames.Append($"{columnAttribute.Name}");

                if (IsString(columnType))
                {
                    formatters.Append($"'{{{i}}}'");
                }
                else
                {
                    formatters.Append($"{{{i}}}");
                }

                if(i < columnTypes.Count - 1)
                {
                    tableNames.Append(", ");
                    formatters.Append(", ");
                }
            }

            fullInsert.Append(tableNames.ToString());
            fullInsert.Append(") VALUES (");
            fullInsert.Append(formatters.ToString());
            fullInsert.Append(')');

            return fullInsert.ToString();
        }

        public static string WriteDeleteString(Type type)
        {
            DBTableAttribute dBTableAttribute = SearchForDBTable(type);
            PropertyInfo[] properties = type.GetProperties();
            DBPrimaryKeyAttribute primaryKey = SearchForPrimaryKey(properties);

            return $"DELETE FROM {dBTableAttribute.Name} WHERE ID = {{0}}";
        }

        private static DBTableAttribute SearchForDBTable(Type type)
        {
            DBTableAttribute? dBTableAttribute = type.GetCustomAttribute<DBTableAttribute>();

            if (dBTableAttribute is null)
            {
                throw new Exception($"Exception in {nameof(WriteCreateString)}, provided type did not contain {nameof(DBTableAttribute)}.");
            }

            return dBTableAttribute;
        }

        private static DBPrimaryKeyAttribute SearchForPrimaryKey(PropertyInfo[] properties)
        {
            DBPrimaryKeyAttribute? dBPrimaryKeyAttribute = null;

            foreach(PropertyInfo property in properties)
            {
                if (dBPrimaryKeyAttribute is null)
                {
                    dBPrimaryKeyAttribute = property.GetCustomAttribute<DBPrimaryKeyAttribute>();
                }
            }

            if(dBPrimaryKeyAttribute is null)
            {
                throw new Exception($"Exception in {nameof(SQLWriter)}, provided type did not contain a " +
                    $"primary key attribute.");
            }

            return dBPrimaryKeyAttribute;
        }

        private static List<Tuple<Type, DBColumnAttribute>> SearchForDBColumns(PropertyInfo[] properties)
        {
            List<Tuple<Type, DBColumnAttribute>> columnTypes = new List<Tuple<Type, DBColumnAttribute>>();

            foreach (PropertyInfo property in properties)
            {
                DBColumnAttribute? attribute = property.GetCustomAttribute<DBColumnAttribute>();
                if (attribute != null)
                {
                    columnTypes.Add(new Tuple<Type, DBColumnAttribute>(property.PropertyType, attribute));
                }
            }

            if (columnTypes.Count == 0)
            {
                throw new Exception($"Exception in {nameof(WriteCreateString)}, provided type did not contain any columns.");
            }

            return columnTypes;
        }

        private static bool IsString(Type type)
        {
            Type objType = type;
            objType = Nullable.GetUnderlyingType(objType) ?? objType;
            return objType == typeof(string);
        }
    }
}
