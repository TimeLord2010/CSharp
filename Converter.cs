using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;
using static System.Convert;
using static System.String;
using System.Numerics;

public static class Converter {

    public static Int32 TryToInt32(object input, int @default = default, Action onError = null) {
        try {
            return ToInt32(input);
        } catch (Exception) {
            if (onError != null) onError.Invoke();
            return @default;
        }
    }

    public static double TryToDouble(object @value, double @default, CultureInfo cultureInfo = null) {
        try {
            return ToDouble(@value, cultureInfo ?? CultureInfo.CurrentCulture);
        } catch (Exception) {
            return @default;
        }
    }

    public static float TryToSingle(object @value, float @default) {
        try {
            return ToSingle(@value);
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

    internal static long TryToInt64(object input, int @default = -1) {
        try {
            return ToInt64(input);
        } catch (Exception) {
            return @default;
        }
    }

    public static N TryConvert<N>(Func<N> converter, N @default = default) {
        try {
            return converter();
        } catch (Exception) {
            return @default;
        }
    }

    public static T TryConvert <N, T> (N input, Func<N, T> converter, T @default = default) {
        try {
            return converter(input);
        } catch (Exception) {
            return @default;
        }
    }

    public static DateTime Converter_DT (string input, DateTime @default = default, string format = "dd/MM/yyyy") {
        return TryConvert(() => DateTime.ParseExact(input, format, CultureInfo.InvariantCulture), @default);
    }

    public static byte TryToByte(object input, byte @default = default) {
        return TryConvert(() => ToByte(input), @default);
    }

    public static DateTime? TryToNullDateTime (string input, string format, DateTime? @default = null) {
        return TryConvert(() => DateTime.ParseExact(input, format, CultureInfo.InvariantCulture), @default);
    }

    public static string DoubleToString(double input, string decimalSeparator = ",", int decimalDigits = 2) {
        NumberFormatInfo nfinfo = new NumberFormatInfo() {
            NumberDecimalSeparator = decimalSeparator,
            NumberDecimalDigits = decimalDigits
        };
        return Format(nfinfo, "{0:N}", input);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="packet">In hexadecimal</param>
    /// <returns></returns>
    public static List<byte> ToByteList(string packet, int number_base = 16, bool reverse_packet = false, bool reverse_cell = false) {
        if (number_base == 16) {} 
        else if (number_base == 10) {
            packet = DecimalToHexadecimal(packet);
        }
        if (reverse_packet) packet = Join("", packet.Reverse());
        List<byte> _data = new List<byte>();
        for (int i = 0; i < packet.Length; i += 2) {
            var cell = packet.Substring(i, 2);
            if (reverse_cell) cell = Join("", cell.Reverse());
            _data.Add(Byte.Parse(cell, NumberStyles.HexNumber));
        }
        return _data;
    }

    public static string DecimalToHexadecimal(string number) {
        var big_int = BigInteger.Parse(number);
        BigInteger quoeficient = big_int;
        var remainders = new List<int>();
        do {
            var remainder = quoeficient % 16;
            quoeficient /= 16;
            remainders.Add((int)remainder);
        } while (quoeficient != 0);
        string a = "";
        for (int i = remainders.Count - 1; i >= 0; i--) {
            a += remainders[i].ToString("X");
        }
        return a;
    }

    public static string ToHexadecimal(string packet) {
        return Join("", packet.Select(c => ((int)c).ToString("X2")));
    }

    public static string ToHexadecimal(IEnumerable<byte> bytes, int length = -1) {
        if (length < 0) length = bytes.Count();
        return Join("", bytes.Take(length).Select(x => x.ToString("X2")));
    }

    public static void ToPropertyValue(object instance, PropertyInfo pi, object value) {
        var proper_value = ToType(pi.PropertyType, value);
        pi.SetValue(instance, proper_value);
    }

    public static object ToType(Type t, object v) {
        if (t == typeof(byte)) {
            return ToByte(v);
        } else if (t == typeof(short)) {
            return ToInt16(v);
        } else if (t == typeof(int)) {
            return ToInt32(v);
        } else if (t == typeof(long)) {
            return ToInt64(v);
        } else if (t == typeof(float)) {
            return ToSingle(v);
        } else if (t == typeof(double)) {
            return ToDouble(v);
        } else if (t == typeof(string)) {
            return v.ToString();
        }
        throw new ArgumentException($"Type ({t.GetType()}) is not mapped yet or is invalid.");
    }

}