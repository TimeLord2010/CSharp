using System;
using System.IO;
using System.Text;

public class Logger {

    public Logger(string fn) {
        FileName = fn;
    }

    string FileName { get; }
    string CurrentDateTime {
        get {
            var dt = DateTime.Now;
            return $"{dt.Year}/{dt.Month}/{dt.Day} {dt.Hour}:{dt.Minute}";
        }
    }

    public void WriteLine(string text) {
        try {
            using var sw = new StreamWriter(FileName, true, Encoding.UTF8);
            sw.WriteLine($"[{CurrentDateTime}]: {text}");
        } catch (Exception) {

        }
    }

    public void WriteLine (Exception ex) {
        WriteLine($"{ex.GetType()}: {ex.Message}");
        WriteLine($"Stack Trace: {ex.StackTrace}.");
    }

}

public static class StaticLogger {

    public static Logger Logger;

    public static void WriteLine (string text) => Logger.WriteLine(text);

    public static void WriteLine(Exception ex) => Logger.WriteLine(ex);

}