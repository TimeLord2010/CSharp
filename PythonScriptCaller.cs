using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.String;

public class PythonScriptCaller {

    public PythonScriptCaller(string python_executable, string file) {
        PythonExecutable = python_executable;
        FileName = file;
    }

    public string PythonExecutable { get; }
    public string FileName { get; }
    public Action<string> Outputs = (_) => { };
    public Action<string> Errors = (_) => { };

    public void Call(IEnumerable<string> args) {
        var start = new ProcessStartInfo {
            FileName = PythonExecutable,
            Arguments = $"{FileName} {Join(" ", args.Select(x => $"\"{x}\""))}",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        var process = Process.Start(start);
        process.WaitForExit();
        var errors = process.StandardError.ReadToEnd();
        if (!String.IsNullOrEmpty(errors)) Errors.Invoke(errors);
        var outputs = process.StandardOutput.ReadToEnd();
        if (!String.IsNullOrEmpty(outputs)) Outputs.Invoke(outputs);
        //using (var process = Process.Start(start)) {
        //    process.ErrorDataReceived += (s, e) => {
        //        OutputEvent.Invoke($"Error: {e.Data}");
        //    };
        //    process.OutputDataReceived += (s, e) => {
        //        OutputEvent.Invoke($"Output: {e.Data}");
        //    };
        //using (var reader = process.StandardError) {
        //    var result = reader.ReadToEnd();
        //    if (result.Length > 0) OutputEvent.Invoke(reader.ReadLine());
        //}
        //using (var reader = process.StandardOutput) {
        //    //string result = reader.ReadToEnd();
        //    //OutputEvent.Invoke(result);
        //    while (!reader.EndOfStream) {
        //        OutputEvent.Invoke(reader.ReadLine());
        //    }
        //}
        //using (var errors = process.StandardError) {
        //    var error = errors.ReadToEnd();
        //    Log.Error($"Error: {error}");
        //}
        //}
    }

    public void Call (params string[] args) {
        Call(args.ToList());
    }

}