using System;

namespace Exceptions {

    public class DatabaseException : Exception {

        public DatabaseException (string query, Exception innerException, string message = "Error in database.") : base(message, innerException) {
            Query = query;
        }

        public string Query { get; }

    }

}