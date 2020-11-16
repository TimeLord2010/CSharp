using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WindowsH {

    //public delegate void Event

    public class WButton {

        public WButton () {
            Grid = new Grid {
                Cursor = Cursors.Hand
            };
            Grid.MouseDown += delegate {
                Rectangle.Fill = OnHoldColors.Background;
            };
            Grid.MouseUp += delegate {
                Rectangle.Fill = OnFocusColors.Background;
                OnClick?.Invoke(Grid, new EventArgs());
            };
            Grid.MouseEnter += delegate {
                Rectangle.Fill = OnFocusColors.Background;
                OnMouseEnter?.Invoke(Grid, new EventArgs());
            };
            Grid.MouseLeave += delegate {
                Rectangle.Fill = NormalColors.Background;
                OnMouseLeave?.Invoke(Grid, new EventArgs());
            };
            Rectangle = new Rectangle();
            Grid.Children.Add(Rectangle);
            ContentHolder = new Grid() {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.Children.Add(ContentHolder);
            RadiusX = 5;
            RadiusY = 5;
            Background = Brushes.LightGray;
            NormalColors = new ColorSchema() {
                Background = Brushes.LightGray,
                Foreground = Brushes.Black
            };
            OnFocusColors = new ColorSchema() {
                Background = new SolidColorBrush(Color.FromRgb(230,230,230)),
                Foreground = Brushes.Black
            };
            OnHoldColors = new ColorSchema() {
                Background = new SolidColorBrush(Color.FromRgb(150,150,150)),
                Foreground = Brushes.Black
            };
        }

        readonly Grid Grid;
        readonly Grid ContentHolder;
        readonly Rectangle Rectangle;

        public object Content { 
            get => ContentHolder.Children.Count == 0 ? null : ContentHolder.Children[0];
            set {
                ContentHolder.Children.Clear();
                if (value is string str) {
                    var tbl = new TextBlock {
                        Text = str,
                        FontSize = FontSize,
                        FontWeight = FontWeight
                    };
                    ContentHolder.Children.Add(tbl);
                } else {
                    throw new NotImplementedException();
                }
            }
        }
        public Thickness Margin {
            get => Grid.Margin;
            set => Grid.Margin = value;
        }
        public Brush Background { 
            get => Rectangle.Fill; 
            set => Rectangle.Fill = value; 
        } 
        public double RadiusX {
            get => Rectangle.RadiusX; 
            set => Rectangle.RadiusX = value; 
        }
        public double RadiusY {
            get => Rectangle.RadiusY;
            set => Rectangle.RadiusY = value; 
        }
        public double FontSize { get; set; }
        public FontWeight FontWeight { get; set; }
        public ColorSchema NormalColors { get; set; }
        public ColorSchema OnFocusColors { get; set; }
        public ColorSchema OnHoldColors { get; set; }
        public event EventHandler OnClick;
        public event EventHandler OnMouseEnter;
        public event EventHandler OnMouseLeave;

        public static implicit operator FrameworkElement (WButton b) {
            return b.Grid;
        }

    }

}