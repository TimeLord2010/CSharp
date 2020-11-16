using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftWord {

    namespace Styles {

        public class TextStyle {

            public int Start, End;
            public FontStyle Style = new FontStyle();

            public TextStyle() { }

            public TextStyle(int start, int end, FontStyle style) {
                Start = start;
                End = end;
                Style = style;
            }

        }

    }

}