using SqlHelper.Interfaces;
using SqlHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.String;
using static System.Convert;

namespace SqlHelper {

    class SqlTable {

        public static IDBHelper DBHelper;

    }

    class SqlTable<T> : SqlTable where T : class, new() {

        public string Name { get => GetType().Name; }
        string LeftColumnNameEscape {
            get => DBHelper.ColumnNameEscapePair.Item1;
        }
        string RightColumnNameEscape {
            get => DBHelper.ColumnNameEscapePair.Item2;
        }

        Func<SqlDataReader, T> getter;
        public Func<SqlDataReader, T> Getter {
            get {
                if (getter != null) return getter;
                getter = (r) => {
                    var t = new T();
                    for (var i = 0; i < VisibleFields.Count(); i++) {
                        var field = VisibleFields.ElementAt(i);
                        if (r.IsDBNull(i)) continue;
                        field.SetValue(t, r.GetValue(i)); 
                    }
                    return t;
                };
                return getter;
            }
            set => getter = value;
        }

        IEnumerable<FieldInfo> VisibleFields {
            get => GetType().GetFields().Where(x => !x.CustomAttributes.Any(y => y.AttributeType == typeof(Ignore)));
        }

        IEnumerable<FieldInfo> PrimaryKeys {
            get => VisibleFields.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(PrimaryKey)));
        }

        IEnumerable<FieldInfo> InsertableFields {
            get => VisibleFields.Where(x => !x.CustomAttributes.Any(y => y.AttributeType == typeof(Identity)) && x.GetValue(this) != null);
        }

        string PrimaryKeyWhere {
            get {
                if (PrimaryKeys.Count() == 0) throw new Exception("Impossible to produce 'where' clause because the is no primary key in the table.");
                var pks = PrimaryKeys.Select(x => $"{LeftColumnNameEscape}{x.Name}{RightColumnNameEscape} = @{x.Name}");
                return $" where {Join(" and ", pks)}";
            }
        }

        void FilterValues() {
            foreach (var field in InsertableFields) {
                field.SetValue(this, DBHelper.FilterValues(field.GetValue(this)));
            }
        }

        void AddParameters(SqlCommand command, IEnumerable<FieldInfo> fields = null) {
            fields = fields ?? VisibleFields;
            foreach (var field in fields) {
                command.Parameters.AddWithValue($"{field.Name}", field.GetValue(this));
            }
        }

        SqlCommand CreateCommand(string q, IEnumerable<FieldInfo> fields = null) {
            var command = new SqlCommand(q);
            AddParameters(command, fields);
            return command;
        }

        void NonQuery(string q, IEnumerable<FieldInfo> fields = null) {
            DBHelper.NonQuery(CreateCommand(q, fields));
        }

        public void CreateTable() {
            string identity_definition(FieldInfo fi) =>
                fi.GetCustomAttributes(true).FirstOrDefault(x => x is Identity) is Identity identity ? $"identity({identity.Seed},{identity.Increment})" : "";
            var definitions = VisibleFields
                .Select(x => $"{LeftColumnNameEscape}{x.Name}{RightColumnNameEscape} {DBHelper.TypeTranslator[x.FieldType]} null {identity_definition(x)}")
                .ToList();
            var primarykeys = PrimaryKeys.Select(x => $"{LeftColumnNameEscape}{x.Name}{RightColumnNameEscape}");
            if (primarykeys.Count() > 0) definitions.Add($"primary key ({Join(", ", primarykeys)})");
            string q = $"create table {Name} (\n\t{Join(",\n\t", definitions)}\n);";
            DBHelper.NonQuery(q);
        }

        public void DropTable() => DBHelper.NonQuery($"drop table {Name};");

        public void Insert() {
            FilterValues();
            //var valid_fields = 
            var fields = InsertableFields.Select(x => $"{LeftColumnNameEscape}{x.Name}{RightColumnNameEscape}");
            var values = InsertableFields.Select(x => $"@{x.Name}");
            NonQuery($"insert into {Name} ({Join(",", fields)}) values ({Join(",", values)});", InsertableFields);
        }

        public void Delete() {
            NonQuery($"delete from {Name} {PrimaryKeyWhere}", PrimaryKeys);
        }

        public void Update() {
            var new_values = InsertableFields.Select(x => $"{LeftColumnNameEscape}{x.Name}{RightColumnNameEscape} = @{x.Name}");
            var parameters = InsertableFields.ToList();
            parameters.AddRange(PrimaryKeys);
            NonQuery($"update {Name} set {Join(",", new_values)} where {PrimaryKeyWhere}", parameters);
        }

        public List<T> Select(int limit = 1000) {
            return DBHelper.QueryLoop($"select top {limit} * from {Name};", (r) => Getter(r));
        }

        public T Refresh() {
            return DBHelper.QueryLoop(CreateCommand($"select * from {Name} {PrimaryKeyWhere}", PrimaryKeys), (r) => Getter(r)).FirstOrDefault();
        }

        public override string ToString() {
            var a = $"{Name}:\n";
            foreach (var field in VisibleFields) {
                a += $"\t{field.Name}: {field.GetValue(this)}\n";
            }
            return a;
        }

    }

}
