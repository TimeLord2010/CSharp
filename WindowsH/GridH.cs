using System.Windows;
using System.Windows.Controls;

namespace WindowsH {

    public class GridH {

        public GridH (Grid grid) {
            Container = grid;
        }

        Grid Container;

        public void Add (UIElement element, int row = 0, int column = 0, int row_span = 1, int column_span = 1) {
            Container.Children.Add(element);
            Grid.SetRow(element, row);
            Grid.SetRowSpan(element, row_span);
            Grid.SetColumn(element, column);
            Grid.SetColumnSpan(element, column_span);
        }

        public void SetRowDefinitions (params (double, GridUnitType)[] cds) {
            SetRowDefinitions(Container, cds);
        }

        public void SetRowDefinitions (int count, double value, GridUnitType unit) {
            Container.RowDefinitions.Clear();
            for (var i = 0; i < count; i++) {
                var rd = new RowDefinition {
                    Height = new GridLength(value, unit)
                };
                Container.RowDefinitions.Add(rd);
            }
        }

        public void SetColumnDefinitions(params (double, GridUnitType)[] sizes) {
            SetColumnDefinitions(Container, sizes);
        }

        public void SetColumnDefinitions (int count, int value = 1, GridUnitType unit = GridUnitType.Auto) {
            Container.ColumnDefinitions.Clear();
            for (var i = 0; i < count; i++) {
                var cd = new ColumnDefinition();
                cd.Width = new GridLength(value, unit);
                Container.ColumnDefinitions.Add(cd);
            }
        }

        public static void SetRowDefinitions(Grid grid, int count, params double[] sizes) {
            grid.RowDefinitions.Clear();
            for (int i = 0; i < count; i++) {
                var rd = new RowDefinition();
                var size = i < sizes.Length ? sizes[i] : double.NaN;
                if (!double.IsNaN(size)) rd.Height = new GridLength(size);
                grid.RowDefinitions.Add(rd);
            }
        }

        public static void SetRowDefinitions(Grid grid, params (double, GridUnitType)[] cds) {
            grid.RowDefinitions.Clear();
            foreach (var (value, type) in cds) {
                var cd = new RowDefinition {
                    Height = new GridLength(value, type)
                };
                grid.RowDefinitions.Add(cd);
            }
        }

        public static void SetColumnDefinitions(Grid grid, int count, params double[] sizes) {
            grid.ColumnDefinitions.Clear();
            for (int i = 0; i < count; i++) {
                var cd = new ColumnDefinition();
                var size = i < sizes.Length ? sizes[i] : double.NaN;
                if (!double.IsNaN(size)) cd.Width = new GridLength(size);
                grid.ColumnDefinitions.Add(cd);
            }
        }

        public static void SetColumnDefinitions(Grid grid, params (double, GridUnitType)[] cds) {
            grid.ColumnDefinitions.Clear();
            foreach (var (value, type) in cds) {
                var cd = new ColumnDefinition {
                    Width = new GridLength(value, type)
                };
                grid.ColumnDefinitions.Add(cd);
            }
        }

    }

}