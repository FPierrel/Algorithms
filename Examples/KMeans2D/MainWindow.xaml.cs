using MachineLearning.Clusterers;
using MachineLearning.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace KMeans2D
{
    public partial class MainWindow : Window
    {
        private const int MAX_CLUSTERS = 8;

        private int _nbClusterOnScreen;
        private int _nbIteration;

        private List<Point> _points;
        private List<Point> _centroid;

        private KMeans _kmeans;

        private Color[] _clusterColor =
        {
            Color.FromRgb(255,0,0),
            Color.FromRgb(0,255,0),
            Color.FromRgb(0,0,255),
            Color.FromRgb(255,255,0),
            Color.FromRgb(0,255,255),
            Color.FromRgb(255,0,255),
            Color.FromRgb(255,128,0),
            //Color.FromRgb(128,255,0), 
            //Color.FromRgb(0,255,128),
            //Color.FromRgb(0,128,255),
            Color.FromRgb(128,0,255)
        };

        public MainWindow()
        {
            InitializeComponent();

            this._points = new List<Point>();
            this._centroid = new List<Point>();
        }

        private void TbClusterNb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");            
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AddEllipse(Cnva, e.GetPosition(Cnva).X, e.GetPosition(Cnva).Y, Color.FromRgb(214, 195, 175));            
            Cnva.UpdateLayout();

            this._points.Add(e.GetPosition(Cnva));
        }

        private void Canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_nbClusterOnScreen < MAX_CLUSTERS)
            {
                AddTriangle(Cnva, e.GetPosition(Cnva).X, e.GetPosition(Cnva).Y, this._clusterColor[this._nbClusterOnScreen]);
                Cnva.UpdateLayout();
                _nbClusterOnScreen++;

                this._centroid.Add(e.GetPosition(Cnva));
            }

            if (TbClusterNb.Text ==  "" || int.Parse(TbClusterNb.Text) < _nbClusterOnScreen)
                TbClusterNb.Text = "" + _nbClusterOnScreen;    
        }
                
        private void AddEllipse(Canvas canvas, double x, double y, Color color, double height = 10, double width = 10)
        {
            Ellipse ellipse = new Ellipse();
            SolidColorBrush brush = new SolidColorBrush();

            ellipse.Height = height;
            ellipse.Width = width;            
            ellipse.StrokeThickness = 0.0;
            brush.Color = color;
            ellipse.Fill = brush;

            Canvas.SetLeft(ellipse, x-width/2);
            Canvas.SetTop(ellipse, y-height/2);

            canvas.Children.Add(ellipse);
        }               

        private void AddTriangle(Canvas canvas, double x, double y, Color color, double height = 10, double width = 10)
        {
            Polygon triangle = new Polygon();
            SolidColorBrush brush = new SolidColorBrush();

            triangle.Points = new PointCollection(new List<Point>{
                new Point
                {
                    X = -width/2,
                    Y = height/2
                }, new Point
                {
                    X = 0,
                    Y = -height/2
                }, new Point
                {
                    X = width/2,
                    Y = height/2
                }});

            triangle.StrokeThickness = 0.0;
            brush.Color = color;
            triangle.Fill = brush;

            Canvas.SetLeft(triangle, x);
            Canvas.SetTop(triangle, y);

            canvas.Children.Add(triangle);
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            Cnva.Children.Clear();
            Cnva.UpdateLayout();

            this._nbClusterOnScreen = 0;
            this._nbIteration = 0;
            this._points = new List<Point>();
            this._centroid = new List<Point>();            
        }

        private void TbClusterNb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbClusterNb.Text != "")
                if (int.Parse(TbClusterNb.Text) < this._nbClusterOnScreen)
                {
                    MessageBox.Show("It is not possible to enter a cluster number less than the cluster number drawn.", "Invalid input");
                    TbClusterNb.Text = "" + _nbClusterOnScreen;
                }
        }

        private void BtnIterate_Click(object sender, RoutedEventArgs e)
        {
            if (this._nbIteration == 0) //first iteration
            {
                this._kmeans = this.initKmeans();
            }
            if (!this._kmeans.Iteration())
                MessageBox.Show("KMeans finished", "Info");
            this._nbIteration++;
            this.updateCanvas(this._kmeans.DataSet, this._kmeans.Centroids);
        }

        private KMeans initKmeans()
        {
            Instances instances = new Instances();
                        
            // DataSet header
            instances.Attributes = new List<InstanceAttribute>{
                new InstanceAttribute { AttributeType = AttributeType.Numeric }, //x header
                new InstanceAttribute { AttributeType = AttributeType.Numeric }, //y header
                new InstanceAttribute { AttributeType = AttributeType.Nominal }  //class header
                };
            instances.ClassIndex = 2;
                        
            double[,] points = new double[this._points.Count, 2];
            foreach (Point p in this._points)
            {
                Instance instance = new Instance();
                instance.Attributes = new List<InstanceAttribute> {
                    new InstanceAttribute { AttributeType = AttributeType.Numeric, DoubleValue = p.X }, // x
                    new InstanceAttribute { AttributeType = AttributeType.Numeric, DoubleValue = p.Y }, // y
                    new InstanceAttribute { AttributeType = AttributeType.Nominal }
                };

                instances.DataSet.Add(instance);
            }

            // Init centroids
            double maxX = Cnva.ActualWidth;
            double maxY = Cnva.ActualHeight;
            Random random = new Random();
            int nbCluster = int.Parse(TbClusterNb.Text) > this._centroid.Count ?
                int.Parse(TbClusterNb.Text) : this._centroid.Count;
            double[,] centroids = new double[nbCluster, 2];

            for (int i = 0; i < nbCluster; i++)
            {
                if(i < this._centroid.Count)
                {
                    centroids[i,0] = this._centroid[i].X;
                    centroids[i,1] = this._centroid[i].Y;
                }
                else
                {
                    centroids[i,0] = random.NextDouble() * maxX;
                    centroids[i,1] = random.NextDouble() * maxY;
                }
            }

            return new KMeans(instances,centroids);
        }

        private void updateCanvas(Instances instances, Instances centroids)
        {
            foreach (var instance in instances.DataSet)
            {
                this.AddEllipse(
                    Cnva, // canvas
                    instance.Attributes[0].DoubleValue, // x
                    instance.Attributes[1].DoubleValue, // y
                    this._clusterColor[(int)instance.Attributes[2].DoubleValue]); // color
            }

            foreach (var centroid in centroids.DataSet)
            {
                this.AddTriangle(
                    Cnva, // canvas
                    centroid.Attributes[0].DoubleValue, // x
                    centroid.Attributes[1].DoubleValue, // y
                    this._clusterColor[(int)centroid.Attributes[2].DoubleValue]);                    
            }
        }
    }
}
