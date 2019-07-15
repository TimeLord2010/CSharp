using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

public class WindowsSO {

    /// <summary>
    /// Shows folder browser a dialog.
    /// NuGet: Install-Package Microsoft.WindowsAPICodePack-Shell
    /// </summary>
    /// <param name="path">Choosen folder path</param>
    /// <returns>If the user has choosen a folder, then return true. False, otherwise.</returns>
    public static bool ChooseFolder(out string path) {
        using (var dialog = new CommonOpenFileDialog { IsFolderPicker = true }) {
            path = null;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                path = dialog.FileName;
                return true;
            }
        }
        return false;
    }

    public static bool ChooseFile(out string file, string defaultDirectory, string filters = "All|*.*", Window window = null) {
        using (var dialog = new CommonOpenFileDialog { IsFolderPicker = false }) {
            Regex regex = new Regex($"({RegexH.Patterns[EPatterns.PTBR_Alphabet_Char]}|\\s|[0-9])+\\|(\\*\\.(\\*|[a-zA-Z0-9]+);?)+");
            Match match;
            while ((match = regex.Match(filters)).Success) {
                var parts = match.Value.Split('|');
                dialog.Filters.Add(new CommonFileDialogFilter(parts[0], parts[1]));
                filters = filters.Substring(match.Value.Length);
            }
            dialog.DefaultDirectory = defaultDirectory;
            file = null;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                file = dialog.FileName;
                if (window != null) window.Focus();
                return true;
            }
        }
        if (window != null) window.Focus();
        return false;
    }

    public static void ShowError (string message, string title = "Error") {
        MessageBox.Show(message,title,MessageBoxButton.OK,MessageBoxImage.Error);
    }

    public static void ToCenterOfScreen (Window window) {
        double screenWidth = SystemParameters.PrimaryScreenWidth;
        double screenHeight = SystemParameters.PrimaryScreenHeight;
        double windowWidth = window.Width;
        double windowHeight = window.Height;
        window.Left = (screenWidth / 2) - (windowWidth / 2);
        window.Top = (screenHeight / 2) - (windowHeight / 2);
    }

}