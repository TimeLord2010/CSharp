using System;
using System.Windows;
using System.Windows.Controls;
using WindowsH;

public enum ResultadoDi�logo {
    Nulo, Ok, Sim, N�o, Cancelar
}

public enum Bot�esDaCaixaDeMensagem {
    Nulo, Ok, SimN�o, SimN�oCancelar
}

public class CaixaDeMensagem : Window {

    Grid MyGrid = new Grid();
    Grid ContainerDeBot�es = new Grid() { 
        HorizontalAlignment = HorizontalAlignment.Right
    };

    new ResultadoDi�logo DialogResult;

    public CaixaDeMensagem(string conte�do, string t�tulo = "") {
        ConstruirGenerico(t�tulo, conte�do);
    }

    void ConstruirGenerico (string t�tulo, string conte�do) {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Width = 300;
        Height = 150;
        ResizeMode = ResizeMode.NoResize;
        Title = t�tulo;
        var rd1 = new RowDefinition();
        MyGrid.RowDefinitions.Add(rd1);
        var rd2 = new RowDefinition() {
            Height = new GridLength(35)
        };
        MyGrid.RowDefinitions.Add(rd2);
        var tbl = new TextBlock() { 
            Text = conte�do,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(10)
        };
        MyGrid.Children.Add(tbl);
        Grid.SetRow(ContainerDeBot�es, 1);
        MyGrid.Children.Add(ContainerDeBot�es);
        Content = MyGrid;
    }

    public ResultadoDi�logo MostrarDi�logo (Bot�esDaCaixaDeMensagem bts) {
                GridH gridH = new GridH(ContainerDeBot�es);
        switch (bts) {
            case Bot�esDaCaixaDeMensagem.Ok:
                break;
            case Bot�esDaCaixaDeMensagem.SimN�oCancelar:
                gridH.SetColumnDefinitions(3);
                gridH.Add(CriarBot�oSim());
                gridH.Add(CriarBot�oN�o(), 0, 1);
                gridH.Add(CriarBot�oCancelar(), 0, 2);
                break;
            case Bot�esDaCaixaDeMensagem.SimN�o:
                gridH.SetColumnDefinitions(2);
                gridH.Add(CriarBot�oSim(), 0, 0);
                gridH.Add(CriarBot�oN�o(), 0, 1);
                break;
        }
        ShowDialog();
        return DialogResult;
    }

    Button CriarBot�oCancelar () {
        return CriarBot�o("Cancelar", ResultadoDi�logo.Cancelar);
    }

    Button CriarBot�oN�o () {
        return CriarBot�o("N�o", ResultadoDi�logo.N�o);
    }

    Button CriarBot�oSim () {
        return CriarBot�o("Sim", ResultadoDi�logo.Sim);
    }

    Button CriarBot�o (string text, ResultadoDi�logo resultado) {
        var b = new Button() {
            Content = text,
            Margin = new Thickness(5)
        };
        b.Click += new RoutedEventHandler((s, e) => {
            DialogResult = resultado;
            Close();
        });
        return b;
    }

}