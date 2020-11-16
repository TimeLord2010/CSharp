using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public class TextBoxUtils {

    public static void SelectAllOnFocus (TextBox tb) {
        tb.PreviewMouseLeftButtonDown += Tb_PreviewMouseLeftButtonDown_Focus;
        tb.GotKeyboardFocus += Tb_GotKeyboardFocus_SelectAll;
        tb.GotFocus += Tb_GotFocus_SelectAll;
    }

    private static void Tb_GotFocus_SelectAll(object sender, RoutedEventArgs e) {
        var tb = sender as TextBox;
        tb.SelectAll();
    }

    private static void Tb_GotKeyboardFocus_SelectAll(object sender, KeyboardFocusChangedEventArgs e) {
        var tb = sender as TextBox;
        tb.SelectAll();
    }

    private static void Tb_PreviewMouseLeftButtonDown_Focus(object sender, MouseButtonEventArgs e) {
        var tb = sender as TextBox;
        if (!tb.IsKeyboardFocusWithin) {
            e.Handled = true;
            tb.Focus();
        }
    }
}