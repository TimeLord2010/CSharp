using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClinica.Scripts.DB {

    public abstract class Template {

        protected Action CT = null;

        public abstract string TableName { get; }

        public abstract Action GetCT();

        public void CreateTable() {
            CT.Invoke();
        }

        private void Create(Exception ex) {
            if (ex.Message.Contains("doesn't exist")) {
                CT = GetCT();
                CT.Invoke();
                CreateTable();
            }
        }

        protected void QueryRLoop(string title, MySqlCommand c, Action<MySqlDataReader> action) {
            using (c) {
                SQLOperations.QueryRLoop(title, c, action, Create, true);
            }
        }

        protected void NonQuery(string title, MySqlCommand c) {
            using (c) {
                SQLOperations.NonQuery(title, c, Create, true);
            }
        }

        protected void NonQuery(string title, Func<MySqlCommand, MySqlCommand> action) {
            SQLOperations.NonQuery(title, action, Create, true);
        }

        public void NonQuery(string title, string q) {
            using (var c = new MySqlCommand(q)) {
                SQLOperations.NonQuery(title, c, Create, true);
            }
        }

    }
}
