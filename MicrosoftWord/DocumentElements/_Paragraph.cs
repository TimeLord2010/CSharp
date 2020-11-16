using MicrosoftWord.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftWord {

    namespace DocumentElements {

        public class _Paragraph {

            public string Text = "";
            public ParagraphStyle ParagraphStyle = new ParagraphStyle();
            public List<TextStyle> Styles = new List<TextStyle>();

            public _Paragraph() { }

            public _Paragraph(string text) {
                Text = text;
            }

            public _Paragraph(string text, ParagraphStyle style) {
                Text = text;
                ParagraphStyle = style;
            }

            public void AddStyle(int start, int end, FontStyle style) {
                if (start < end && style != null) {
                    var t1 = new TextStyle(start, end, style);
                    Styles.Add(t1);
                }
            }

        }

    }

}