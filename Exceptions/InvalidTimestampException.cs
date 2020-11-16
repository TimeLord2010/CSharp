using System;

namespace Exceptions {

    public class InvalidTimestampException : Exception {
        public InvalidTimestampException(string message) : base(message) {
        }
    }

}