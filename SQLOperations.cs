using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

public static class SQLOperations {

    public static MySqlH Sql;
    public static bool ThrowExceptions = false;
    public static bool MessageExceptions = true;

    /// <summary>
    /// Gets the 'MySqlH' instante from 'App.Current.Properties["sql"]' and executes an action. If the code fails, a MessageBox is shown.
    /// </summary>
    /// <param name="title">The MessageBox title.</param>
    /// <param name="action">The action to execute the MySqlH instance.</param>
    private static void SQL(string title, Action<MySqlH> action, Action<Exception> onError = null, bool tryAgain = false) {
        int a = 0;
    A:
        try {
            Sql.Connection.Open();
            action.Invoke(Sql);
        } catch (Exception ex) {
            if (onError != null) onError.Invoke(ex);
            if (tryAgain && a == 0) {
                a += 1;
                goto A;
            }
            if (MessageExceptions) MessageBox.Show($"Classe: {ex.GetType()}\nMensagem: {ex.Message}", title, MessageBoxButton.OK, MessageBoxImage.Error);
            if (ThrowExceptions) throw ex;
        } finally {
            if (Sql != null && Sql.Connection != null) Sql.Connection.Close();
        }
    }

    public static void Connection(string title, Action<MySqlConnection> action) {
        SQL(title, (sql) => {
            try {
                sql.Connection.Open();
                action.Invoke(sql.Connection);
            } catch (Exception ex) {
                throw ex;
            } finally {
                if (sql != null && sql.Connection != null) sql.Connection.Close();
            }
        });
    }

    public static void NonQuery(string title, string nonQuery) {
        SQL(title, (sql) => {
            sql.NonQuery(nonQuery);
        });
    }

    public static void NonQuery(string title, MySqlCommand c, Action<Exception> onError = null, bool tryAgain = false) {
        SQL(title, (sql) => {
            sql.NonQuery(c);
        }, onError, tryAgain);
    }

    public static void NonQuery(string title, Func<MySqlCommand, MySqlCommand> action, Action<Exception> onError = null, bool tryAgain = false) {
        SQL(title, (sql) => {
            var c = new MySqlCommand();
            c = action.Invoke(c);
            sql.NonQuery(c);
        }, onError, tryAgain);
    }

    public static void QueryR(string title, MySqlCommand command, Action<MySqlDataReader> action, Action<Exception> onError = null, bool tryAgain = false) {
        SQL(title, (sql) => {
            sql.QueryR(command, action);
        }, onError, tryAgain);
    }

    public static void QueryRLoop(string title, string command, Action<MySqlDataReader> action, Action<Exception> onError = null, bool tryAgain = false) {
        QueryRLoop(title, new MySqlCommand(command), action, onError, tryAgain);
    }

    public static void QueryRLoop(string title, MySqlCommand c, Action<MySqlDataReader> action, Action<Exception> onError = null, bool tryAgain = false) {
        QueryR(title, c, (r) => {
            while (r.Read()) {
                action.Invoke(r);
            }
        }, onError, tryAgain);
    }

    public static void Command(string title, Action<MySqlCommand> action) {
        SQL(title, (sql) => {
            var c = new MySqlCommand();
            c.Connection = sql.Connection;
            try {
                c.Connection.Open();
                action.Invoke(c);
            } catch (Exception ex) {
                throw ex;
            } finally {
                c.Connection.Close();
            }
        });
    }

    public static void DataAdapter(string title, string query, Action<MySqlDataAdapter> action) {
        Command(title, (co) => {
            var da = new MySqlDataAdapter();
            co.CommandText = query;
            da.SelectCommand = co;
            action.Invoke(da);
        });
    }

}