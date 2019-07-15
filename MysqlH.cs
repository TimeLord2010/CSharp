using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;

class MySqlH {

    string CS;
    public bool ExitOnError = false;

    public MySqlH(string cs) {
        CS = cs;
    }

    public void NonQuery(MySqlCommand command) {
        Run((con) => {
            command.Connection = con;
            command.ExecuteNonQuery();
        });
    }

    public void NonQuery(string a) {
        Run(a, (command) => {
            command.ExecuteNonQuery();
        });
    }

    public List<string[]> Query (MySqlCommand command) {
        var result = new List<string[]>();
        Run((con) => {
            command.Connection = con;
            var reader = command.ExecuteReader();
            while (reader.Read()) {
                var row = new string[reader.FieldCount];
                for (int i = 0; i < row.Length; i++) {
                    try {
                        row[i] = reader.GetString(i);
                    } catch (Exception) { }
                }
                result.Add(row);
            }
        });
        return result;
    }

    public void QueryR (MySqlCommand command, Action<MySqlDataReader> action) {
        Run((con) => {
            command.Connection = con;
            var r = command.ExecuteReader();
            action.Invoke(r);
        });
    }

    public List<string[]> Query(string a) {
        var result = new List<string[]>();
        Run(a, (command) => {
            var reader = command.ExecuteReader();
            while (reader.Read()) {
                var row = new string[reader.FieldCount];
                for (int i = 0; i < row.Length; i++) {
                    row[i] = reader.GetString(i);
                }
                result.Add(row);
            }
        });
        return result;
    }

    private void Run(Action<MySqlConnection> action) {
        MySqlConnection connection = null;
        try {
            connection = new MySqlConnection(CS);
            connection.Open();
            action.Invoke(connection);
        } catch (Exception ex) {
            throw ex;
        } finally {
            if (connection != null) connection.Close();
        }
    }

    private void Run(string b, Action<MySqlCommand> a) {
        MySqlConnection connection = null;
        try {
            using (connection = new MySqlConnection(CS)) {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = b;
                a.Invoke(command);
            }
        } catch (Exception ex) {
            throw ex;
        } finally {
            if (connection != null) connection.Close();
        }
    }

}