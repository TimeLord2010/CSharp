using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using MicrosoftWord.DocumentElements;
using MicrosoftWord.Other;
using MicrosoftWord.Styles;
using ColorTranslator = System.Drawing.ColorTranslator;
using TextRenderer = System.Windows.Forms.TextRenderer;
using WordApp = Microsoft.Office.Interop.Word.Application;

namespace MicrosoftWord {

    enum Orientation {
        Portraint, Landscape
    }

    class WordDocument {

        object missing = Missing.Value;
        public readonly List<object> Content = new List<object>();
        public bool ShowAnimation = true;
        public bool Visible = true;
        public Orientation Orientation = Orientation.Portraint;
        public float LeftMargin;
        public float TopMargin;
        public float RightMargin;
        public float BottomMargin;

        public List<ConditionalFormatting> ConditionalFormattings = new List<ConditionalFormatting>();

        public WordDocument() {
            LeftMargin = TopMargin = RightMargin = BottomMargin = 20;
        }

        public void Create() {
            var app = new WordApp {
                ShowAnimation = ShowAnimation,
                Visible = Visible
            };
            var d = app.Documents.Add(ref missing, ref missing, ref missing, ref missing);
            d.PageSetup.PaperSize = WdPaperSize.wdPaperA4;
            d.PageSetup.LeftMargin = LeftMargin;
            d.PageSetup.TopMargin = TopMargin;
            d.PageSetup.RightMargin = RightMargin;
            d.PageSetup.BottomMargin = BottomMargin;
            d.PageSetup.Orientation = Orientation == Orientation.Portraint ?
                WdOrientation.wdOrientPortrait :
                WdOrientation.wdOrientLandscape;
            foreach (var item in Content) {
                Add(d, item);
            }
        }

        private void Add(Document d, object item) {
            if (item is _Paragraph paragraph) {
                AddParagraph(d, paragraph);
            } else if (item is _Table table) {
                AddTable(d, table);
            } else if (item is string text) {
                AddParagraph(d, new _Paragraph(text));
            } else if (item is _HorizontalLine horizontalLine) {
                AddHorizontalLine(d, horizontalLine);
            }
        }

        private void AddHorizontalLine(Document d, _HorizontalLine horizontalLine) {
            var line = d.Paragraphs.Last.Range.InlineShapes.AddHorizontalLineStandard(ref missing);
            line.Height = horizontalLine.Height;
            line.Fill.Solid();
            line.HorizontalLineFormat.NoShade = true;
            line.Fill.ForeColor.RGB = ColorTranslator.ToOle(horizontalLine.Color);
            line.HorizontalLineFormat.PercentWidth = horizontalLine.PorcentWidth;
            line.HorizontalLineFormat.Alignment = horizontalLine.Alignment;
        }

        private void AddTable(Document d, _Table table) {
            if (table.RowCount <= 0 || table.ColumnCount <= 0) {
                return;
            }
            try {
                object oEndOfDoc = "\\endofdoc";
                Range wrdRng = d.Bookmarks.get_Item(ref oEndOfDoc).Range;
                var table1 = d.Tables.Add(wrdRng, table.RowCount, table.ColumnCount, ref missing, ref missing);
                //table1.Borders.Shadow = true;
                Utilities.Data.ForEach(table.GetTable(), (cell, i, j) => {
                    if (cell == null) return;
                    var range = table1.Cell(i + 1, j + 1).Range;
                    table1.Cell(i+1, j+1).TopPadding = 10;
                    range.Text = cell == null ? "" : cell.Text;
                    ApplyStyle(range.Font, cell == null ? new ParagraphStyle() : cell.ParagraphStyle);
                    SetStyles(d, range, cell == null ? new List<TextStyle>() : cell.Styles);
                    range.ParagraphFormat.Alignment = cell.ParagraphStyle.Alignment;
                    range.ParagraphFormat.LineSpacing = cell.ParagraphStyle.LineSpacing;
                });
                try {
                    if (table.HasBorders) {
                        table1.Borders.InsideLineStyle = WdLineStyle.wdLineStyleSingle;
                        table1.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
                        //table1.Borders.Shadow = true;
                    }
                } catch (Exception ex) {
                    System.Windows.MessageBox.Show(ex.Message, "Erro in table border");
                }
            } catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.Message, "Error in table instance.");
            }
        }

        private void AddParagraph(Document d, _Paragraph paragraph) {
            var p = d.Content.Paragraphs.Add(ref missing);
            var ps = paragraph.ParagraphStyle;
            p.Range.Text = paragraph.Text;
            ApplyStyle(p.Range.Font, ps);
            SetStyles(d, p.Range, paragraph.Styles);
            p.LineSpacing = ps.LineSpacing;
            p.Alignment = ps.Alignment;
            for (int i = 0; i < ConditionalFormattings.Count; i++) {
                var cf = ConditionalFormattings[i];
                for (var m = Regex.Match(paragraph.Text, cf.regex); m.Success; m = m.NextMatch()) {
                    object start = p.Range.Start + m.Index;
                    object end = p.Range.Start + m.Index + m.Length;
                    var rng = d.Range(ref start, ref end);
                    ApplyStyle(rng.Font, cf.FontStyle);
                }
            }
            p.Range.InsertParagraphAfter();
        }

        private static void ApplyStyle(Font font, FontStyle style) {
            font.Name = style.FontName;
            font.Size = style.FontSize;
            font.Bold = style.Bold ? 1 : 0;
            font.Italic = style.Italic ? 1 : 0;
            font.Underline = style.Underline;
        }

        private void SetStyles(Document d, Range range, List<TextStyle> styles) {
            foreach (var style in styles) {
                if (style == null) continue;
                object start = range.Start + style.Start;
                object end = range.Start + style.End;
                var rng = d.Range(start, end);
                ApplyStyle(rng.Font, style.Style);
            }
        }

        private Paragraph CreatePara(Document d, string text, ParagraphStyle ps) {
            var p = d.Content.Paragraphs.Add(ref missing);
            p.LineSpacing = ps.LineSpacing;
            ApplyStyle(p.Range.Font, ps);
            p.Range.Text = text;
            if (text.Contains(":")) {
                object objStart = p.Range.Start;
                object objEnd = p.Range.Start + text.IndexOf(":");
                var rngBold = d.Range(ref objStart, ref objEnd);
                rngBold.Bold = 1;
            }
            p.Alignment = ps.Alignment;
            p.Range.InsertParagraphAfter();
            return p;
        }

        private static string CalcSeparator(int total, string separator, System.Drawing.Font f, string left, string right) {
            var w1 = TextRenderer.MeasureText(left, f).Width;
            var w2 = TextRenderer.MeasureText(right, f).Width;
            var t1 = total - (w1 + w2);
            var sep = separator;
            while (TextRenderer.MeasureText(separator, f).Width <= t1) {
                sep += separator;
            }
            return sep;
        }
    }

}