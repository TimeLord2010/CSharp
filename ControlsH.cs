using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

public class ControlsH {

    public static DependencyObject GetScrollViewer(DependencyObject o) {
        if (o is ScrollViewer) { return o; }
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++) {
            var child = VisualTreeHelper.GetChild(o, i);
            var result = GetScrollViewer(child);
            if (result == null) {
                continue;
            } else {
                return result;
            }
        }
        return null;
    }

    public static void EnsureThickness(Shape shape) {
        shape.SnapsToDevicePixels = true;
        shape.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
    }

    public static void EnsureThickness(UIElement element) {
        element.SnapsToDevicePixels = true;
        element.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
    }

    public static void BehaveText(TextBox box, string text) {
        box.Text = text;
        box.Foreground = Brushes.Gray;
        box.GotKeyboardFocus += (sender, e) => {
            if (box.Text == text) {
                box.Text = "";
                box.Foreground = Brushes.Black;
            }
        };
        box.LostKeyboardFocus += (sender, e) => {
            if (box.Text == "" || box.Text == text) {
                box.Text = text;
                box.Foreground = Brushes.Gray;
            }
        };
    }

    public static void BehaveText(PasswordBox box, string text) {
        box.Password = text;
        box.Foreground = Brushes.Gray;
        box.GotKeyboardFocus += (sender, e) => {
            if (box.Password == text) {
                box.Password = "";
                box.Foreground = Brushes.Black;
            }
        };
        box.LostKeyboardFocus += (sender, e) => {
            if (box.Password == "" || box.Password == text) {
                box.Password = text;
                box.Foreground = Brushes.Gray;
            }
        };
    }

    /*public static Rect GetRect(FrameworkElement element) {
        double x, y, width, height;
        x = y = width = height = 0;
        var parent = element.Parent;
        if (parent is Grid grid) {
            switch (element.HorizontalAlignment) {
                case HorizontalAlignment.Left:
                    x = element.Margin.Left;
                    width = element.Width;
                    break;
                case HorizontalAlignment.Stretch:
                    width = grid.ActualWidth - (element.Margin.Left + element.Margin.Right);
                    x = width - element.Margin.Right;
                    break;
                case HorizontalAlignment.Right:
                    width = element.Width;
                    x = grid.ActualWidth - (element.Margin.Right + width);
                    break;
                case HorizontalAlignment.Center:
                    break;
                default: break;
            }
            switch (element.VerticalAlignment) {

            }
        }
        return new Rect(x, y, width, height);
    }*/

}