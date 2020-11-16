using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using static System.Convert;

static class StringHelper {

    public enum ReplaceMode {
        /// <summary>
        /// This mode will only delete the old value and insert the new value.
        /// </summary>
        Injection,
        /// <summary>
        /// This mode will delete the old value and override any overflow character.
        /// </summary>
        Substitution,
        /// <summary>
        /// This mode will start replacing characters until the new value is fully inserted. (Possible left overs from old value)
        /// </summary>
        Partial
    }

    /// <summary>
    /// Replaces a substring for a given string in the input string.
    /// </summary>
    /// <param name="input">Source string</param>
    /// <param name="oldValue">Value to be searched</param>
    /// <param name="newValue">Replace for</param>
    /// <param name="multipleTimes">If set to false, the method will only replace the first occurence</param>
    /// <param name="mode">Mode</param>
    public static string Replace(string input, string oldValue, string newValue, bool multipleTimes, ReplaceMode mode) {
        string original = input;
    Begin:
        int a = input.IndexOf(oldValue);
        if (a == -1) return input;
        string b = "";
        if (ReplaceMode.Injection == mode) {
            for (int i = 0; i < input.Length; i++) {
                if (i < a || i >= a + oldValue.Length) {
                    b += input.ElementAt(i);
                }
            }
            input = "";
            for (int i = 0; i < b.Length; i++) {
                if (a == i) {
                    input += newValue;
                }
                input += b.ElementAt(i);
            }
            if (multipleTimes) goto Begin;
        } else {
            int counter = 0;
            for (int i = 0; i < original.Length; i++) {
                if (i >= a && i < a + newValue.Length) {
                    b += newValue.ElementAt(counter++);
                } else {
                    b += original.ElementAt(i);
                }
            }
            if (counter < newValue.Length) {
                for (int i = counter; i < newValue.Length; i++) {
                    b += newValue.ElementAt(i);
                }
            }
            if (ReplaceMode.Substitution == mode && oldValue.Length > newValue.Length) {
                b = b.Remove(a + newValue.Length, oldValue.Length - newValue.Length);
            }
            if (multipleTimes) {
                input = b;
                goto Begin;
            } else {
                return b;
            }
        }
        return "";
    }

    /// <summary>
    /// For every C characters in A, insert B.
    /// </summary>
    /// <param name="original">Original string</param>
    /// <param name="injection">Insert string</param>
    /// <param name="period">Period</param>    
    /// <returns></returns>
    public static string InsertInStringEvery(string original, string injection, int period) {
        string d = "";
        for (int e = 0; e < original.Length; e++) {
            if ((e + 1) % period == 0) {
                d += injection;
            }
            d += original.ElementAt(e);
        }
        return d;
    }

    /// <summary>
    /// Inserts a new line between words in every specified number of characters
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="period"></param>
    /// <returns></returns>
    public static string BreakWordsEvery(string origin, int period) {
        string[] Words = Regex.Split(origin, " ");
        string b = "";
        int c = 0;
        for (int a = 0; a < Words.Length; a++) {
            b += Words[a] + " ";
            c += Words[a].Length + 1;
            if (c >= period) {
                b += Environment.NewLine;
                c = 0;
            }
        }
        return b;
    }

    public static bool IsDigit(char input) => input >= '0' && input <= '9';

      public static char PopEnd(ref string input) {
        if (input.Length == 0) return '\n';
        var last = input.Last();
        input = input.Substring(0, input.Length - 1);
        return last;
    }

    public static string Print<T>(T[,] matrix, string[] columnLabels = null) {
        string result = "";
        int maxSize = 0;
        for (int i = 0; i < matrix.GetLength(0); i++) {
            for (int j = 0; j < matrix.GetLength(1); j++) {
                if (matrix[i, j] is string value && value.Length > maxSize) {
                    maxSize = value.Length;
                }
            }
        }
        double c = 8;
        double a = (maxSize / c);
        a = Math.Ceiling(a);
        if (columnLabels != null) {
            for (int i = 0; i < columnLabels.Length; i++) {
                result += columnLabels[i] + new string('\t', ToInt32(a) == 0 ? 1 : ToInt32(a));
            }
            result += "\n";
        }
        for (int i = 0; i < matrix.GetLength(0); i++) {
            for (int j = 0; j < matrix.GetLength(1); j++) {
                result += matrix[i, j];
                if (matrix[i, j] is string value) {
                    double b = (maxSize - value.Length) / c;
                    for (int k = 0; k < b; k++) {
                        result += "\t";
                    }
                }
                result += "\t";
            }
            result += "\n";
        }
        return result;
    }

    public static bool IsNullEmptyWhiteSpace(string input) {
        if (input == null) return true;
        return input.Replace(" ", "").Length == 0;
    }

    public static string Print(double[,] matrix, string columnSeparator = "\t", string rowSeparator = "\n", string[] columnLabels = null, string[] rowLabels = null) {
        string result = "";
        if (columnLabels != null) {
            for (int i = 0; i < columnLabels.Length; i++) {
                result += $"{columnLabels[i]}{columnSeparator}";
            }
        }
        result += rowSeparator;
        for (int i = 0; i < matrix.GetLength(0); i++) {
            if (rowLabels != null && i < rowLabels.Length) {
                result += rowLabels[i] + columnSeparator;
            }
            for (int j = 0; j < matrix.GetLength(1); j++) {
                result += $"{matrix[i, j]}" + columnSeparator;
            }
            result += rowSeparator;
        }
        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="func">substring, index, shouldcontinue</param>
    public static void ForEachOutsideParentheses (string source, Func<string, int, bool> func) {
        int count = 0;
        for (int i = 0; i < source.Length; i++) {
            var c = source[i];
            if (c == '(') {
                count++;
            } else if (c == ')') {
                count--;
            } else if (count == 0) {
                int original_index = i;
                for (; i < source.Length && source[i] != '(' && source[i] != ')'; i++) { }
                var sub = source.Substring(original_index, i - original_index);
                var r = func(sub, original_index);
                if (!r) {
                    break;
                }
                i--;
            }
        }
    }

    //public static List<string> Split (string source, char separator, int count) {
    //    var list = new List<string>();
    //    for (var i = 0; list.Count < count && source.Length < i ; i++) {
    //        if (source[i] == separator) {
    //            list.Add(source.Substring(0, i));
    //            source = source.Substring(i);
    //        }
    //    }

    //    return list;
    //}

}

