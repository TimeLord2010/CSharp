using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

public abstract class PageInitializer<T> : PageHandler<T> {

    protected abstract string Title { get; }
    protected abstract int Width { get; }
    protected abstract int Height { get; }
    protected virtual WindowStyle WS { get => WindowStyle.SingleBorderWindow; }

    public PageInitializer(T page) : base(page) {
        InnerPage.Loaded += Page_Loaded;
    }

    protected void Page_Loaded(object sender, RoutedEventArgs e) {
        ControlsH.InitializeWindow(InnerPage, Title, Width, Height, WS);
    }

}