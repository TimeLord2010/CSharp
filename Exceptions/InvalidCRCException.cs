using System;

namespace Exceptions {

    public class InvalidCRCException : Exception {
        public InvalidCRCException(string message) : base(message) {
        }
    }

}