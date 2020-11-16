using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class MySqlH {

    public string CS;
    public bool ThrowExceptions { get; set; } = false;

    public MySqlConnection Connection {
        get => new MySqlConnection(CS);
    }

    public MySqlH(MySqlConnectionStringBuilder sb) {
        CS = sb.ToString();
    }

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

    public void QueryR(MySqlCommand command, Action<MySqlDataReader> action) {
        Run((con) => {
            command.Connection = con;
            var r = command.ExecuteReader();
            action.Invoke(r);
        });
    }

    public void QueryR(string q, Action<MySqlDataReader> action) {
        QueryR(new MySqlCommand(q), action);
    }

    public void QueryRLoop(MySqlCommand c, Action<MySqlDataReader> action) {
        QueryR(c, (r) => {
            while (r.Read()) {
                action.Invoke(r);
            }
        });
    }

    public void QueryRLoop(string c, Action<MySqlDataReader> action) {
        MySqlCommand c1 = new MySqlCommand(c);
        QueryRLoop(c1, action);
    }

    public string[] GetColumns (string table_name) {
        var list = new List<string>();
        QueryRLoop();
        return list.ToArray();
    }

    private void Run(Action<MySqlConnection> action) {
        using var connection = new MySqlConnection(CS);
        connection.Open();
        action(connection);
    }

    private void Run(string b, Action<MySqlCommand> a) {
        Run((c) => {
            var command = c.CreateCommand();
            command.CommandText = b;
            a.Invoke(command);
        });
    }

}