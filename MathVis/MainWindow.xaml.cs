using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Timer = System.Timers.Timer;

namespace MathVis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const byte N = 1 << 0;
        private const byte S = 1 << 1;
        private const byte W = 1 << 2;
        private const byte E = 1 << 3;
        private const double DISTANCE_INCREMENT = .5;
        private int _imageHeight;
        private int _imageWidth;

        private int _timerTickCount;

        private volatile WriteableBitmap _writeableBitmap;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Methods

        private void Init()
        {
            // Init WriteableBitmap
            _writeableBitmap = BitmapFactory.New((int) Image.Width, (int) Image.Height);
            Image.Source = _writeableBitmap;

            _imageWidth = _writeableBitmap.PixelWidth;
            _imageHeight = _writeableBitmap.PixelHeight;
        }

        private void DrawRandomTraversal()
        {
            // https://bl.ocks.org/mbostock/70a28267db0354261476
            throw new NotImplementedException();
        }

        private void FillRandomTraversal()
        {
            int width = _imageWidth;
            int height = _imageHeight;

            var mazeGen = new RandomTraversal(width, height);
            byte[] cells = mazeGen.GenerateMaze();
            double distance = 0;
            // make each new wave start with different color
            distance += _timerTickCount * 222.5;
            var visited = new bool[width * height];
            // first cell is in bottom left - see RandomTraversal.cs
            int startIndex = (height - 1) * width;
            int[] frontier = {startIndex};

            // flood color
            while (true)
            {
                // frontier for this iteration
                var frontier1 = new List<int>();

                // get the color for this iteration's distance
                Color color = MapDistToColor(distance);
                // increment distance for next iteration
                distance += DISTANCE_INCREMENT;

                // Use dispatcher for multi-threading
                // thread per traversal
                int[] pixels = frontier;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // color in the frontier
                    foreach (int pixel in pixels)
                    {
                        // get pixel x and y from number
                        int x = pixel % width;
                        int y = pixel / width;
                        _writeableBitmap.SetPixel(x, y, color);
                        // DrawPixel(x, y, color);
                    }
                });

                // add delay so you can see it render each frontier
                // 60 frontiers per second   
                Thread.Sleep(1000 / 60);
                // update the image
                Image.ForceRedraw();

                // explore the frontier
                foreach (int pixel in frontier)
                {
                    // store current cell here
                    int i0 = pixel;
                    // set next cell here    
                    int i1;

                    // if the cell has the bit set for that direction
                    if ((cells[i0] & E) > 0)
                    {
                        i1 = i0 + 1;

                        // if the cell has not been visited already
                        if (!visited[i1])
                        {
                            // add the cell in that direction to the frontier and mark as visited
                            visited[i1] = true;
                            frontier1.Add(i1);
                        }
                    }

                    if ((cells[i0] & W) > 0)
                    {
                        i1 = i0 - 1;

                        if (!visited[i1])
                        {
                            visited[i1] = true;
                            frontier1.Add(i1);
                        }
                    }

                    if ((cells[i0] & S) > 0)
                    {
                        i1 = i0 + width;

                        if (!visited[i1])
                        {
                            visited[i1] = true;
                            frontier1.Add(i1);
                        }
                    }

                    if ((cells[i0] & N) > 0)
                    {
                        i1 = i0 - width;

                        if (!visited[i1])
                        {
                            visited[i1] = true;
                            frontier1.Add(i1);
                        }
                    }
                }

                frontier = frontier1.ToArray();

                // leave loop when no more frontier
                if (frontier1.Count == 0)
                {
                    break;
                }
            }
        }

        private void LoopTraversal()
        {
            // 5 sec interval
            var timer = new Timer {Interval = 5000};
            timer.Elapsed += OnTimedEvent;
            timer.Start();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            _timerTickCount++;
            FillRandomTraversal();
        }

        private static Color MapDistToColor(double distance)
        {
            double hue360 = distance % 360;
            double hue = hue360 / 360;
            return ColorRGB.FromHSL(hue, 1.0, 0.5);
        }

        #endregion

        #region Eventhandler

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            // clear the bitmap
            using (_writeableBitmap.GetBitmapContext())
            {
                _writeableBitmap.Clear();
            }

            LoopTraversal();
            //FillRandomTraversal();
            //DrawRandomTraversal();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear 
            _writeableBitmap.Clear();
        }

        #endregion
    }
}