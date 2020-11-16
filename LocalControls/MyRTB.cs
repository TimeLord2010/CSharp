using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace LocalControls {

    public class MyRTB {

        public MyRTB (string message = "") {
            RichTextBox.Document = Document;
            var para = new Paragraph();
            para.Inlines.Add(new Run(message));
            Document.Blocks.Add(para);
            RichTextBox.TextChanged += (s,e) => { 
                if (TextChanged != null) {
                    TextChanged.Invoke(s,e);
                }
            };
        }

        readonly RichTextBox RichTextBox = new RichTextBox();
        readonly FlowDocument Document = new FlowDocument();
        public HorizontalAlignment HorizontalAlignment { 
            get => RichTextBox.HorizontalAlignment; 
            set => RichTextBox.HorizontalAlignment = value; 
        }
        public VerticalAlignment VerticalAlignment { 
            get =>  RichTextBox.VerticalAlignment; 
            set => RichTextBox.VerticalAlignment = value; 
        }
        public Thickness Margin {
            get => RichTextBox.Margin; 
            set => RichTextBox.Margin = value;
        }
        public Thickness Padding { 
            get => RichTextBox.Padding; 
            set => RichTextBox.Padding = value; 
        }
        public double FontSize {
            get => RichTextBox.FontSize;
            set => RichTextBox.FontSize = value;
        }
        public bool IsReadOnly {
            get => RichTextBox.IsReadOnly;
            set => RichTextBox.IsReadOnly = value;
        }
        public Thickness BorderThickness {
            get => RichTextBox.BorderThickness;
            set => RichTextBox.BorderThickness = value;
        }
        public Brush Foreground {
            get => RichTextBox.Foreground;
            set => RichTextBox.Foreground = value;
        }
        public event TextChangedEventHandler TextChanged;

        public static implicit operator RichTextBox (MyRTB rtb) {
            return rtb.RichTextBox;
        }

    }

}