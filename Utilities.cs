using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using static System.Convert;

namespace Utilities {

    public class ControlledAction {

        public Exception last_error;
        Action Action;
        int MaxSize, MaxTime;

        public ControlledAction(Action action, int maxSize, int maxTime) {
            Action = action;
            MaxSize = maxSize;
            MaxTime = maxTime;
        }

        public void Start() {
            Thread monitor_thread = new Thread(() => { });
            Thread dangerous_thread = new Thread(() => {
                try {
                    Action.Invoke();
                } catch (Exception ex) {
                    last_error = ex;
                    if (monitor_thread.IsAlive) monitor_thread.Abort();
                }
            });
            Process process = Process.GetCurrentProcess();
            var oldSize = process.PrivateMemorySize64;
            dangerous_thread.Start();
            monitor_thread = new Thread(() => {
                double timer = MaxTime;
                while (true) {
                    Thread.Sleep(500);
                    process.Refresh();
                    if (timer <= 0) {
                        dangerous_thread.Abort();
                        MessageBox.Show("Time out");
                        break;
                    }
                    if (process.PrivateMemorySize64 - oldSize > 1024 * 1024 * MaxSize) {
                        dangerous_thread.Abort();
                        MessageBox.Show($"{MaxSize}Mb of memory used by process during calculation.");
                        //Environment.Exit(0);
                        break;
                    }
                    if (!dangerous_thread.IsAlive) break;
                    timer -= 0.5;
                }
            });
            monitor_thread.Start();
            monitor_thread.Join();
        }
    }

    public static class Errors {

        public static Exception last_error;

        public static void ShowError(string message, string title) {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowError(Exception ex) {
            ShowError(ex.Message, ex.GetType().ToString());
        }

        public static void ShowError() {
            ShowError(last_error);
        }

        public static bool SafeExecute(Action action) {
            try {
                action.Invoke();
                return true;
            } catch (Exception ex) {
                last_error = ex;
                return false;
            }
        }

    }

    public static class Converter {

        public static Int32 TryToInt32(object input, int @default, Action onError = null) {
            try {
                return ToInt32(input);
            } catch (Exception) {
                if (onError != null) onError.Invoke();
                return @default;
            }
        }

        public static double TryToDouble(object @value, double @default) {
            try {
                return ToDouble(@value);
            } catch (Exception) {
                return @default;
            }
        }

        public static List<N[,]> ConvertEvery<T, N>(this List<T[,]> matrices, Func<T, N> converter) {
            var list = new List<N[,]>();
            matrices.ForEach(x => {
                var conv = new N[x.GetLength(0), x.GetLength(1)];
                conv.ForEach((int i, int j) => converter.Invoke(x[i, j]));
                list.Add(conv);
            });
            return list;
        }

        public static N[,] ConvertEvery<T, N>(this T[,] matrix, Func<T, N> converter) {
            var conv = new N[matrix.GetLength(0), matrix.GetLength(1)];
            matrix.ForEach((int i, int j) => conv[i, j] = converter.Invoke(matrix[i, j]));
            return conv;
        }

    }

    public static class IO {

        public static bool WriteInFile(string input, string fileName = "test.txt") {
            try {
                using (var w = new StreamWriter(fileName, false)) {
                    w.Write(input);
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }

    }

    public static class Data {

        public static List<T> Tail <T> (List<T> list) {
            if (list.Count == 0) {
                return null;
            } else {
                list.RemoveAt(0);
                return list;
            }
        }

        public static bool Equal <T> (T[,] a, T[,] b, Func<T,T,bool> comparer = null) {
            if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1)) return false;
            if (comparer == null) comparer = (aa, bb) => aa.Equals(bb);
            for (int i = 0; i < a.GetLength(0); i++) {
                for (int j = 0; j < a.GetLength(1); j++) {
                    if (!comparer.Invoke(a[i,j],b[i,j])) return false;
                }
            }
            return true;
        }

        public static string ToAscII (string a) {
            a = a.ToLower();
            a = Regex.Replace(a,"[áàâã]","a");
            a = Regex.Replace(a,"[íìî]","i");
            a = Regex.Replace(a,"[úùûü]","u");
            a = Regex.Replace(a,"[óòõô]","o");
            a = Regex.Replace(a,"[éèê]","e");
            a = Regex.Replace(a,"[ç]","c");
            return a;
        }

        public static char ToSuperscript (char a) { //  ᵃ ᵇ ᶜ ᵈ ᵉ ᶠ ᵍ ʰ ⁱ ʲ ᵏ ˡ ᵐ ⁿ ᵒ ᵖ ʳ ˢ ᵗ ᵘ ᵛ ʷ ˣ ʸ ᶻ
            switch (a) {
                case 'n':
                    return 'ⁿ';
                default:
                    throw new NotImplementedException();
            }
        }

        public static string ToSubscript (double input) {
            string a = input.ToString();
            string result = "";
            for (int i = 0; i < a.Length; i++) {
                result += ToSubscript(a[i]);
            }
            return result;
        }

        public static char ToSubscript (char input) {
            switch (input) {
                case '0':
                    return '₀';
                case '1':
                    return '₁';
                case '2':
                    return '₂';
                case '3':
                    return '₃';
                case '4':
                    return '₄';
                case '5':
                    return '₅';
                case '6':
                    return '₆';
                case '7':
                    return '₇';
                case '8':
                    return '₈';
                case '9':
                    return '₉';
                default:
                    return input;
            }
        }

        public static T[] Clone<T>(T[] a) {
            return a.ToList().ToArray();
            /*var b = new T[a.Length];
            for (int i = 0; i < a.Length; i++) {
                b[i] = a[i];
            }
            return a;*/
        }

        public static T[,] Clone<T>(T[,] a) {
            var b = new T[a.GetLength(0), a.GetLength(1)];
            for (int i = 0; i < b.GetLength(0); i++) {
                for (int j = 0; j < b.GetLength(1); j++) {
                    b[i, j] = a[i, j];
                }
            }
            return b;
        }

        public static bool Any<T>(Func<T, bool> func, params T[] ts) {
            for (int i = 0; i < ts.Length; i++) if (func.Invoke(ts[i])) return true;
            return false;
        }

        public static bool All<T>(Func<T, bool> func, params T[] ts) {
            for (int i = 0; i < ts.Length; i++) if (!func.Invoke(ts[i])) return false;
            return true;
        }

        public static void ForEach<T>(this T[] array, Func<int, T> func) {
            for (int i = 0; i < array.Length; i++) {
                array[i] = func.Invoke(i);
            }
        }

        public static void ForEach<T>(this T[,] matrix, Func<T, T> func) {
            for (int i = 0; i < matrix.GetLength(0); i++) {
                for (int j = 0; j < matrix.GetLength(1); j++) {
                    matrix[i, j] = func.Invoke(matrix[i, j]);
                }
            }
        }
        public static void ForEach<T>(this T[,] matrix, Action<T> action) {
            for (int i = 0; i < matrix.GetLength(0); i++) {
                for (int j = 0; j < matrix.GetLength(1); j++) {
                    action.Invoke(matrix[i, j]);
                }
            }
        }

        public static void ForEach<T>(this T[,] matrix, Action<int, int> action) {
            for (int i = 0; i < matrix.GetLength(0); i++) {
                for (int j = 0; j < matrix.GetLength(1); j++) {
                    action.Invoke(i, j);
                }
            }
        }

        public static void ForEach<T>(this T[,] matrix, Action<T, int, int> action) {
            for (int i = 0; i < matrix.GetLength(0); i++) {
                for (int j = 0; j < matrix.GetLength(1); j++) {
                    action.Invoke(matrix[i, j], i, j);
                }
            }
        }


        public static void ForEach<T>(this T[,] matrix, Func<int, int, T> func) {
            for (int i = 0; i < matrix.GetLength(0); i++) {
                for (int j = 0; j < matrix.GetLength(1); j++) {
                    matrix[i, j] = func.Invoke(i, j);
                }
            }
        }

        public static void ForEach<T>(this T[,] matrix, Func<T, int, int, T> func) {
            for (int i = 0; i < matrix.GetLength(0); i++) {
                for (int j = 0; j < matrix.GetLength(1); j++) {
                    matrix[i, j] = func.Invoke(matrix[i, j], i, j);
                }
            }
        }

        public static bool ContainsAny(this string haystack, params string[] needles) {
            foreach (string needle in needles) {
                if (haystack.Contains(needle)) return true;
            }
            return false;
        }

        public static bool ContainsAny(this char a, params char[] search) {
            for (int i = 0; i < search.Length; i++) {
                if (a == search[i]) {
                    return true;
                }
            }
            return false;
        }

        public static double Min(List<double> list, int order) {
            var list2 = new List<double>(list);
            if (order >= list2.Count) throw new ArgumentException("'order' cannot be bigger than the list length.");
            var value = list2.Min();
            int index = 0;
            while (index++ < order) {
                for (int i = 0; i < list2.Count; i++) {
                    if (list2[i] == value) {
                        list2.RemoveAt(i);
                        break;
                    }
                }
                value = list2.Min();
            }
            return value;
        }

        public static double Max(List<double> list, int order) {
            var list2 = new List<double>(list);
            if (order >= list2.Count) throw new ArgumentException("'order' cannot be bigger than the list length.");
            var value = list2.Max();
            int indexer = 0;
            while (indexer++ < order) {
                for (int i = 0; i < list2.Count; i++) {
                    if (list2[i] == value) {
                        list2.RemoveAt(i);
                        break;
                    }
                }
                value = list2.Max();
            }
            return value;
        }

        public static string GetLeftMultiplierW(string origin, string search) {
            origin = origin.Replace(" ", "");
            string numberMatch = @"[0-9]+(\.[0-9]+)?";
            var match = Regex.Match(origin, $"-?([a-zA-Z]+|{numberMatch})?{search}");
            var result = match.Value;
            if (result.Length == 0) return "0";
            result = result.Substring(0, result.Length - search.Length);
            if (result.Length == 0) return "1";
            if (result == "-") return "-1";
            return result;
        }

        public static double GetLeftMultiplier(string origin, string match) {
            int index = origin.IndexOf(match);
            if (index < 0) {
                return 0;
            } else {
                int index2 = index - 1;
                while (index2 >= 0 && ((origin.ElementAt(index2) >= '0' && origin.ElementAt(index2) <= '9') || origin.ElementAt(index2) == '.')) {
                    index2--;
                }
                index2++;
                if (index2 == index) {
                    if (index2 > 0 && origin.ElementAt(index2 - 1) == '-') {
                        return -1;
                    } else {
                        return 1;
                    }
                } else {
                    if (index2 > 0 && origin.ElementAt(index2 - 1) == '-') {
                        index2--;
                    }
                    return ToDouble(origin.Substring(index2, index - index2));
                }
            }
        }

    }

}