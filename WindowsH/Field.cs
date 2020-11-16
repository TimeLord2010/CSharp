using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsH {

    public class Field {

        public Field (string header) {
            Container = new StackPanel();
            var tbl = new TextBlock() {
                Text = header,
                Foreground = HeaderForegroundColor,
                FontSize = HeaderFontSize
            };
            Container.Children.Add(tbl);
            TextBox = new TextBox() {
                FontSize = FontSize,
                Background = Background,
                Foreground = Foreground
            };
            Container.Children.Add(TextBox);
        }

        public StackPanel Container;

        public Brush HeaderForegroundColor { get; set; } = Brushes.Gray;
        public double HeaderFontSize { get; set; } = 12;
        public double FontSize { get; set; } = 13;
        public Brush Background { get; set; } = Brushes.White;
        public Brush Foreground { get; set; } = Brushes.Black;
        public TextBox TextBox;
        public string Text {
            get => TextBox.Text;
        }

        public static implicit operator UIElement (Field field) {
            return field.Container;
        }

    }

}