using System;
using System.Windows;

namespace Utilities {

	public class Errors {

        public Exception last_error;

        public bool SafeExecute(Action action) {
            try {
                action.Invoke();
                return true;
            } catch (Exception ex) {
                last_error = ex;
                return false;
            }
        }

        public bool SafeExecute(string message, Action action) {
            try {
                action.Invoke();
                return true;
            } catch (Exception ex) {
                last_error = ex;
                MessageBox.Show(message + $"\nMensagem: {ex.Message}");
                return false;
            }
        }

    }

}