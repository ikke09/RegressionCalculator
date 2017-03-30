using RuntimeFunctionParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Regression
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Point> _inputPoints;
        private Function _regressionFunction;
        private int _degreeOfPolynominal;
        private const int cMaxDegreeForPolynominal = 9;
        private const int cMinDegreeForPolynominal = 1;

        private TextBox _txtDegreeForPolynominal;
        public MainWindow()
        {
            InitializeComponent();
            _inputPoints = new List<Point>();
            _regressionFunction = null;
            _degreeOfPolynominal = 1;
            _txtDegreeForPolynominal = new TextBox();
        }

        private void mainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Clicked on Canvas
            Point p = e.GetPosition(null);
            _inputPoints.Add(p);
            RenderPoints(_inputPoints);
        }

        private void RenderPoints(List<Point> points)
        {
            foreach (Point p in points)
            {
                Ellipse e = new Ellipse();
                e.Width = e.Height = 10;
                e.Margin = new Thickness(p.X, p.Y, 0, 0);
                e.Fill = Brushes.Black;
                e.Stroke = Brushes.White;
                e.StrokeThickness = 1;
                mainCanvas.Children.Add(e);
            }
        }

        private void RenderFunction(Function f)
        {
            if (f == null)
                return;

            double minX = double.MaxValue;
            double maxX = double.MinValue;
            foreach (Point p in _inputPoints)
            {
                if (p.X > maxX)
                    maxX = p.X;

                if (p.X < minX)
                    minX = p.X;
            }

            double step = ((maxX - minX) / _inputPoints.Count) * 0.1;
            double range = minX;
            Polyline pl = new Polyline();
            pl.Stroke = Brushes.Red;
            pl.StrokeThickness = 3;
            PointCollection pc = new PointCollection();
            for (double i = minX - range; i <= maxX + range; i += step)
            {
                Point p = new Point(i, f.Solve(i, 0));
                pc.Add(p);
            }
            pl.Points = pc;
            mainCanvas.Children.Add(pl);
        }

        private void RenderText(string text)
        {
            mainCanvas.Children.Remove(_txtDegreeForPolynominal);
            _txtDegreeForPolynominal.Clear();

            _txtDegreeForPolynominal.Text = text;
            _txtDegreeForPolynominal.Foreground = Brushes.GreenYellow;
            _txtDegreeForPolynominal.FontSize = 16.0;
            int x = (int)(mainCanvas.RenderSize.Width - 20);
            int y = 10;
            Canvas.SetLeft(_txtDegreeForPolynominal, x);
            Canvas.SetTop(_txtDegreeForPolynominal, y);
            mainCanvas.Children.Add(_txtDegreeForPolynominal);
        }

        private void mainCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                _regressionFunction = RegressionFunction.CalculateRegression(_inputPoints, _degreeOfPolynominal);
                RenderFunction(_regressionFunction);
            }
            else if (e.Key == Key.R)
            {
                Reset();
            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                int val = _degreeOfPolynominal + 1;
                _degreeOfPolynominal = Clamp(val, cMinDegreeForPolynominal, cMaxDegreeForPolynominal);
                RenderText("" + _degreeOfPolynominal);
            }
            else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
            {
                int val = _degreeOfPolynominal - 1;
                _degreeOfPolynominal = Clamp(val, cMinDegreeForPolynominal, cMaxDegreeForPolynominal);
                RenderText("" + _degreeOfPolynominal);
            }
        }

        private int Clamp(int val, int min, int max)
        {
            if (val < min)
                return min;
            else if (val > max)
                return max;

            return val;
        }

        private void mainCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            mainCanvas.Focusable = true;
            mainCanvas.Focus();
            RenderText("" + _degreeOfPolynominal);
        }

        private void Reset()
        {
            _inputPoints.Clear();
            _regressionFunction = null;
            _degreeOfPolynominal = cMinDegreeForPolynominal;
            mainCanvas.Children.Clear();
            RenderText("" + _degreeOfPolynominal);
        }
    }
}
