using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum EPatterns {
    NewLine, Double, Integer, PTBR_Alphabet_Char, CPF
}

public static class RegexH {

    public static Dictionary<EPatterns, string> Patterns = Original();

    public delegate void PartsF (ref string begin, ref string search, ref string end);

    private static Dictionary<EPatterns, string> Original () {
        Dictionary<EPatterns, string> Patterns = new Dictionary<EPatterns, string>();
        Patterns.Add(EPatterns.NewLine, "\r\n?|\n");
        Patterns.Add(EPatterns.Integer, "[0-9]+");
        Patterns.Add(EPatterns.Double, $"{Patterns[EPatterns.Integer]}(\\.{Patterns[EPatterns.Integer]})?");
        Patterns.Add(EPatterns.PTBR_Alphabet_Char, "([a-zA-ZçÇ]|[áÁãÃâÂàÀ]|[íÍ]|[éÉêÊ]|[óÓ]|[úÚ])");
        Patterns.Add(EPatterns.CPF, @"[0-9]{3}(\.[0-9]{3}){2}-[0-9]{1,2}");
        return Patterns;
    }

    public static string Replace(string input, string pattern, string replacement) {
        var matches = Regex.Matches(input, pattern);
        while (matches.Count > 0) {
            var match = matches[0];
            string begin = input.Substring(0, match.Index);
            string end = input.Substring(match.Index + match.Length);
            var rp = Regex.Match(match.Value, replacement).Value;
            input = begin + rp + end;
            matches = Regex.Matches(input, pattern);
        }
        return input;
    }

    public static void Parts(this string input, string pattern, out string begin, out string match, out string end) {
        var matches = Regex.Matches(input, pattern);
        if (matches.Count > 0) {
            begin = input.Substring(0, matches[0].Index);
            match = matches[0].Value;
            end = input.Substring(matches[0].Index + matches[0].Length);
        } else {
            begin = null;
            match = null;
            end = null;
        }
    }

    public static string ForEach(this string input, string pattern, Func<string, string> func) {
        input.Parts(pattern, out string begin, out string search, out string end);
        while (begin != null && search != null && end != null) {
            string result = func.Invoke(search);
            if (Regex.IsMatch(result,pattern)) {
                throw new Exception("Infinite loop Exception.");
            }
            input = begin + result + end;
            input.Parts(pattern, out begin, out search, out end);
        }
        return input;
    }

    public static string ForEach(this string input, string pattern, PartsF func) {
        input.Parts(pattern, out string begin, out string search, out string end);
        while (begin != null && search != null && end != null) {
            func.Invoke(ref begin, ref search, ref end);
            //if (Regex.IsMatch(search, pattern)) throw new Exception("Infinite loop Exception.");
            if (Regex.IsMatch(search, pattern)) {
                search = search.ForEach(pattern,func);
            }
            if (begin + search + end == input) break;
            input = begin + search + end;
            input.Parts(pattern, out begin, out search, out end);
        }
        return input;
    }

    public static string ForEach (this string input, Regex regex, PartsF partsF) {
        return input.ForEach(regex.ToString(), partsF);
    }

}