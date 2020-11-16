using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelper {

    public class SqlServerHelper : DBHandler {

        public SqlServerHelper(string server, string user, string password, string database = null) : base(server, user, password, database) {
        }

        public override string ConnectionString {
            get {
                var db = Database == null ? "" : $"Database={Database};";
                return $"Server={Server};{db}User ID={User};Password={Password};";
            }
        }

        public override IEnumerable<string> GetTables {
            get {
                return QueryLoop($"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_CATALOG = '{Database}'", (r) => {
                    return r.GetString(0);
                });
            }
        }

        public override (string, string) ColumnNameEscapePair { get => ("[", "]"); }

        public override Dictionary<Type, string> TypeTranslator {
            get => new Dictionary<Type, string>() {
                {typeof(string) , "nvarchar(max)" },
                {typeof(byte), "tinyint not" },
                {typeof(byte?), "tinyint" },
                {typeof(short), "smallint not" },
                {typeof(short?), "smallint" },
                {typeof(int), "int not" },
                {typeof(int?), "int" },
                {typeof(long), "bigint not" },
                {typeof(long?), "bigint" },
                {typeof(float), "float(24) not" },
                {typeof(float?), "float(24)" },
                {typeof(double), "float(53) not" },
                {typeof(double?), "float(53)" },
                {typeof(decimal), "decimal(18,6) not" },
                {typeof(decimal?), "decimal(18,6)" },
                {typeof(DateTime), "Datetime not" },
                {typeof(DateTime?), "Datetime" },
                {typeof(TimeSpan), "Time not" },
                {typeof(TimeSpan?), "Time" },
                {typeof(byte[]), "varbinary(max)" }
            };
        }

        public override object FilterValues (object t) {
            if (t is DateTime dt) {
                if (dt < new DateTime(1973,1,1)) {
                    return new DateTime(1973,1,1);
                }
            }
            return t;
        }

        public override List<(string name, string type ,bool nullable, int range)> GetColumns(string table) {
            return SafeQueryLoop($"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{table}'", (r) => {
                return (r.GetString(0), r.GetString(1), false, 0);
            });
        }
    }

}
