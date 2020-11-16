using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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

    public static T GetFirstParentOfType<T>(DependencyObject @object) {
        int i = 0;
        while (true) {
            i++;
            var parent = VisualTreeHelper.GetParent(@object);
            if (parent == null) {
                throw new Exception($"Parent of the given type not found. The loop ran {i} times.");
            }
            if (parent is T t) {
                return t;
            }
        }
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

    public static void CreateColumns(DataGrid dataGrid, object @class) {
        CreateColumns(dataGrid, @class.GetType());
    }

    public static void CreateColumns(DataGrid dataGrid, Type type) {
        var ps = type.GetProperties();
        for (int i = 0; i < ps.Length; i++) {
            var p = ps[i];
            CreateColumn(dataGrid, p.Name, 1, DataGridLengthUnitType.Auto);
        }
    }

    public static void CreateColumns(DataGrid dataGrid, params string[] headers) {
        for (int i = 0; i < headers.Length; i++) {
            CreateColumn(dataGrid, headers[i], 1, DataGridLengthUnitType.Auto);
        }
    }

    public static void CreateColumn(DataGrid datagrid, string header, int width = 1, DataGridLengthUnitType type = DataGridLengthUnitType.Star) {
        var c = new DataGridTextColumn() {
            Header = header,
            Binding = new Binding(header),
            Width = new DataGridLength(width, type)
        };
        datagrid.Columns.Add(c);
    }

    public static void CreateColumn(DataGrid datagrid, string header, string binding = null, int width = 1, DataGridLengthUnitType type = DataGridLengthUnitType.Star) {
        var c = new DataGridTextColumn() {
            Header = header,
            Binding = new Binding(binding ?? header),
            Width = new DataGridLength(width, type)
        };
        datagrid.Columns.Add(c);
    }

    public static void ApplyFontWeight(TextRange textRange, FontWeight weight) {
        var cw = FontWeights.Normal;
        try {
            cw = (FontWeight)textRange.GetPropertyValue(TextElement.FontWeightProperty);
        } catch (Exception) { }
        if (cw == weight) {
            weight = FontWeights.Normal;
        } else {
            weight = FontWeights.Bold;
        }
        textRange.ApplyPropertyValue(TextElement.FontWeightProperty, weight);
    }

    public static List<T> GetChildrenOfType<T> (UIElement element) {
        var list = new List<T>();
        if (element is Panel panel) {
            foreach (UIElement child in panel.Children) {
                list.AddRange(GetChildrenOfType<T>(child));
            }
        }
        if (element is T t) {
            list.Add(t);
        }
        return list;
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
