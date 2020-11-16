using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using static System.String;
using static System.Convert;

#pragma warning disable 0649



class SqlHelper {
	
    public class Connection {
        private string aux;
        public string CS {
            get {
                if (!IsNullOrEmpty(aux)) return aux;
                string a = "Data Source=" + DataSource + ";";
                if (!IsNullOrEmpty(InitialCatalog)) { a += "Initial Catalog" + InitialCatalog + ";"; }
                if (!IsNullOrEmpty(IntegratedSecurity)) { a += "Integrated Security=" + IntegratedSecurity + ";"; }
                if (!IsNullOrEmpty(Database)) { a += "Database=" + Database + ";"; }
                a += "Timeout=" + Timeout + ";";
                return a;
            }
            set { aux = value; }
        }
        public string DataSource;
        public string InitialCatalog;
        public string IntegratedSecurity;
        public string Database;
        public int Timeout;
    }

    public Connection connection = new Connection();
    public Exception exception;
    public bool supressErrors = false;
    public bool supressInfo = true;
    public bool resetException = true;

    public SqlHelper(string connectionString) => connection.CS = connectionString;

    public void Reseed(string table, string database = "", int value = 0) {
        if (resetException) { exception = null; }
        try {
            using (var sqlConnection = new SqlConnection(connection.CS)) {
                sqlConnection.Open();
                string a = "";
                a += database + ";";
                a += "DBCC CHECKIDENT([" + table + "], RESEED," + value + ");";
                using (var sqlCommand = new SqlCommand(a, sqlConnection)) { sqlCommand.ExecuteNonQuery(); }
                sqlConnection.Close();
            }
        } catch (Exception e) {
            exception = e;
            if (!supressErrors) { MessageBox.Show(e.Message); }
        }
    }
    public bool DatabaseExists (string Database) {
        string command = $"IF (db_id('{Database}') is not null) SELECT 1 AS res; ELSE SELECT 0 AS res;";
        DataTable dt = SqlQuery(command);
        if (dt.Rows.Count > 0) {
            int result = ToInt32(dt.Rows[0][0]);
            if (result == 1) {
                return true;
            } else {
                return false;
            }
        } else {
            A(new Exception("Error while checking if a database exists"),command);
            return false;
        }
    }
    public bool TableExists(string Table) {
        if (resetException) { exception = null; }
        string command = "IF (EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = '{0}'))";
        command += "BEGIN ";
        command += "SELECT 1 AS res;";
        command += "END ";
        command += "ELSE ";
        command += "BEGIN ";
        command += "SELECT 0 AS res;";
        command += "END ";
        DataTable dt = SqlQuery(command);
        if (dt.Rows.Count > 0) {
            int result = ToInt32(dt.Rows[0][0]);
            if (result == 1) {
                return true;
            } else {
                return false;
            }
        } else {
            A(new Exception("Error while checking if a table exists"),command);
            return false;
        }
    }
    public void SqlNonQuery(string commandText) {
        if (resetException) { exception = null; }
        try {
            using (var sqlConnection = new SqlConnection(connection.CS)) {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(commandText, sqlConnection);
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        } catch (Exception e) {
            A(e, commandText);
        }
    }
    public void SqlNonQuery(string commandText, List<SqlParameter> values) {
        if (resetException) { exception = null; }
        try {
            using (var sqlConnection = new SqlConnection(connection.CS)) {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(commandText, sqlConnection);
                for (int a = 0; a < values.Count; a++) { sqlCommand.Parameters.Add(values[a]); }
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        } catch (Exception e) {
            A(e, commandText);
        }
    }
    public void SqlNonQuery(string commandText, params SqlParameter[] values) {
        if (resetException) { exception = null; }
        try {
            using (var sqlConnection = new SqlConnection(connection.CS)) {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(commandText, sqlConnection);
                sqlCommand.Parameters.AddRange(values);
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        } catch (Exception e) {
            A(e, commandText);
        }
    }
    public DataTable SqlQuery(string commandText) {
        DataTable dataTable = new DataTable();
        if (resetException) exception = null;
        try {
            using (var sqlConnection = new SqlConnection(connection.CS)) {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(commandText, sqlConnection);
                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader()) { dataTable.Load(sqlDataReader); }
                sqlConnection.Close();
            }
        } catch (Exception e) {
            A(e, commandText);
        }
        return dataTable;
    }
    public DataTable SqlQuery(string commandText, List<SqlParameter> values) {
        if (resetException) exception = null;
        DataTable dt = new DataTable();
        try {
            using (var SqlConnection = new SqlConnection(connection.CS)) {
                SqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(commandText, SqlConnection);
                for (int a = 0; a < values.Count; a++) sqlCommand.Parameters.Add(values[a]);
                using (var SqlDataReader = sqlCommand.ExecuteReader()) { dt.Load(SqlDataReader); }
                SqlConnection.Close();
            }
        } catch (SqlException e) {
            A(e, commandText);
        }
        return dt;
    }
    public DataTable SqlQuery(string commandText, params SqlParameter[] sqlParameter) {
        DataTable dataTable = new DataTable();
        if (resetException) { exception = null; }
        try {
            using (var sqlConnection = new SqlConnection(connection.CS)) {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(commandText, sqlConnection);
                sqlCommand.Parameters.AddRange(sqlParameter);
                using (var sqlDataReader = sqlCommand.ExecuteReader()) { dataTable.Load(sqlDataReader); }
                sqlConnection.Close();
            }
        } catch (Exception e) {
            A(e, commandText);
        }
        return dataTable;
    }
    private void A(Exception c, string b) {
        exception = c;
        string a = "";
        if (!supressErrors) a += "Error:" + c.Message;
        if (!supressErrors && !supressInfo) a += Environment.NewLine;
        if (!supressInfo) a += "CommandText:" + b;
        if (a.Length > 0) MessageBox.Show(a);
    }
}
