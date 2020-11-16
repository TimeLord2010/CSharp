using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Convert;

public static class Data {

    /// <summary>
    /// Check if a type is integer. Note that boolean is considered integer.
    /// </summary>
    public static bool IsInteger(Type t) {
        return Any(x => x == t, typeof(int), typeof(long), typeof(short), typeof(byte), typeof(bool));
    }

    public static int Age(DateTime Nascimento) {
        var now = DateTime.Today;
        var age = now.Year - Nascimento.Year;
        if (Nascimento.Date > now.AddYears(-age)) age--;
        return age;
    }

    public static List<T> Tail<T>(List<T> list) {
        if (list.Count == 0) {
            return null;
        } else {
            list.RemoveAt(0);
            return list;
        }
    }

    public static bool Equal<T>(T[,] a, T[,] b, Func<T, T, bool> comparer = null) {
        if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1)) return false;
        if (comparer == null) comparer = (aa, bb) => aa.Equals(bb);
        for (int i = 0; i < a.GetLength(0); i++) {
            for (int j = 0; j < a.GetLength(1); j++) {
                if (!comparer.Invoke(a[i, j], b[i, j])) return false;
            }
        }
        return true;
    }

    public static string ToAscII(string a) {
        a = a.ToLower();
        a = Regex.Replace(a, "[áàâã]", "a");
        a = Regex.Replace(a, "[íìî]", "i");
        a = Regex.Replace(a, "[úùûü]", "u");
        a = Regex.Replace(a, "[óòõô]", "o");
        a = Regex.Replace(a, "[éèê]", "e");
        a = Regex.Replace(a, "[ç]", "c");
        return a;
    }

    public static char ToSuperscript(char a) { //  ᵃ ᵇ ᶜ ᵈ ᵉ ᶠ ᵍ ʰ ⁱ ʲ ᵏ ˡ ᵐ ⁿ ᵒ ᵖ ʳ ˢ ᵗ ᵘ ᵛ ʷ ˣ ʸ ᶻ
        return a switch
        {
            'n' => 'ⁿ',
            _ => throw new NotImplementedException(),
        };
    }

    public static string ToSubscript(double input) {
        string a = input.ToString();
        string result = "";
        for (int i = 0; i < a.Length; i++) {
            result += ToSubscript(a[i]);
        }
        return result;
    }

    public static char ToSubscript(char input) {
        return input switch
        {
            '0' => '₀',
            '1' => '₁',
            '2' => '₂',
            '3' => '₃',
            '4' => '₄',
            '5' => '₅',
            '6' => '₆',
            '7' => '₇',
            '8' => '₈',
            '9' => '₉',
            _ => input,
        };
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
        foreach (var item in ts) if (func(item)) return true;
        return false;
    }

    public static bool All<T>(Func<T, bool> func, params T[] ts) {
        for (int i = 0; i < ts.Length; i++) if (!func.Invoke(ts[i])) return false;
        return true;
    }

    public static void ForEach<T>(T[] array, Func<int, T> func) {
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
    public static void ForEach<T>(T[,] matrix, Action<T> action) {
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

    public static void ForEach<T>(IEnumerable<T> collection, Action<T> action) {
        foreach (var item in collection) {
            action(item);
        }
    }

    public static void ForEach<T>(Action<T> action, params T[] ts) {
        foreach (T t in ts) {
            action.Invoke(t);
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

    public static void DoFor<T>(Action<T> action, params T[] ts) {
        foreach (var item in ts) {
            action.Invoke(item);
        }
    }

    public static int GetBites(long a) {
        if (a < 0) {
            throw new NotImplementedException("Negative not implemented.");
        }
        int count = 1;
        while (a > Math.Pow(2, count++) - 1) {
        }
        return count;
    }

}