using System.Windows;
using System.Windows.Controls;

namespace WindowsH {

    public class ComboBoxH {

        public ComboBoxH (int selected_index = -1, params string[] items) {
            Container = new ComboBox();
            foreach (var item in items) {
                Container.Items.Add(new ComboBoxItem() { 
                    Content = item
                });
            }
            Container.SelectedIndex = selected_index;
        }

        public readonly ComboBox Container;

        public string Selection {
            get {
                var cbi = Container.SelectedItem as ComboBoxItem;
                return cbi.Content.ToString();
            }
        }

        public static implicit operator UIElement (ComboBoxH boxH) {
            return boxH.Container;
        }

    }

}