using Exceptions;
using System;
using System.Data.SqlClient;
using System.Linq;
using static System.Convert;

// SqlServer 

public class DB {

    public DB(string server, string db, string user, string password) {
        Server = server;
        Database = db;
        User = user;
        Password = password;
    }

    public DB(string server, string user, string password) {
        Server = server;
        User = user;
        Password = password;
    }

    public string Server, Database, User, Password;

    public string CS {
        get => $"Server={Server};Database={Database};User ID={User};Password={Password};";
    }

    Action<Exception> onError;
    public Action<Exception> OnError {
        get {
            if (onError != null) {
                return onError;
            } else {
                return (ex) => {
                    throw ex;
                };
            }
        }
        set => onError = value;
    }

    Action<string> onDebug;
    public Action<string> OnDebug {
        get {
            if (onDebug != null) {
                return onDebug;
            } else {
                return (_) => { };
            }
        }
        set {
            onDebug = value;
        }
    }

    public void Connection(Action<SqlConnection> action) {
        using var con = new SqlConnection(CS);
        con.Open();
        action(con);
    }

    public void Command(Action<SqlCommand> action, string stm) {
        try {
            Connection((con) => {
                using var com = new SqlCommand(stm, con);
                action.Invoke(com);
            });
        } catch (SqlException ex) {
            throw new DatabaseException(stm, ex, $"Database: {Database}.");
        }
    }

    public void Command(SqlCommand command, Action<SqlCommand> action) {
        try {
            using (command) {
                Connection((connection) => {
                    command.Connection = connection;
                    action.Invoke(command);
                });
            }
        } catch (SqlException ex) {
            throw new DatabaseException(command.CommandText, ex, $"Database: {Database}");
        }
    }

    public void Reader(Action<SqlDataReader> action, string stm) {
        Command((com) => {
            using var r = com.ExecuteReader();
            action.Invoke(r);
        }, stm);
    }

    public void Reader(Action<SqlDataReader> action, SqlCommand command) {
        Connection((con) => {
            using (command) {
                command.Connection = con;
                using var reader = command.ExecuteReader();
                action(reader);
            }
        });
    }

    public void ReaderLoop(Action<SqlDataReader> action, string stm) {
        Reader((r) => {
            while (r.Read()) {
                action.Invoke(r);
            }
        }, stm);
    }

    public void ReaderLoop(Action<SqlDataReader> action, SqlCommand com) {
        Command(com, (c) => {
            var reader = c.ExecuteReader();
            while (reader.Read()) {
                action(reader);
            }
        });
    }

    public void NonQuery(string nq, Action<SqlCommand> action = null) {
        Command((com) => {
            if (action != null) action.Invoke(com);
            com.ExecuteNonQuery();
        }, nq);
    }

    public void NonQuery(SqlCommand co) {
        Command(co, (com) => {
            com.ExecuteNonQuery();
        });
    }

    public T SafeQuery<T>(Func<T> func) {
        try {
            return func.Invoke();
        } catch (Exception ex) {
            OnError.Invoke(ex);
        }
        return default;
    }

    public T SafeQuery<T>(Func<SqlCommand, T> func, string stm, T def = default) {
        return SafeQuery(() => {
            Command((com) => {
                def = func(com);
            }, stm);
            return def;
        });
    }

    public T SafeQuery<T>(Func<SqlDataReader, T> func, object query_or_command, T def = default) {
        T toT(SqlCommand c) {
            var reader = c.ExecuteReader();
            return reader.Read() ? func(reader) : def;
        };
        if (query_or_command is SqlCommand command) {
            Command(command,(com) => {
                def = toT(command);
            });
            return def;
        } else if (query_or_command is string q) {
            return SafeQuery(toT, q);
        } else {
            throw new ArgumentException($"Object 'query_or_command' should be string or SqlCommand. Received: {query_or_command.GetType()}.");
        }
    }

    public MyList<T> SafeQueryLoop<T>(string q, Func<SqlDataReader, T> reader, T def = default(T)) {
        return SafeQuery(() => {
            var list = new MyList<T>();
            Command((com) => {
                using var r = com.ExecuteReader();
                while (r.Read()) {
                    list.Add(reader(r));
                }
            }, q);
            return list;
        });
    }

    public MyList<T> SafeQueryLoop<T>(SqlCommand com, Func<SqlDataReader, T> reader) {
        return SafeQuery(() => {
            var list = new MyList<T>();
            Connection((con) => {
                using (com) {
                    com.Connection = con;
                    using var r = com.ExecuteReader();
                    while (r.Read()) {
                        list.Add(reader.Invoke(r));
                    }
                }
            });
            return list;
        }) ?? new MyList<T>();
    }

    public MyList<(string name, string type, bool nullable)> GetColumns(string table) {
        return SafeQueryLoop($"SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{table}'", (r) => {
            return (r.GetString(0), r.GetString(1), r.GetString(2).ToLower() == "yes");
        });
    }

    public bool IsColumnNullable(string table, string column) {
        return SafeQueryLoop($"select  is_nullable from sys.columns where object_id = object_id('{table}') and name = '{column}'", (r) => {
            return ToInt32(r.GetValue(0)) == 1;
        }).FirstOr();
    }

    public bool ContainsAllColumns(string table, params string[] columns) {
        var current_columns = GetColumns(table);
        return FuncUtils.ContainsAll(current_columns.Select(x => x.name), columns);
    }

    public string InsertFieldsString(string table, params string[] fields) {
        var columns = GetColumns(table);
        var c = columns.Select((x, i) => {
            if (x.type.Contains("char")) {
                return $"'{fields[i]}'";
            } else {
                return fields[i];
            }
        });
        return $"({string.Join(",", c)})";
    }

    public static int? GetInt32(SqlDataReader r, int n, int? @default = null) {
        return r.IsDBNull(n) ? @default : ToInt32(r.GetValue(n));
    }

    public static bool? GetBool(SqlDataReader r, int n, bool? @default = null) {
        return r.IsDBNull(n) ? @default : r.GetBoolean(n);
    }

    public static byte? GetByte(SqlDataReader r, int n, byte? @default = null) {
        return r.IsDBNull(n) ? @default : r.GetByte(n);
    }

    public static string GetString(SqlDataReader r, int n, string @default = null) {
        return r.IsDBNull(n) ? @default : r.GetString(n);
    }

    public static DateTime? GetDT(SqlDataReader r, int n, DateTime? @default = default) {
        //if (@default == null) @default = DateTime.MinValue;
        return r.IsDBNull(n) ? @default : r.GetDateTime(n);
    }

    public static decimal? GetDecimal(SqlDataReader r, int n, decimal? @default = default) {
        return r.IsDBNull(n) ? @default : r.GetDecimal(n);
    }

    public static string GetValue(object a) {
        if (a is null) return "null";
        if (a is string s) return $"'{s}'";
        if (a is DateTime dt) return $"CAST(N'{dt.Year}-{dt.Month}-{dt.Day} {dt.Hour}:{dt.Minute}:{dt.Second}.000' AS DateTime)";
        return a.ToString();
    }

    public bool ReturnedRows(SqlCommand command) {
        bool has_rows = false;
        Reader((r) => {
            has_rows = r.Read();
        }, command);
        return has_rows;
    }

    public MyList<string> GetTables {
        get {
            return SafeQueryLoop($"select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'BASE TABLE'", (r) => r.GetString(0), null);
        }
    }

    public bool IsColumnIdentity(string table, string column) {
        int? a = null;
        ReaderLoop((r) => {
            a = r.GetInt32(0);
        }, $"select columnproperty(object_id('{table}'),'{column}','IsIdentity')");
        if (!a.HasValue) throw new Exception($"Couldn't check if {table}.{column} was identity.");
        return a == 1;
    }

    public bool TableExists(string table, string schema = "dbo") {
        var c = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @schema AND TABLE_NAME = @tablename;");
        c.Parameters.AddWithValue("tablename", table);
        c.Parameters.AddWithValue("schema", schema);
        bool exists = false;
        ReaderLoop((r) => {
            exists = true;
        }, c);
        return exists;
    }

}