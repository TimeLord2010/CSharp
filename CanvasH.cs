using System.Windows;
using System.Windows.Controls;

class CanvasH {

    public Canvas Container;

    public CanvasH(Canvas canvas) {
        Container = canvas;
    }

    public static void SetLT (UIElement element, int left, int top) {
        Canvas.SetLeft(element, left);
        Canvas.SetTop(element, top);
    }

    public static void GetLT (UIElement element, out double left, out double top) {
        left = Canvas.GetLeft(element);
        top = Canvas.GetTop(element);
    }

}