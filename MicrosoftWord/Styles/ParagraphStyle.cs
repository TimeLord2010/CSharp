using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftWord {

    namespace Styles {

        public enum wdHorizontalAlignment {
            Left, Center, Right, Justify
        }

        public class ParagraphStyle : FontStyle {

            public wdHorizontalAlignment ParagraphAligment {
                set {
                    switch (value) {
                        case wdHorizontalAlignment.Left:
                            Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                            break;
                        case wdHorizontalAlignment.Right:
                            Alignment = WdParagraphAlignment.wdAlignParagraphRight;
                            break;
                        case wdHorizontalAlignment.Center:
                            Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                            break;
                        case wdHorizontalAlignment.Justify:
                            Alignment = WdParagraphAlignment.wdAlignParagraphJustify;
                            break;
                        default:
                            throw new Exception();
                    }
                }
            }
            public WdParagraphAlignment Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            public float LineSpacing = 10.5f;

            public ParagraphStyle() { }


        }

    }

}