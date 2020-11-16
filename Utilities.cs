using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using static System.Convert;

namespace Utilities {

    public static class IO {

        public static void SafeDelete(params string[] names) {
            foreach (var item in names) {
                if (File.Exists(item)) File.Delete(item);
            }
        }

        public static bool WriteInFile(string input, string name = "test.txt") {
            try {
                using (var w = new StreamWriter(name, false)) {
                    w.Write(input);
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public static void MakeFolderEqual(string d1, string d2) {
            MFE(d1, d2);
            MFE(d2, d1);
        }

        public static (List<FileInfo>, List<DirectoryInfo>, List<FileInfo>, List<DirectoryInfo>) Simulate_MakeFolderEqual (string d1, string d2) {
            var r = (f1: new List<FileInfo>(), d1: new List<DirectoryInfo>(), f2: new List<FileInfo>(), d2: new List<DirectoryInfo>());
            MFE(d1, d2, true, r.f2, r.d2);
            MFE(d2, d1, true, r.f1, r.d1);
            return r;
        }

        static void MFE(string d1, string d2, bool simulate = false, List<FileInfo> addFiles = null, List<DirectoryInfo> addDirectories = null) {
            foreach (var fi1 in GetFileInfos(d1)) {
                bool found = false;
                foreach (var fi2 in GetFileInfos(d2)) if (fi1.Name == fi2.Name) found = true;
                if (!found) {
                    if (simulate) {
                        addFiles.Add(fi1);
                    } else {
                        File.Copy(fi1.FullName, $"{d2}/{fi1.Name}");
                    }
                }
            }
            foreach (var dir1 in GetDirectoryInfos(d1)) {
                bool found = false;
                foreach (var dir2 in GetDirectoryInfos(d2)) if (dir1.Name == dir2.Name) found = true;
                var nd = $"{d2}/{dir1.Name}";
                if (found) {
                    MFE(dir1.FullName, nd, simulate, addFiles, addDirectories);
                } else {
                    if (simulate) {
                        addDirectories.Add(new DirectoryInfo(nd));
                    } else {
                        CopyDirectory(dir1.FullName, d2);
                    }
                }
            }
        }

        public static void CopyDirectory (string source, string destination) {
            var di = new DirectoryInfo(source);
            var nd = $"{destination}/{di.Name}";
            Directory.CreateDirectory(nd);
            foreach (var fi in GetFileInfos(source)) {
                File.Copy(fi.FullName, $"{nd}/{fi.Name}");
            }
        }

        static IEnumerable<FileInfo> GetFileInfos(string d) => Directory.GetFiles(d).Select(x => new FileInfo(x));

        static IEnumerable<DirectoryInfo> GetDirectoryInfos(string d) => Directory.GetDirectories(d).Select(x => new DirectoryInfo(x));


    }

}