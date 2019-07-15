using System;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using static System.Convert;

namespace MathHelper {

    public static class Operations {

        public static ulong Factorial(ulong n) {
            if (n == 0) return 1;
            ulong r = ToUInt64(n);
            for (ulong i = ToUInt64(n) - 1; i > 0; i--) {
                r *= i;
            }
            return r;
        }

        public static uint Factorial(uint n) {
            return ToUInt32(Factorial(ToUInt64(n)));
        }
    }

    public static class Sequences {

    }

    public static class Probability {

        private const string path = "in MathH.Combanations(n, r).";

        public static uint Combination(uint n, uint r) {
            return Operations.Factorial(n) / (Operations.Factorial(n - r) * Operations.Factorial(r));
        }

        public static uint Permutation(uint n, uint r) {
            return Operations.Factorial(n) / Operations.Factorial(n - r);
        }

        public static int[,] Combinations(uint n, uint r) {
            if (r > n) throw new InvalidOperationException($"'r' was greater than 'n' {path}");
            if (n == 0) throw new InvalidOperationException($"'n' was 0 {path}");
            List<int[]> result = new List<int[]>();
            int[] a = new int[n];
            for (int i = 0; i < n; i++) a[i] = 0;
            result.Add(a);
            int j = 0;
            while (true) {
                List<int[]> b = new List<int[]>();
                for (int i = 0; i < result.Count(); i++) {
                    var row = result[i];
                    a = new int[row.Length];
                    for (int k = 0; k < row.Count(); k++) {
                        a[k] = j == k ? 1 : row[k];
                    }
                    b.Add(a);
                }
                for (int i = 0; i < b.Count; i++) result.Add(b[i]);
                if (result[result.Count - 1].All(x => x == 1)) break;
                j++;
            }
            for (int i = 0; i < result.Count; i++) {
                if (result[i].Count(x => x == 1) != r) {
                    result.RemoveAt(i--);
                }
            }
            var array = new int[result.Count,n];
            for (int i = 0; i < array.GetLength(0); i++) {
                var row = result[i];
                for (j = 0; j < array.GetLength(1); j++) {
                    array[i,j] = row[j];
                }
            }
            return array;
        }

        public static List<int[]> Combinations(int n) {
            return Combinations(ToUInt32(n));
        }

        public static List<int[]> Combinations (uint n) {
            if (n == 0) throw new InvalidOperationException($"'n' was 0 {path}");
            List<int[]> result = new List<int[]>();
            int[] a = new int[n];
            for (int i = 0; i < n; i++) a[i] = 0;
            result.Add(a);
            int j = 0;
            while (result[result.Count - 1].Any(x => x == 0)) {
                List<int[]> b = new List<int[]>();
                for (int i = 0; i < result.Count(); i++) {
                    a = new int[result[i].Length];
                    for (int k = 0; k < result[i].Length; k++) a[k] = j == k ? 1 : result[i][k];
                    b.Add(a);
                }
                for (int i = 0; i < b.Count; i++) result.Add(b[i]);
                j++;
            }
            return result;
        }

    }

}