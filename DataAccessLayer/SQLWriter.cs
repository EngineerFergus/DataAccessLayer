using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DataAccessLayer
{
    public static class SQLWriter
    {
        public static string WriteCreateString(Type type)
        {
            DBTableAttribute dBTableAttribute = SearchForDBTable(type);

            PropertyInfo[] properties = type.GetProperties();
            DBPrimaryKeyAttribute dBPrimaryKeyAttribute = SearchForPrimaryKey(properties, type);
            List<Tuple<Type, DBColumnAttribute>> columnTypes = SearchForDBColumns(properties, type);

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
                else if(objType == typeof(long))
                {
                    sb.Append($"{columnType.Item2.Name} INTEGER NOT NULL");
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


            bool hasForeignKey = TrySearchForForeignKey(properties, type, out DBForeignKeyAttribute? key,
                out DBColumnAttribute? column);

            if (hasForeignKey)
            {
                if (column is null || key is null)
                {
                    throw new Exception();
                }

                sb.Append($", FOREIGN KEY({column.Name}) REFERENCES {key.ForeignTableName}(ID)");
            }

            sb.Append(')');

            return sb.ToString();
        }

        public static string WriteReadString(Type type)
        {
            DBTableAttribute dBTableAttribute = SearchForDBTable(type);
            PropertyInfo[] properties = type.GetProperties();
            DBPrimaryKeyAttribute primaryKey = SearchForPrimaryKey(properties, type);
            List<Tuple<Type, DBColumnAttribute>> columns = SearchForDBColumns(properties, type).OrderBy(x => x.Item2.Number).ToList();

            StringBuilder read = new StringBuilder();
            read.Append("SELECT ");

            for(int i = 0; i < columns.Count; i++)
            {
                read.Append($"{columns[i].Item2.Name}");

                if(i < columns.Count - 1)
                {
                    read.Append(", ");
                }
            }

            read.Append($" FROM {dBTableAttribute.Name}");

            return read.ToString();
        }

        public static string WriteReadByIDString(Type type)
        {
            string read = WriteReadString(type);
            return $"{read} WHERE ID = {{0}}";
        }

        public static string WriteUpdateString(Type type)
        {
            DBTableAttribute dBTableAttribute = SearchForDBTable(type);
            PropertyInfo[] properties = type.GetProperties();

            DBPrimaryKeyAttribute primaryKey = SearchForPrimaryKey(properties, type);
            List<Tuple<Type, DBColumnAttribute>> columns = SearchForDBColumns(properties, type).OrderBy(x => x.Item2.Number).ToList();

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

            DBPrimaryKeyAttribute primaryKey = SearchForPrimaryKey(properties, type);
            List<Tuple<Type, DBColumnAttribute>> columnTypes = SearchForDBColumns(properties, type);
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
            DBPrimaryKeyAttribute primaryKey = SearchForPrimaryKey(properties, type);

            return $"DELETE FROM {dBTableAttribute.Name} WHERE ID = {{0}}";
        }

        private static DBTableAttribute SearchForDBTable(Type type)
        {
            DBTableAttribute? dBTableAttribute = type.GetCustomAttribute<DBTableAttribute>();

            if (dBTableAttribute is null)
            {
                throw new Exception($"Exception in {nameof(DataAccessLayer)}.{nameof(WriteCreateString)}, " +
                    $"provided type {type.FullName} did not contain {nameof(DBTableAttribute)}.");
            }

            return dBTableAttribute;
        }

        private static DBPrimaryKeyAttribute SearchForPrimaryKey(PropertyInfo[] properties, Type type)
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
                throw new Exception($"Exception in {nameof(DataAccessLayer)}.{nameof(SQLWriter)}, " +
                    $"provided type {type.FullName} did not contain a {nameof(DBPrimaryKeyAttribute)}.");
            }

            return dBPrimaryKeyAttribute;
        }

        private static List<Tuple<Type, DBColumnAttribute>> SearchForDBColumns(PropertyInfo[] properties, Type type)
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
                throw new Exception($"Exception in {nameof(DataAccessLayer)}.{nameof(SQLWriter)}, " +
                    $"provided type {type.FullName} did not contain any properties with {nameof(DBColumnAttribute)}.");
            }

            return columnTypes;
        }

        private static bool TrySearchForForeignKey(PropertyInfo[] properties, Type type, out DBForeignKeyAttribute? key,
            out DBColumnAttribute? column)
        {
            key = null;
            column = null;
            bool foundKey = false;
            int count = 0;

            foreach(PropertyInfo property in properties)
            {
                DBForeignKeyAttribute? k = property.GetCustomAttribute<DBForeignKeyAttribute>();

                if(k != null)
                {
                    count++;
                    if(count > 1)
                    {
                        throw new Exception($"Exception in {nameof(DataAccessLayer)}.{nameof(SQLWriter)}, " +
                            $"more than one {nameof(DBForeignKeyAttribute)} found on provided type {type.FullName}.");
                    }

                    foundKey = true;
                    key = k;

                    DBColumnAttribute? c = property.GetCustomAttribute<DBColumnAttribute>();

                    if(c is null)
                    {
                        throw new Exception($"Exception in {nameof(DataAccessLayer)}.{nameof(SQLWriter)}, " +
                            $"{nameof(DBForeignKeyAttribute)} was found on a property that did not have a " +
                            $"{nameof(DBColumnAttribute)}");
                    }

                    column = c;
                }

            }

            return foundKey;
        }

        private static bool IsString(Type type)
        {
            Type objType = type;
            objType = Nullable.GetUnderlyingType(objType) ?? objType;
            return objType == typeof(string);
        }
    }
}
