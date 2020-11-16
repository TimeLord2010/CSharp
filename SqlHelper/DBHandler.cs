using SqlHelper.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelper {

    public abstract class DBHandler : IDBHelper {

        public DBHandler(string server, string user, string password, string database = null) {
            Server = server;
            Database = database;
            User = user;
            Password = password;
        }

        public string Server { get; }

        public string Database { get; set; }

        public string User { get; }

        public string Password { get; }

        public abstract string ConnectionString { get; }

        public abstract IEnumerable<string> GetTables { get; }

        public Action<Exception> OnError { get; set; }

        public abstract (string, string) ColumnNameEscapePair { get; }

        public abstract Dictionary<Type, string> TypeTranslator { get; }

        public void Command(Action<SqlCommand> action) {
            Connection((connection) => {
                using (var command = new SqlCommand("", connection)) {
                    action.Invoke(command);
                }
            });
        }

        public void Command(SqlCommand command, Action<SqlCommand> action) {
            Connection((connection) => {
                using (command) {
                    command.Connection = connection;
                    action.Invoke(command);
                }
            });
        }

        public void Command(string q, Action<SqlCommand> action) {
            Command(new SqlCommand(q), action);
        }

        public void Connection(Action<SqlConnection> action) {
            using (var connection = new SqlConnection(ConnectionString)) {
                connection.Open();
                action.Invoke(connection);
            }
        }

        public abstract List<(string name, string type, bool nullable, int range)> GetColumns(string table);

        public void NonQuery(string q) {
            Command((command) => {
                command.CommandText = q;
                command.ExecuteNonQuery();
            });
        }

        public void NonQuery(SqlCommand command) {
            Connection((connection) => {
                using (command) {
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                }
            });
        }

        public List<T> QueryLoop<T>(string q, Func<SqlDataReader, T> func) {
            return QueryLoop(new SqlCommand(q), func);
        }

        public List<T> QueryLoop<T>(SqlCommand command, Func<SqlDataReader, T> func) {
            var list = new List<T>();
            Command(command, (co) => {
                ReaderLoop(co.ExecuteReader(), (r) => {
                    list.Add(func.Invoke(r));
                });
            });
            return list;
        }

        public void Reader(Action<SqlDataReader> action) {
            Command((command) => {
                using (var reader = command.ExecuteReader()) {
                    action.Invoke(reader);
                }
            });
        }

        public void Reader(SqlDataReader reader, Action<SqlDataReader> action) {
            Command((command) => {
                using (reader) {
                    action.Invoke(reader);
                }
            });
        }

        public void Reader(SqlCommand command, Action<SqlDataReader> action) {
            Command(command, (c) => {
                using (var reader = command.ExecuteReader()) {
                    action.Invoke(reader);
                }
            });
        }

        public void Reader(string q, Action<SqlDataReader> action) {
            Reader(new SqlCommand(q), action);
        }

        void ReaderLoopUse(SqlDataReader reader, Action<SqlDataReader> action) {
            while (reader.Read()) {
                action.Invoke(reader);
            }
        }

        public void ReaderLoop(Action<SqlDataReader> action) {
            Reader(r => ReaderLoopUse(r, action));
        }

        public void ReaderLoop(SqlDataReader reader, Action<SqlDataReader> action) {
            Reader(reader, r => ReaderLoopUse(r, action));
        }

        public void ReaderLoop(string q, Action<SqlDataReader> action) {
            Reader(q, r => ReaderLoopUse(r, action));
        }

        public bool ReturnedRows(string q) {
            return ReturnedRows(new SqlCommand(q));
        }

        public bool ReturnedRows(SqlCommand command) {
            bool a = false;
            Reader(command, (r) => {
                a = r.Read();
            });
            return a;
        }

        public List<T> SafeQueryLoop<T>(string q, Func<SqlDataReader, T> func, T @default = default) {
            try {
                return QueryLoop(q, (r) => {
                    try {
                        return func.Invoke(r);
                    } catch (Exception ex1) {
                        OnError(ex1);
                        return @default;
                    }
                });
            } catch (Exception ex) {
                OnError(ex);
                return null;
            }
        }

        public List<T> SafeQueryLoop<T>(SqlCommand command, Func<SqlDataReader, T> func, T @default = default) {
            try {
                return QueryLoop(command, (r) => {
                    try {
                        return func.Invoke(r);
                    } catch (Exception ex1) {
                        OnError(ex1);
                        return @default;
                    }
                });
            } catch (Exception ex) {
                OnError(ex);
                return null;
            }
        }

        public abstract object FilterValues(object t);
    }

}
