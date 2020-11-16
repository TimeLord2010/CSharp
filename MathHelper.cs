using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Shapes;
using static System.Math;
using static System.Convert;
using static System.String;
using static StringHelper;
using static RegexH;
using static Utilities.Data;
using System.Windows.Controls;

namespace MathHelper {

    public class Arithmetics {

        public enum Operation {
            Add, Sub, Mult, Div
        }

        public static readonly char[] Operands = { '+', '-', '*', '•', '×', '÷', '/', '≤', '≥', '=' };

        public static string Solve(string input) {
            input = Simplify(input);
            input.Parts($"{Patterns[EPatterns.Double]}\\*-?{Patterns[EPatterns.Double]}", out string begin, out string search, out string end);
            while (All(x => x != null, begin, search, end)) {
                var parts = search.Split('*');
                var conv = Array.ConvertAll(parts, x => ToDouble(x));
                input = begin + (conv[0] * conv[1]) + end;
                input.Parts($"{Patterns[EPatterns.Double]}\\*{Patterns[EPatterns.Double]}", out begin, out search, out end);
            }
            input.Parts($"(^|[+-]){Patterns[EPatterns.Double]}-{Patterns[EPatterns.Double]}($|[+-])", out begin, out search, out end);
            while (All(x => x != null, begin, search, end)) {
                if (search[0] == '+') {
                    begin += "+";
                    search = search.Substring(1);
                }
                if (search.Last() == '-' || search.Last() == '+') {
                    end = search.Last() + end;
                    search = search.Substring(0, search.Length - 1);
                }
                var parts = search.Split('-');
                input = begin + (ToDouble(parts[0]) - ToDouble(parts[1])) + end;
                input.Parts($"(^|[+-]){Patterns[EPatterns.Double]}-{Patterns[EPatterns.Double]}($|[+-])", out begin, out search, out end);
            }
            input = Simplify(input);
            return input;
        }

        public static string Simplify(string input) {
            input = input.Replace(" ", "");
            int i = 0;
        A:
            input = Replace(input, @"\(-?[a-zA-Z0-9]+\)", "-?[a-zA-Z0-9]+");
            var regex = new Regex(@"(^|[+-])\(.+\)($|[+-])");
            input.ForEach(regex, (ref string x, ref string y, ref string z) => {
                if (y[0] == '-') {
                    x += "-";
                    y = y.Substring(1);
                    y = Negate(y);
                }
                if (y.Last() == '+' || y.Last() == '-') {
                    z = y.Last() + z;
                    y = y.Substring(0, y.Length - 1);
                }
                y = y.Substring(1, y.Length - 2);
            });
            input = input.Replace("--", "-");
            input = input.Replace("++", "+");
            input = input.Replace("-+", "-");
            input = input.Replace("+-", "-");
            input = input.Replace("=+", "=");
            input = Regex.Replace(input, "([0-9]+\\*|[a-zA-Z]+\\*?)0", "0");
            input = Regex.Replace(input, "^0-", "-");
            input = Regex.Replace(input, "-0$", "");
            input = Regex.Replace(input, "^1\\*1$", "1");
            input = Regex.Replace(input, "\\*1$", "");
            input = Regex.Replace(input, "^1\\*", "");
            regex = new Regex("-?[a-zA-Z]+-[a-zA-Z]+($|[+-])");
            input.ForEach(regex, (ref string x, ref string y, ref string z) => {
                if (!Regex.IsMatch(y.Last() + "", "[a-zA-Z]")) {
                    z = y.Last() + z;
                    y = y.Substring(0, y.Length - 1);
                }
                if (!Regex.IsMatch(y[0] + "", "[a-zA-Z]")) {
                    var parts = y.Substring(1).Split('-');
                    if (parts[0] == parts[1]) {
                        y = "-2*" + parts[0];
                    }
                } else {
                    var parts = y.Split('-');
                    if (parts[0] == parts[1]) {
                        y = "0";
                    }
                }

            });
            /*regex = new Regex(@"(^|[+\-\(])[a-zA-Z]($|[+\-\)])");
            input.ForEach(regex, (ref string x, ref string y, ref string z) => {
                if (!Regex.IsMatch(y[0] + "", "[a-zA-Z]")) {
                    x += y[0];
                    y = y.Substring(1);
                }
                if (!Regex.IsMatch(y.Last() + "", "[a-zA-Z]")) {
                    z = y.Last() + z;
                    y = y.Substring(0, y.Length - 1);
                }
                y = "1*" + y;
            });*/
            input = Replace(input, "-[a-zA-Z]+\\*-1$", "\\+[a-zA-Z]+");
            if (input.Length == 0) return "0";
            if (input.Length > 0 && input.ElementAt(0) == '+') input = input.Substring(1);
            if (i++ == 0) goto A;
            return input;
        }

        public static string Negate(string input) {
            input = Simplify(input);
            if (IsNullEmptyWhiteSpace(input)) return "";
            if (input.ElementAt(0) != '-' && input.ElementAt(0) != '+') {
                input = input.Insert(0, "+");
            }
            input.ForEach(@"\([0-9a-zA-Z]", x => x = x.Replace("(", "(+"));
            input = input.Replace("-", "$");
            input = input.Replace("+", "-");
            input = input.Replace("$", "+");
            input = input.Replace("≤", "≥");
            input = input.Replace("≥", "≤");
            input = Simplify(input);
            return input;
        }

    }

    namespace Algebra {

        public static class Matrix {

            public static T[,] Transpose<T>(T[,] matrix) {
                int w = matrix.GetLength(0), h = matrix.GetLength(1);
                var result = new T[h, w];
                for (int i = 0; i < w; i++) {
                    for (int j = 0; j < h; j++) {
                        result[j, i] = matrix[i, j];
                    }
                }
                return result;
            }

            public static string AllToLeft(string equation) {
                if (IsNullEmptyWhiteSpace(equation)) return "";
                if (equation.Count(x => x == '=') != 1) throw new FormatException("Can't have less or more than 1 of '='.");
                string[] parts = equation.Split('=');
                parts[1] = Arithmetics.Negate(parts[1]);
                equation = Join("+", parts);
                return equation + "=0";
            }

            public static double[,] MultRow(double[,] matrix, int row, double multiplier) {
                for (int i = 0; i < matrix.GetLength(1); i++) {
                    matrix[row, i] = matrix[row, i] * multiplier;
                }
                return matrix;
            }

            public static string[,] MultRow(string[,] matrix, int row, string multiplier) {
                for (int i = 0; i < matrix.GetLength(1); i++) {
                    matrix[row, i] = $"({matrix[row, i]})*({multiplier})";
                }
                return matrix;
            }

            public static double[,] MultColumn(double[,] matrix, int column, double multiplier) {
                for (int i = 0; i < matrix.GetLength(1); i++) {
                    matrix[i, column] = matrix[i, column] * multiplier;
                }
                return matrix;
            }

            public static string[,] MultColumn(string[,] matrix, int column, string multiplier) {
                for (int i = 0; i < matrix.GetLength(1); i++) {
                    matrix[i, column] = $"({matrix[i, column]})*({multiplier})";
                }
                return matrix;
            }

            public static double[,] SubtractRowByRow(double[,] matrix, int row1, int row2, double mult1, double mult2) {
                for (int i = 0; i < matrix.GetLength(1); i++) {
                    matrix[row1, i] = mult1 * matrix[row1, i] - mult2 * matrix[row2, i];
                }
                return matrix;
            }

            public static string[,] SubtractRowByRow(string[,] matrix, int row1, int row2, string mult1, string mult2) {
                for (int i = 0; i < matrix.GetLength(1); i++) {
                    matrix[row1, i] = $"({mult1})*({matrix[row1, i]})-({mult2})*({matrix[row2, i]})";
                }
                return matrix;
            }

            /// <summary>
            /// UNTESTED
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="matrix"></param>
            /// <param name="column"></param>
            /// <param name="count"></param>
            /// <returns></returns>
            public static T[,] RemoveColumn<T>(T[,] matrix, int column, int count = 1) {
                column = FilterIndex(matrix, column, 1);
                if (count < 1) throw new ArgumentException();
                var converted = new T[matrix.GetLength(0), matrix.GetLength(1) - count];
                for (int i = 0; i < converted.GetLength(0); i++) {
                    int columni = 0, counter = 0;
                    for (int j = 0; j < converted.GetLength(1); j++) {
                        if (j >= columni && counter < count) {
                            columni++;
                            counter++;
                        }
                        converted[i, j] = matrix[i, columni];
                    }
                }
                return converted;
            }

            public static T[,] RemoveRow<T>(T[,] matrix, int row, int count = 1) {
                row = FilterIndex(matrix, row, 0);
                if (count < 1) throw new ArgumentException();
                var converted = new T[matrix.GetLength(0) - count, matrix.GetLength(1)];
                int rowi = 0, counter = 0;
                for (int i = 0; i < converted.GetLength(0); i++) {
                    if (i >= rowi && counter < count) {
                        rowi++;
                        counter++;
                    }
                    for (int j = 0; j < converted.GetLength(1); j++) {
                        converted[i, j] = matrix[rowi, j];
                    }
                }
                return converted;
            }

            public static List<double> ColumnToList(double[,] matrix, int column) {
                var a = new List<double>();
                for (int i = 0; i < matrix.GetLength(0); i++) {
                    a.Add(matrix[i, column]);
                }
                return a;
            }

            public static List<double> RowToList(double[,] matrix, int row) {
                row = FilterIndex(matrix, row, 0);
                var a = new List<double>();
                for (int i = 0; i < matrix.GetLength(1); i++) {
                    a.Add(matrix[row, i]);
                }
                return a;
            }

            private static int FilterIndex <T> (T[,] matrix, int index, int dimention) {
                int length = matrix.GetLength(dimention);
                if (length > 0) {
                    while (index < 0) {
                        index += length;
                    }
                    while (index > length) {
                        index -= length;
                    }
                }
                return index;
            }

        }
    }

    namespace Trigonometry {

        public enum Coeficient {
            X,
            Y
        }

        public class CartesianPlane {

            public static double ArcTan(double tangent) {
                if (double.IsInfinity(tangent)) return 90;
                bool IsNegative = false, IsBig = false;
                if (tangent < 0) {
                    IsNegative = true;
                    tangent = -tangent;
                }
                double result = 0;
                if (tangent > 1) {
                    IsBig = true;
                    tangent = tangent / (1 + Sqrt(1 + Pow(tangent, 2)));
                }
                int count = 1000, index = 0;
            A:
                result += Pow(-1, index) / (2 * index + 1) * Pow(tangent, 2 * index + 1);
                index++;
                if (index < count) goto A;
                result *= (180 / PI); // randians to degrees
                if (double.IsNaN(result)) {
                    MessageBox.Show($"Return value would be NaN.\nTangent: {tangent}", "Error in MathHelper.Geral.ArcTan()");
                    Environment.Exit(Environment.ExitCode);
                }
                if (IsBig) {
                    result *= 2;
                }
                if (IsNegative) return 360 - result;
                return result;
            }

            public static double PointsDistance(Point a, Point b) => Sqrt(Pow(a.X - b.X, 2) + Pow(a.Y - b.Y, 2));

            /// <summary>Measures the vertical or horizontal distance of two points</summary>
            /// <param name="c">true for X and false for Y</param>
            public static double PointsDiference(Point a, Point b, bool c) {
                double d = c ? a.X : a.Y;
                double e = c ? b.X : b.Y;
                if (d > e) {
                    return d - e;
                } else {
                    return e - d;
                }
            }

            public static bool HavePointInCommom(Line line1, Line line2) {
                return
                    (line1.X1 == line2.X1 && line1.Y1 == line2.Y1) ||
                    (line1.X2 == line2.X2 && line1.Y2 == line2.Y2) ||
                    (line1.X1 == line2.X2 && line1.Y1 == line2.Y2) ||
                    (line1.X2 == line2.X1 && line1.Y2 == line2.Y1);
            }

            public static bool HaveSamePoints(Line line1, Line line2) => line1.X1 == line2.X1 && line1.X2 == line2.X2 && line1.Y1 == line2.Y1 && line1.Y2 == line2.Y2;
        }

        public class FirstDegree {
            /// <summary>coeficient = a*x + b</summary>
            public struct Equation {
                public Coeficient coeficient;
                /// <summary>a*x</summary>
                public double a;
                /// <summary>b</summary>
                public double b;
            }

            public static Equation FindEquation(Point a, Point b) {
                double originalAngle = 0, originalYdiference = a.Y - b.Y, originalXdiference = a.X - b.X;
                Equation lineEquation = new Equation();
                if (originalXdiference == 0) { // original line is a vertical line
                    lineEquation.coeficient = Coeficient.X;
                    lineEquation.b = a.X;
                } else {
                    lineEquation.coeficient = Coeficient.Y;
                    originalAngle = originalYdiference / originalXdiference;
                    double aux = a.Y - (originalAngle * a.X);
                    lineEquation.a = originalAngle;
                    lineEquation.b = aux;
                }
                return lineEquation;
            }

            public static Equation FindEquation(Point point, double tangent) {
                Equation equation = new Equation();
                if (double.IsInfinity(tangent)) {
                    equation.coeficient = Coeficient.X;
                    equation.b = point.X;
                } else {
                    equation.coeficient = Coeficient.Y;
                    equation.a = tangent;
                    equation.b = point.Y - point.X * tangent;
                }
                return equation;
            }

            public static double FindValueFor(Equation equation, double x) {
                return equation.a * x + equation.b;
            }

            public static double GetLineAngle(Point a, Point b) {
                double XDiference = a.X - b.X, YDiference = a.Y - b.Y;
                return XDiference == 0 ? Double.PositiveInfinity : YDiference / XDiference;
            }

            public static double GetAngle(Equation lineEquation) {
                if (lineEquation.coeficient == Coeficient.Y) {
                    double x1 = 1, x2 = 2;
                    double y1 = (lineEquation.a * x1) + lineEquation.b, y2 = (lineEquation.a * x2) + lineEquation.b;
                    double XDiference = x2 - x1, YDiference = y2 - y1;
                    return YDiference / XDiference;
                } else {
                    return Double.PositiveInfinity;
                }
            }

            /// <summary>
            /// Calculates two points that: have the specified distance from the given point, and belong to the given line equation.
            /// </summary>
            /// <param name="equation">Line equation</param>
            /// <param name="reference">Reference point</param>
            /// <param name="distance">Distance</param>
            /// <returns></returns>
            public static Point[] PointsWithDistance(Equation equation, Point reference, double distance) {
                reference.Y = FindValueFor(equation, reference.X);
                SecondDegree.Equation equation2 = new SecondDegree.Equation() {
                    coeficient = Coeficient.Y,
                    a = 1 + Pow(equation.a, 2),
                    b = (2 * equation.a * equation.b) - (2 * reference.X) - (2 * reference.Y * equation.a),
                    c = Pow(reference.X, 2) + Pow(reference.Y, 2) + Pow(equation.b, 2) - (2 * reference.Y * equation.b) - Pow(distance, 2)
                };
                Point[] points = SecondDegree.Bhaskara(equation2);
                if (points == null || (double.IsNaN(points[0].X) && double.IsNaN(points[1].X))) {
                    MessageBox.Show("Shutting down...", "Error: line had no intersection in curve");
                    Environment.Exit(Environment.ExitCode);
                }
                points[0].Y = FindValueFor(equation, points[0].X);
                points[1].Y = FindValueFor(equation, points[1].X);
                return points;
            }

            public static Point Intersection(Equation equation1, Equation equation2) {
                double x = (equation2.b - equation1.b) / (equation1.a - equation2.a);
                double y = FindValueFor(equation1, x);
                return new Point(x, y);
            }

            public static double DistanceFromPoint(Equation equation, Point point) {
                var PEquation = GetPerpendicularEquation(equation, point);
                Point intersectionP = Intersection(equation, PEquation);
                return CartesianPlane.PointsDistance(point, intersectionP);
            }

            public static double GetPerpendicularAngle(Equation equation) {
                return -(1 / GetAngle(equation));
            }

            public static Equation GetPerpendicularEquation(Equation equation, Point point) {
                Equation equation1 = new Equation();
                double angle = GetAngle(equation);
                double PerpendicularAngle = GetPerpendicularAngle(equation);
                if (angle == 0) {
                    equation1.coeficient = Coeficient.X;
                    equation1.b = point.Y;
                } else {
                    equation1.coeficient = Coeficient.Y;
                    equation1.a = PerpendicularAngle;
                    equation1.b = point.Y - (PerpendicularAngle * point.X);
                }
                return equation1;
            }

            /// <summary>
            /// Bounces equation1 with equation2. As if a projectile (equation1) hits a wall (equation2), and bounces.
            /// </summary>
            /// <param name="equation"></param>
            /// <param name="equation"></param>
            /// <returns></returns>
            public static Equation Reflect(Equation equation1, Equation equation2) {
                double oldAngle = CartesianPlane.ArcTan(GetAngle(equation1));
                double hitTangent = GetAngle(equation2);
                double hitAngle = CartesianPlane.ArcTan(hitTangent);
                double newAngle = 2 * hitAngle - oldAngle;
                return FindEquation(Intersection(equation1, equation2), Tan(newAngle * (PI / 180)));
            }

            public static Equation ChangeAngle(Equation equation, Point reference, double angle) {
                double oldAngle = GetAngle(equation);
                Equation equation1 = new Equation();
                if (Double.IsInfinity(oldAngle)) {
                    return FindEquation(reference, angle);
                } else {
                    equation1.coeficient = Coeficient.Y;
                    equation1.a = angle;
                    equation1.b = reference.Y - (reference.X * angle);
                }
                return equation1;
            }

            /// <summary>
            /// Multiplies a and b from the equation by the given number.
            /// </summary>
            /// <param name="equation"></param>
            /// <param name="number"></param>
            /// <returns></returns>
            public static Equation Multiply(Equation equation, double number) {
                equation.a *= number;
                equation.b *= number;
                return equation;
            }

            public static string Print(Equation equation) {
                char a = equation.coeficient == Coeficient.Y ? 'y' : 'x';
                char b = a == 'y' ? 'x' : 'y';
                return $"{a} = {equation.a:0.###}*{b} + {equation.b:0.###}";
            }
        }

        public class SecondDegree {
            /// <summary>coeficient = a*x² + b*x + c</summary>
            public struct Equation {
                public Coeficient coeficient;
                /// <summary>a*x²</summary>
                public double a;
                /// <summary>b*x</summary>
                public double b;
                /// <summary>c</summary>
                public double c;
            }
            public static Equation Multiply(Equation equation, double number) {
                equation.a *= number;
                equation.b *= number;
                equation.c *= number;
                return equation;
            }
            public static string Print(Equation equation) {
                char a = equation.coeficient == Coeficient.X ? 'x' : 'y';
                char b = equation.coeficient == Coeficient.X ? 'y' : 'x';
                string result = $"{a} = {equation.a:0.###}*{b}² + {equation.b:0.###}*{b} + {equation.c:0.###}";
                return result;
            }
            public static Point[] Bhaskara(Equation equation) {
                double delta = Pow(equation.b, 2) - 4 * equation.a * equation.c;
                double x1 = (-equation.b - Sqrt(delta)) / (2 * equation.a);
                double x2 = (-equation.b + Sqrt(delta)) / (2 * equation.a);
                Point[] points = new Point[2];
                points[0] = new Point(x1, 0);
                points[1] = new Point(x2, 0);
                return points;
            }
        }

        public class Circumference {
            /// <summary>a*x² + b*y² + c*x + d*y + e</summary>
            public class Equation {
                /// <summary>a*x²</summary>
                public readonly double a = 1;
                /// <summary>b*y²</summary>
                public readonly double b = 1;
                /// <summary>c*x</summary>
                public double c;
                /// <summary>d*y</summary>
                public double d;
                /// <summary>e</summary>
                public double e;
            }
            public static Equation FindEquation(Point center, double radius) {
                Equation equation = new Equation();
                equation.c = -(2 * center.X);
                equation.d = -(2 * center.Y);
                equation.e = System.Math.Pow(center.X, 2) + System.Math.Pow(center.Y, 2) - System.Math.Pow(radius, 2);
                return equation;
            }
            public static Point[] Intersection(Equation equation, FirstDegree.Equation lineEquation) {
                SecondDegree.Equation resultEquation;
                if (lineEquation.coeficient == Coeficient.Y) {
                    resultEquation = new SecondDegree.Equation {
                        coeficient = Coeficient.Y,
                        a = equation.a + equation.b * Pow(lineEquation.a, 2),
                        b = 2 * equation.b * lineEquation.a * lineEquation.b + equation.d * lineEquation.a + equation.c,
                        c = equation.e + equation.d * lineEquation.b + equation.b * Pow(lineEquation.b, 2)
                    };
                } else {
                    resultEquation = new SecondDegree.Equation {
                        coeficient = Coeficient.X,
                        a = Pow(lineEquation.a, 2) * equation.a + equation.b,
                        b = 2 * equation.a * lineEquation.a * lineEquation.b + equation.d + equation.c * lineEquation.a,
                        c = equation.a * Pow(lineEquation.b, 2) + equation.c * lineEquation.b + equation.e
                    };
                }
                Point[] points = SecondDegree.Bhaskara(resultEquation);
                points[0].Y = lineEquation.a * points[0].X + lineEquation.b;
                points[1].Y = lineEquation.a * points[1].X + lineEquation.b;
                return points;
            }
            public static string Print(Equation equation) {
                return Format(new CultureInfo("en-US"), "{0}*x² + {1}*y² + {2}*x + {3}*y + {4}", equation.a, equation.b, equation.c, equation.d, equation.e);
            }
        }
    }

}
