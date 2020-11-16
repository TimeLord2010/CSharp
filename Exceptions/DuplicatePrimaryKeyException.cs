using System;
using System.Collections.Generic;
using System.Windows.Documents;
using static System.String;

namespace Exceptions {

    public class DuplicatePrimaryKeyException : Exception {

        public DuplicatePrimaryKeyException(string table, string statement, IEnumerable<string> fields) {
            TableName = table;
            Statement = statement;
            Fields = fields;
        }

        public string TableName { get; }
        public IEnumerable<string> Fields { get; }
        public string Statement { get; }

        public override string Message => $"Sql statement '{Statement}' couldn't be completed because it violated the primary key constraint for the fields ({Join(", ", Fields)}) in the table {TableName}.";

    }

}