using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftWord {

    namespace DocumentElements {

        public class _Table {

            readonly _Paragraph[,] Cells;
            public bool HasBorders = true;

            public int RowCount {
                get => Cells.GetLength(0);
            }

            public int ColumnCount {
                get => Cells.GetLength(1);
            }

            public _Table(int rows, int columns) {
                Cells = new _Paragraph[rows, columns];
            }

            public void Insert(int row, int column, _Paragraph paragraph) {
                Cells[row, column] = paragraph;
            }

            public void Insert (int row, int column, string text) {
                Insert(row, column, new _Paragraph(text));
            }

            public _Paragraph[] GetRow(int i) {
                _Paragraph[] paragraphs = new _Paragraph[Cells.GetLength(0)];
                for (int j = 0; j < Cells.GetLength(0); j++) {
                    paragraphs[j] = Cells[i, j];
                }
                return paragraphs;
            }

            public _Paragraph[] GetColumn(int i) {
                _Paragraph[] paragraphs = new _Paragraph[Cells.GetLength(1)];
                for (int j = 0; j < Cells.GetLength(1); j++) {
                    paragraphs[j] = Cells[j, i];
                }
                return paragraphs;
            }

            public IEnumerable<_Paragraph[]> Rows() {
                for (int i = 0; i < Cells.GetLength(0); i++) {
                    var row = GetRow(i);
                    yield return row;
                }
            }

            public IEnumerable<_Paragraph[]> Columns() {
                for (int i = 0; i < Cells.GetLength(1); i++) {
                    var column = GetColumn(i);
                    yield return column;
                }
            }

            public IEnumerable<_Paragraph> GetCells() {
                for (int i = 0; i < Cells.GetLength(0); i++) {
                    for (int j = 0; j < Cells.GetLength(1); j++) {
                        var cell = Cells[i, j];
                        yield return cell;
                    }
                }
            }

            public _Paragraph[,] GetTable() {
                return Cells;
            }

        }

    }

}