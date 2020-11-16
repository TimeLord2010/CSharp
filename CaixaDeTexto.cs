using System;
using System.Windows;
using System.Windows.Controls;
using WindowsH;

public enum ResultadoDiálogo {
    Nulo, Ok, Sim, Não, Cancelar
}

public enum BotõesDaCaixaDeMensagem {
    Nulo, Ok, SimNão, SimNãoCancelar
}

public class CaixaDeMensagem : Window {

    Grid MyGrid = new Grid();
    Grid ContainerDeBotões = new Grid() { 
        HorizontalAlignment = HorizontalAlignment.Right
    };

    new ResultadoDiálogo DialogResult;

    public CaixaDeMensagem(string conteúdo, string título = "") {
        ConstruirGenerico(título, conteúdo);
    }

    void ConstruirGenerico (string título, string conteúdo) {
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Width = 300;
        Height = 150;
        ResizeMode = ResizeMode.NoResize;
        Title = título;
        var rd1 = new RowDefinition();
        MyGrid.RowDefinitions.Add(rd1);
        var rd2 = new RowDefinition() {
            Height = new GridLength(35)
        };
        MyGrid.RowDefinitions.Add(rd2);
        var tbl = new TextBlock() { 
            Text = conteúdo,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(10)
        };
        MyGrid.Children.Add(tbl);
        Grid.SetRow(ContainerDeBotões, 1);
        MyGrid.Children.Add(ContainerDeBotões);
        Content = MyGrid;
    }

    public ResultadoDiálogo MostrarDiálogo (BotõesDaCaixaDeMensagem bts) {
                GridH gridH = new GridH(ContainerDeBotões);
        switch (bts) {
            case BotõesDaCaixaDeMensagem.Ok:
                break;
            case BotõesDaCaixaDeMensagem.SimNãoCancelar:
                gridH.SetColumnDefinitions(3);
                gridH.Add(CriarBotãoSim());
                gridH.Add(CriarBotãoNão(), 0, 1);
                gridH.Add(CriarBotãoCancelar(), 0, 2);
                break;
            case BotõesDaCaixaDeMensagem.SimNão:
                gridH.SetColumnDefinitions(2);
                gridH.Add(CriarBotãoSim(), 0, 0);
                gridH.Add(CriarBotãoNão(), 0, 1);
                break;
        }
        ShowDialog();
        return DialogResult;
    }

    Button CriarBotãoCancelar () {
        return CriarBotão("Cancelar", ResultadoDiálogo.Cancelar);
    }

    Button CriarBotãoNão () {
        return CriarBotão("Não", ResultadoDiálogo.Não);
    }

    Button CriarBotãoSim () {
        return CriarBotão("Sim", ResultadoDiálogo.Sim);
    }

    Button CriarBotão (string text, ResultadoDiálogo resultado) {
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