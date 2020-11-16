using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class IO {

    public static long DirectorySize(DirectoryInfo d) {
        long size = 0;
        foreach (var fi in d.GetFiles()) 
            size += fi.Length;
        foreach (DirectoryInfo di in d.GetDirectories()) 
            size += DirectorySize(di);
        return size;
    }

    public static long TotalSize (IEnumerable<FileSystemInfo> fsis) {
        long necessary_size = 0;
        foreach (var fsi in fsis) {
            if (fsi is FileInfo fi) {
                necessary_size += fi.Length;
            } else if (fsi is DirectoryInfo di) {
                necessary_size += IO.DirectorySize(di);
            }
        }
        return necessary_size;
    }

    public static void CopyDirectory (DirectoryInfo to_copy, string destination, Func<FileSystemInfo, bool> should_copy = null, Func<Exception, FileInfo, bool> on_error_continue = null) {
        CopyDirectory(to_copy, new DirectoryInfo(destination), should_copy, on_error_continue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="to_copy"></param>
    /// <param name="destination"></param>
    /// <param name="should_copy"></param>
    /// <param name="on_error_continue">Event on file copy error. Return determines if the method should continue the next files for that directory.</param>
    public static void CopyDirectory (DirectoryInfo to_copy, DirectoryInfo destination, Func<FileSystemInfo, bool> should_copy = null, Func<Exception, FileInfo, bool> on_error_continue = null) {
        if (!Directory.Exists(destination.FullName)) {
            Directory.CreateDirectory(destination.FullName);
        }
        foreach (var fi in to_copy.GetFiles()) {
            if (!should_copy?.Invoke(fi) ?? false) {
                continue;
            }
            try {
                File.Copy(fi.FullName, $"{destination.FullName}\\{fi.Name}");
            } catch (Exception ex) {
                if (!on_error_continue?.Invoke(ex, fi) ?? false) {
                    continue;
                }
            }
        }
        foreach (var di in to_copy.GetDirectories()) {
            if (!should_copy?.Invoke(di) ?? false) {
                continue;
            }
            CopyDirectory(di, new DirectoryInfo($"{destination.FullName}\\{di.Name}"), should_copy);
        }
    }

    public static void EnsureFile(string path) {
        if (!File.Exists(path)) {
            File.Create(path);
        }
    }

    public static void EnsureFolder(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
    }

    public static void DeleteDirectory(string target_dir) {
        if (!Directory.Exists(target_dir)) {
            return;
        }
        string[] files = Directory.GetFiles(target_dir);
        string[] dirs = Directory.GetDirectories(target_dir);
        foreach (string file in files) {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }
        foreach (string dir in dirs) {
            DeleteDirectory(dir);
        }
        Directory.Delete(target_dir, false);
    }

    public static void SafeDelete (params string[] file_names) {
        foreach (var item in file_names) {
            try {
                File.Delete(item);
            } catch (Exception) {

            }
        }
    }

    public static void ReadTextFile (string fn, Action<string> line_by_line) {
        ReadTextFile(fn, (line) => {
            line_by_line?.Invoke(line);
            return true;
        });
    }

    public static void ReadTextFile (string fn, Func<string, bool> line_by_line_continue) {
        using (var sr = new StreamReader(fn)) {
            while (!sr.EndOfStream) {
                if (!line_by_line_continue?.Invoke(sr.ReadLine()) ?? true) {
                    break;
                }
            }
        }
    }

    public static void WriteTextFile (string fn, string text, Encoding encoding, bool append = false) {
        using (var sw = new StreamWriter(fn, append, encoding)) {
            sw.WriteLine(text);
        }
    }

}