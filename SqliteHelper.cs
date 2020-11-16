using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using static System.String;

namespace SqliteHelper {

    public class SqliteHandler {


        public readonly string DatabaseName;

        public SqliteHandler(string dbName) {
            if (dbName.EndsWith(".db")) {
                dbName = dbName.Substring(0, dbName.Length - 3);
            }
            DatabaseName = dbName;
        }

        void Connection(Action<SQLiteConnection> action) {
            using var conn = new SQLiteConnection($"Data Source={DatabaseName}.db; Version = 3; New = True; Compress = True;");
            conn.Open();
            action.Invoke(conn);
        }

        void Command(Action<SQLiteCommand> action, string text, params (string, object)[] paras) {
            Connection((con) => {
                using (var com = con.CreateCommand()) {
                    com.CommandType = CommandType.Text;
                    com.CommandText = text;
                    foreach (var i in paras) {
                        string pn = i.Item1;
                        if (!pn.Contains("@")) pn = $"@{pn}";
                        com.Parameters.AddWithValue(pn, i.Item2);
                    }
                    action.Invoke(com);
                }
            });
        }

        public void NonQuery(string nq, params (string, object)[] paras) {
            Command((com) => com.ExecuteNonQuery(), nq, paras);
        }

        public void Query(string q, Action<SQLiteDataReader> action, params (string, object)[] paras) {
            Command((com) => {
                var r = com.ExecuteReader();
                action.Invoke(r);
            }, q, paras);
        }

        public void QueryLoop(string q, Action<SQLiteDataReader> action, params (string, object)[] paras) {
            Query(q, (r) => {
                while (r.Read()) action.Invoke(r);
            }, paras);
        }

        public bool QueryHasRows(string q, params (string, object)[] paras) {
            bool a = false;
            Query(q, (r) => a = r.Read(), paras);
            return a;
        }

        public List<string> GetTables {
            get {
                var list = new List<string>();
                QueryLoop($"SELECT name FROM sqlite_master WHERE type='table';", (r) => {
                    list.Add(r.GetString(0));
                });
                return list;
            }
        }

        public List<(string name, string type, bool not_null, object default_value, bool pk)> GetColumns (string table) {
            var columns = new List<(string,string, bool, object, bool)>();
            QueryLoop($"pragma table_info([{table}]);", (r) => {
                var type = r.GetString(2);
                //type = type.Length == 0 ? "text" : type;
                columns.Add((r.GetString(1), type, r.GetBoolean(3), r.IsDBNull(4)? null: r.GetValue(4), r.GetBoolean(5)));
            });
            return columns;
        }

        public string GetTableCreationSql (string table) {
            string sql = null;
            QueryLoop($"select sql from sqlite_master where type = 'table' and name = @name; ", (r) => {
                sql = r.GetString(0);
            }, ("@name", table));
            return sql;
        }

        public string GetRows (IEnumerable<string> types, SQLiteDataReader reader, bool wrap_str = true) {
            var fields = new string[reader.FieldCount];
            string wrap = wrap_str ? "'" : "";
            for (var i = 0; i < fields.Length; i++) {
                var type = types.ElementAt(i);
                fields[i] = type switch
                {
                    "text" => $"{wrap}{reader.GetString(i)}{wrap}",
                    "integer" => reader.GetInt64(i).ToString(),
                    "read" => reader.GetDecimal(i).ToString("N50"),
                    _ => reader.GetValue(i).ToString()
                };
            }
            return String.Join(",", fields);
        }

    }

    public class SqliteColumn : Attribute {

        public bool Null = false;
        public object Default = null;

        public static implicit operator SqliteColumn(CustomAttributeData v) {
            var c = new SqliteColumn();
            if (v.AttributeType != typeof(SqliteColumn) && v.AttributeType != typeof(PrimaryKey)) {
                return null;
            }
            foreach (var arg in v.NamedArguments) {
                if (arg.MemberName == "Null") {
                    c.Null = (bool)arg.TypedValue.Value;
                } else if (arg.MemberName == "Default") {
                    c.Default = arg.TypedValue.Value;
                }
            }
            return c;
        }
    }

    public class PrimaryKey : SqliteColumn {

        public bool AutoIncrement { get; set; } = true;

        public static implicit operator PrimaryKey(CustomAttributeData v) {
            SqliteColumn c = v;
            if (c == null) {
                return null;
            }
            PrimaryKey pk = new PrimaryKey {
                Default = c.Default,
                Null = c.Null
            };
            foreach (var arg in v.NamedArguments) {
                if (arg.MemberName == "AutoIncrement") {
                    pk.AutoIncrement = (bool)arg.TypedValue.Value;
                }
            }
            return pk;
        }
    }

    public class Ignore : Attribute { }

    public class SqliteTable {

        public static SqliteHandler Handler;

    }

    /// <summary>
    /// Be sure to set SqliteTable.Handler before performing sql operations in this class.
    /// </summary>
    /// <typeparam name="T">A class that has fields marked with SqliteColumn, PrimaryKey or Ignored.</typeparam>
    public abstract class SqliteTable<T> : SqliteTable where T : new() {


        public virtual string TableName { get => GetType().Name; }

        /// <summary>
        /// <para>Perform an action before a row is inseted.</para>
        /// If the method returns false, then the row is not inserted.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual bool BeforeInsert() => true;

        public virtual void AfterInsert() { }

        /// <summary>
        /// <para>Performs an action before a row deletion.</para>
        /// If the method returns false, then the row is not deleted.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual bool BeforeDelete() => true;

        public virtual void AfterDelete() { }

        public virtual bool BeforeUpdate() => true;

        public virtual void AfterUpdate() { }

        public virtual void CreateTable() {
            var list = new List<(string, Type, SqliteColumn)>();
            var pks = new List<(string, bool)>();
            foreach (var f in GetType().GetFields()) {
                bool skip = false;
                PrimaryKey pk = null;
                SqliteColumn c = null;
                foreach (var a in f.CustomAttributes) {
                    if (a.AttributeType == typeof(Ignore)) {
                        skip = true;
                        break;
                    }
                    if (a.AttributeType == typeof(PrimaryKey)) {
                        pk = a;
                        c = pk;
                    } else if (a.AttributeType == typeof(SqliteColumn)) {
                        c = a;
                    }
                }
                if (!skip) {
                    if (c == null && pk == null) {
                        c = new SqliteColumn();
                        //throw new Exception("If a field has no 'Ignore' attribute, it must have 'SqliteColumn' or 'Primary key' attributes.");
                    }
                    if (c != null) {
                        list.Add((f.Name, f.FieldType, c));
                    }
                    if (pk != null) {
                        pks.Add((f.Name, pk.AutoIncrement));
                    }
                }
            }
            string nq;
            if (pks.Count == 1) {
                var cid = list.First(x => x.Item1 == pks[0].Item1);
                if (Data.IsInteger(cid.Item2) && pks[0].Item2) {
                    var cds2 = new List<string>();
                    foreach (var column in list) {
                        if (column.Item1 == pks[0].Item1) {
                            cds2.Add($"{column.Item1} {ToSqliteDataType(column.Item2)} primary key autoincrement");
                        } else {
                            cds2.Add(BuidColumnDefinition(column));
                        }
                    }
                    nq = $"create table if not exists {TableName} ({Join(",", cds2)});";
                    Handler.NonQuery(nq);
                    return;
                }
            }
            var cds = from column in list select BuidColumnDefinition(column);
            var pkc = "";
            if (pks.Count > 0) {
                pkc = $", primary key ({Join(",", pks.Select(x => x.Item1))})";
            }
            nq = $"create table if not exists {TableName} ({Join(",", cds)}{pkc});";
            Handler.NonQuery(nq);
        }

        public virtual void DropTable() {
            Handler.NonQuery($"drop table {TableName};");
        }

        static string BuidColumnDefinition((string, Type, SqliteColumn) a) {
            return $"{a.Item1} {ToSqliteDataType(a.Item2)} {ToSqliteDefault(a.Item3.Default)} {(a.Item3.Null ? "" : "not")} null";
        }

        static string ToSqliteDefault(object d) {
            if (d == null) {
                return "";
            }
            bool f(Type t) => d.GetType() == t;
            if (Data.Any(f, typeof(string))) {
                return $"default ('{d}')";
            }
            if (Data.Any(f, typeof(double), typeof(float))) {
                return $"default({((double)d).ToString("F15").TrimEnd('0')})";
            }
            if (Data.IsInteger(d.GetType())) {
                return $"default ({d})";
            }
            if (Data.Any(f, typeof(byte[]))) {
                return $"default ({((byte[])d).Aggregate("", (ac, x) => ac + x.ToString())})";
            }
            throw new ArgumentException("The type should be primitive.");
        }

        static string ToSqliteDataType(Type t) {
            bool f(Type a) => t == a;
            if (Data.Any(f, typeof(double), typeof(float))) {
                return "real";
            }
            if (Data.IsInteger(t)) {
                return "integer";
            }
            if (Data.Any(f, typeof(byte[]))) {
                return "blob";
            }
            return "text";
        }

        static List<FieldInfo> GetColumns(Func<FieldInfo, bool> filterF = null) {
            var l = new List<FieldInfo>();
            foreach (var field in typeof(T).GetFields().Where(x => x.GetCustomAttribute(typeof(Ignore)) == null)) {
                if (filterF != null && filterF.Invoke(field)) {
                    l.Add(field);
                }
            }
            return l;
        }

        static List<FieldInfo> GetPrimaryKeys() {
            var l = GetColumns(fi => fi.GetCustomAttribute(typeof(PrimaryKey)) != null);
            //var l = new List<FieldInfo>();
            //foreach (var field in t.GetFields()) {
            //    bool ignore = false, pk = false;
            //    foreach (var attribute in field.GetCustomAttributes()) {
            //        if (attribute.GetType() == typeof(Ignore)) {
            //            ignore = true;
            //        }
            //        if (attribute.GetType() == typeof(PrimaryKey)) {
            //            pk = true;
            //        }
            //    }
            //    if (!ignore && pk) {
            //        l.Add(field);
            //    }
            //}
            return l;
        }

        static List<FieldInfo> GetNonPrimaryKeys() {
            return GetColumns(fi => fi.GetCustomAttribute(typeof(PrimaryKey)) == null);
        }

        static List<string> SetColumns(List<FieldInfo> columns) {
            return columns.Select(x => $"{x.Name} = @{x.Name}").ToList();
        }

        static (string, object)[] GetParameters(object instance, IEnumerable<FieldInfo> columns) {
            return columns.Select(x => (x.Name, x.GetValue(instance))).ToArray();
        }

        public virtual void Insert() {
            var a = new List<(string, object)>();
            foreach (var field in GetType().GetFields()) {
                bool shouldIgnore = false;
                foreach (var attribute in field.CustomAttributes) {
                    if (attribute.GetType() == typeof(Ignore)) {
                        shouldIgnore = true;
                        break;
                    }
                }
                if (!shouldIgnore) {
                    if (FieldHasAI(field)) {
                        a.Add((field.Name, null));
                    } else {
                        a.Add((field.Name, field.GetValue(this)));
                    }
                }
            }
            var fns = from fn in a select fn.Item1;
            var pns = from fn in fns select $"@{fn}";
            string q = $"insert into {TableName} ({Join(",", fns)}) values ({Join(",", pns)});";
            Handler.NonQuery(q, a.ToArray());
        }

        static bool FieldHasAI(FieldInfo f) {
            foreach (var att in f.CustomAttributes) {
                PrimaryKey pk = att;
                if (pk != null) return pk.AutoIncrement;
            }
            return false;
        }

        public virtual T GetInstance(SQLiteDataReader r) {
            var fields = GetColumns();
            var t = new T();
            for (int i = 0; i < fields.Count; i++) {
                fields[i].SetValue(t, GetValue(r, fields[i], i));
            }
            return t;
        }

        public object GetValue(SQLiteDataReader r, FieldInfo field, int i) {
            var type = field.FieldType;
            if (type == typeof(double)) {
                return r.GetDouble(i);
            } else if (type == typeof(string)) {
                return r.GetString(i);
            } else if (type == typeof(int)) {
                return r.GetInt32(i);
            } else if (type == typeof(long)) {
                return r.GetInt64(i);
            } else if (type == typeof(byte[])) {
                var bytes = new byte[1];
                r.GetBytes(i, 0, bytes, 0, Int32.MaxValue);
                return bytes;
            } else {
                throw new Exception("The type is not mapped!.");
            }
        }

        public virtual void Delete() {
            var pks = GetPrimaryKeys();
            Handler.NonQuery($"delete from {TableName} where {Join(" and ", SetColumns(pks))};", GetParameters(this, pks));
        }

        public virtual void Update() {
            var pks = GetPrimaryKeys();
            var npks = GetNonPrimaryKeys();
            Handler.NonQuery($"update {TableName} set {Join(", ", SetColumns(npks))} where {Join(" and ", pks)};", GetParameters(this, pks.Concat(npks)));
        }

        public virtual List<T> Select(int limit = 1000) {
            var l = new List<T>();
            Handler.QueryLoop($"select * from {TableName};", (r) => {
                l.Add(GetInstance(r));
            });
            return l;
        }

    }

}