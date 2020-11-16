using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftWord {

    namespace Styles {

        public class FontStyle {

            public string FontName = "Consolas";
            public float FontSize = 11f;
            public bool Bold = false;
            public bool Italic = false;
            public WdUnderline Underline = WdUnderline.wdUnderlineNone;

        }

    }

}