using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHelper.Interfaces {

    interface IDBHelper {

        string Server { get; }
        string Database { get; }
        string User { get; }
        string Password { get; }
        string ConnectionString { get; }
        IEnumerable<string> GetTables { get; }
        (string, string) ColumnNameEscapePair { get; }
        Dictionary<Type, string> TypeTranslator { get; }
        object FilterValues(object t);
        Action<Exception> OnError { get; set; }
        void Connection(Action<SqlConnection> action);
        void Command(string q, Action<SqlCommand> action);
        void Command(Action<SqlCommand> action);
        void Command(SqlCommand command, Action<SqlCommand> action);
        void Reader(Action<SqlDataReader> action);
        void Reader(string q, Action<SqlDataReader> action);
        void Reader(SqlCommand coomand, Action<SqlDataReader> action);
        void Reader(SqlDataReader reader, Action<SqlDataReader> action);
        void ReaderLoop(string q, Action<SqlDataReader> action);
        void ReaderLoop(Action<SqlDataReader> action);
        void ReaderLoop(SqlDataReader reader, Action<SqlDataReader> action);
        void NonQuery(string q);
        void NonQuery(SqlCommand command);
        List<T> QueryLoop<T>(string q, Func<SqlDataReader, T> func);
        List<T> QueryLoop<T>(SqlCommand command, Func<SqlDataReader, T> func);
        List<T> SafeQueryLoop<T>(string q, Func<SqlDataReader, T> func, T @default = default);
        List<T> SafeQueryLoop<T>(SqlCommand command, Func<SqlDataReader, T> func, T @default = default);
        List<(string name, string type, bool nullable, int range)> GetColumns(string table);
        bool ReturnedRows(string q);
        bool ReturnedRows(SqlCommand command);

    }

}
