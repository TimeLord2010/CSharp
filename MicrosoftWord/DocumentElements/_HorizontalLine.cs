using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using MicrosoftWord.Styles;

namespace MicrosoftWord.DocumentElements {

    class _HorizontalLine {

        public float Height { get; set; } = 1;
        public Color Color { get; set; } = Color.Black;
        public int PorcentWidth { get; set; } = 100;
        public WdHorizontalLineAlignment Alignment { get; set; } = WdHorizontalLineAlignment.wdHorizontalLineAlignCenter;

    }

}
