using Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace MicrosoftExcel {

    public class Excel {

        Application app = new Application();
        Workbook Workbook;
        Sheets Sheets;

        public Excel(string fn = null) {
            if (fn != null) { 
                FileName = fn;
                Workbook = app.Workbooks.Open(FileName);
                Sheets = Workbook.Sheets;
            }
        }

        ~Excel () {
            if (Workbook != null) {
                Workbook.Close();
                System.Windows.MessageBox.Show("Des");
            }
        }

        readonly object M = Missing.Value;

        public string this[int book, int row, int column] {
            get {
                var sheet = (Worksheet)Sheets.Item[book];
                return sheet.Cells[row, column].Value.ToString();
            }
        }

        public string FileName { get; }

        public int SheetCount {
            get => Sheets.Count;
        }

        public int RowCount (int sheet) {
            var s = (Worksheet)Sheets.Item[sheet];
            return s.Rows.Count;
        }

        public int ColumnCount (int sheet) {
            var s = (Worksheet)Sheets.Item[sheet];
            return s.Columns.Count;
        }

    }

}