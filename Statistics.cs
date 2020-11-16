using System;
using System.Collections.Generic;

namespace MathHelper {

    public class Statistics {

        public Statistics (double[] data) {
            Data = data;
        }

        public double[] Data;

        public double Sum {

            get {
                double sum = 0.0;
                foreach (var item in Data) {
                    sum += item;
                }
                return sum;
            }

        }

        public double Mean {

            get {
                return Sum / Data.Length;
            }
        }

        public double StandardDeviation {

            get {
                return Math.Sqrt(Variance);
            }

        }

        public double Variance {
            get {
                var mean = Mean;
                double a = 0;
                foreach (var item in Data) {
                    a += Math.Pow(item - mean, 2);
                }
                return a / Data.Length;
            }
        }

        public double StandardizedScore (double value) {
            return (value - Mean) / StandardDeviation;
        }

        public string PrintAll {

            get {
                var a = "";
                a += $"Data: {String.Join(", ",Data)}\n";
                a += new string('-',50) + "\n";
                a += $"Sum: {Sum}\n";
                a += $"Mean: {Mean}\n";
                a += $"Standard deviation: {StandardDeviation}\n";
                a += $"Variance: {Variance}\n";
                a += $"Standardized score for: \n";
                for (int i = 0; i < Data.Length; i++) {
                    a += $"\t-{Data[i]}: {StandardizedScore(Data[i])}\n";
                }
                return a;
            }
        }

    }

}
