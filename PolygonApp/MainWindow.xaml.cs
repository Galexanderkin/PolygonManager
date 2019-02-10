namespace PolygonApp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Point> polygonPoints = new List<Point>();
        Polygon polygonA;
        Polygon polygonB;
        Polygon mergerPolygon;
        Polygon intersectPolygon;

        public MainWindow()
        {
            InitializeComponent();
            polygonA = new Polygon
            {
                Stroke = Brushes.Blue,
            };
            mainGrid.Children.Add(polygonA);
            polygonB = new Polygon
            {
                Stroke = Brushes.Red,
            };
            mainGrid.Children.Add(polygonB);
            intersectPolygon = new Polygon
            {
                Stroke = Brushes.Green,
                Fill = Brushes.Green
            };
            mainGrid.Children.Add(intersectPolygon);
            mergerPolygon = new Polygon
            {
                Stroke = Brushes.Indigo,
                Fill = Brushes.Indigo
            };
            mainGrid.Children.Add(mergerPolygon);
        }
      
        private void intersectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = PolygonCalculator.Polygon
                    .Intersect(
                        new PolygonCalculator.Polygon(
                            this.polygonA.Points
                            .Select(x => new PolygonCalculator.Point(Convert.ToSingle(x.X), Convert.ToSingle(x.Y)))
                            .ToArray()),
                        new PolygonCalculator.Polygon(
                            this.polygonB.Points
                            .Select(x => new PolygonCalculator.Point(Convert.ToSingle(x.X), Convert.ToSingle(x.Y)))
                            .ToArray()));
                intersectPolygon.Points = new PointCollection(result.Points.Select(x => new Point(x.X, x.Y)));
            }
            catch (Exception ex)
            {
                this.ClearPolygons();
                MessageBox.Show($"Exception text: {ex.Message}{Environment.NewLine}Try again", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void mergerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PolygonCalculator.Polygon result = PolygonCalculator.Polygon
                .Merge(
                new PolygonCalculator.Polygon(
                    this.polygonA.Points
                    .Select(x => new PolygonCalculator.Point(Convert.ToSingle(x.X), Convert.ToSingle(x.Y)))
                    .ToArray()),
                new PolygonCalculator.Polygon(
                    this.polygonB.Points
                    .Select(x => new PolygonCalculator.Point(Convert.ToSingle(x.X), Convert.ToSingle(x.Y)))
                    .ToArray()));
                mergerPolygon.Points = new PointCollection(result.Points.Select(x => new Point(x.X, x.Y)));
            }
            catch (Exception ex)
            {
                this.ClearPolygons();
                MessageBox.Show($"Exception text: {ex.Message}{Environment.NewLine}Try again", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearPolygons()
        {
            mergerButton.IsEnabled = false;
            intersectButton.IsEnabled = false;
            clearButton.IsEnabled = false;
            polygonA.Points.Clear();
            polygonB.Points.Clear();
            if (intersectPolygon != null)
            {
                intersectPolygon.Points.Clear();
            }
            if (mergerPolygon != null)
            {
                mergerPolygon.Points.Clear();
            }
            polygonPoints.Clear();
            mainGrid.Children.OfType<Line>().ToList().ForEach(x => mainGrid.Children.Remove(x));
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = Mouse.GetPosition(mainGrid);
            coordX.Text = p.X.ToString();
            coordY.Text = p.Y.ToString();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (polygonA.Points.Any() && polygonB.Points.Any())
            {
                return;
            }
            Point p = Mouse.GetPosition(mainGrid);
            if (polygonPoints.Any())
            {
                Line edge = new Line
                {
                    X1 = polygonPoints.Last().X,
                    Y1 = polygonPoints.Last().Y,
                    X2 = p.X,
                    Y2 = p.Y,
                    Stroke = Brushes.Black
                };
                mainGrid.Children.Add(edge);
                clearButton.IsEnabled = true;
            }
            polygonPoints.Add(p);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.C:

                    IList<Line> lines = mainGrid.Children.OfType<Line>().ToList();
                    foreach (Line item in lines)
                    {
                        mainGrid.Children.Remove(item);
                    }
                    if (!polygonA.Points.Any())
                    {
                        polygonA.Points = new PointCollection(polygonPoints);
                    }
                    else if (!polygonB.Points.Any())
                    {
                        polygonB.Points = new PointCollection(polygonPoints);
                    }
                    polygonPoints.Clear();
                    if (polygonA.Points.Any() && polygonB.Points.Any())
                    {
                        mergerButton.IsEnabled = true;
                        intersectButton.IsEnabled = true;
                        clearButton.IsEnabled = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClearPolygons();
        }  
    }
}
