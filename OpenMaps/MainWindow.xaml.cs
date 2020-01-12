using Microsoft.Win32;
using SharedLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace OpenMaps
{
    public partial class MainWindow : Window
    {
        class ImageHolder
        {
            public int Id { get; set; }
            public System.Windows.Controls.Image ImageControl { get; set; }
            public string FileName { get; set; }
        }

        OpenFileDialog dialog = new OpenFileDialog()
        {
            CheckFileExists = false,
            CheckPathExists = true,
            Multiselect = false,
            Title = "Select map",
            Filter = "Image files|*.jpg;*.jpeg;*.png"
        };
        List<ImageHolder> routeImages = new List<ImageHolder>();
        string map = string.Empty;
        bool setScaleMode = false;
        bool secondPoint = false;
        System.Drawing.Point p1, p2;
        List<Result> results = new List<Result>();
        Schedule scheduleWindow;

        public MainWindow()
        {
            InitializeComponent();

            DrawingSettings.DrawBounds = true;
            DrawingSettings.DrawKeyPoints = true;
            DrawingSettings.DrawRoute = true;
        }

        private void LoadMap_Click(object sender, RoutedEventArgs e)
        {
            if (dialog.ShowDialog() == true)
            {
                MapImage.Source = new BitmapImage(new Uri(dialog.FileName));
                MainTabControl.SelectedIndex = 0;
                map = dialog.FileName;
            }
        }

        private void LoadImages_Click(object sender, RoutedEventArgs e)
        {
            ImagesTabControl.Items.Clear();
            routeImages.Clear();

            dialog.Multiselect = true;
            dialog.Title = "Select route images";
            int i = 0;
            if (dialog.ShowDialog() == true)
            {
                foreach(var file in dialog.FileNames)
                {
                    var tab = new TabItem();
                    tab.Header = Path.GetFileNameWithoutExtension(file);
                    var bitmap = new BitmapImage(new Uri(file));
                    var image = new System.Windows.Controls.Image();
                    image.Source = bitmap;
                    tab.Content = image;
                    ImagesTabControl.Items.Add(tab);
                    routeImages.Add(new ImageHolder { Id = i, FileName = file, ImageControl = image });
                    i++;
                }
                MainTabControl.SelectedIndex = 0;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (MapImage.Source == null) return;

            SaveFileDialog save = new SaveFileDialog
            {
                Filter = "JPG Files (*.jpg)|*.jpg"
            };
            if (save.ShowDialog() == true)
            {
                JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(MapImage.Source as BitmapSource));
                using (FileStream fileStream = new FileStream(save.FileName, FileMode.Create))
                    jpegBitmapEncoder.Save(fileStream);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ImagesTabControl.Items.Clear();
            ResultsTabControl.Items.Clear();
            MapImage.Source = null;
            routeImages.Clear();
            setScaleMode = false;
            secondPoint = false;
            results.Clear();
            scheduleWindow = null;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BuildRoute_Click(object sender, RoutedEventArgs e)
        {
            if (MapImage.Source == null || routeImages.Count < 2) return;

            MainTabControl.Opacity = 0.2;
            Progress.Value = 0;
            Report.Text = $"{0}/{routeImages.Count} images processed";
            FileMenuItem.IsEnabled = false;
            BuildRoute.IsEnabled = false;

            new TaskFactory().StartNew(() =>
            {
                double k = 0;
                foreach (var image in routeImages)
                {
                    results.Add(RouteEngine.Run(new AirPhoto(new Bitmap(map), map), new AirPhoto(new Bitmap(image.FileName), image.FileName)));
                    k++;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Progress.Value = (k / routeImages.Count) * 100;
                        Report.Text = $"{k}/{routeImages.Count} images processed";
                    }));
                }

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    FileMenuItem.IsEnabled = true;
                    BuildRoute.IsEnabled = true;
                    MainTabControl.Opacity = 1;
                    for (int i = 0; i < results.Count; i++)
                    {
                        var bitmap = results[i].RAWmatchestoBitmap();
                        var image = new System.Windows.Controls.Image
                        {
                            Source = SourceBitmapConverter.ImageSourceFromBitmap(bitmap)
                        };
                        var tab = new TabItem
                        {
                            Header = Path.GetFileNameWithoutExtension(results[i].SingleImage.FileName),
                            Content = image
                        };
                        ResultsTabControl.Items.Add(tab);
                        
                        results[i] = RouteEngine.FilterPoints(results[i]);
                        results[i].Id = i + 1;
                        routeImages.ElementAt(i).ImageControl.Source = SourceBitmapConverter.ImageSourceFromBitmap(results[i].SingleImage.AfterClusterizationtoBitmap());
                    }
                    if (DrawingSettings.DrawKeyPoints)
                    {
                        MapImage.Source = SourceBitmapConverter.ImageSourceFromBitmap(RouteEngine.DrawKPAtMap(SourceBitmapConverter.BitmapFromSource(MapImage.Source), results));
                    }
                    if (DrawingSettings.DrawBounds)
                    {
                        MapImage.Source = SourceBitmapConverter.ImageSourceFromBitmap(RouteEngine.DrawBounds(SourceBitmapConverter.BitmapFromSource(MapImage.Source), results));
                    }
                    if (DrawingSettings.DrawRoute)
                    {
                        MapImage.Source = SourceBitmapConverter.ImageSourceFromBitmap(RouteEngine.BuildRoute(SourceBitmapConverter.BitmapFromSource(MapImage.Source), results));
                    }
                }));
            });
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder help = new StringBuilder();
            help.Append("This program allows you to build a route of UAV based on one photo from big hight and few photos from lower high.");
            help.Append(Environment.NewLine);
            help.Append("Order of work next:");
            help.Append(Environment.NewLine);
            help.Append("1. Load main map and set of local images.");
            help.Append(Environment.NewLine);
            help.Append("2. Build a route. This may take a lot (I mean LOT) of time.");
            help.Append(Environment.NewLine);
            help.Append("3. Save result.");
            help.Append(Environment.NewLine);
            help.Append(Environment.NewLine);
            help.Append(Environment.NewLine);
            help.Append("Developer: Vladimir Kravchenko, 2019 - 2020\n");
            help.Append("OpenMaps.WPF v3.0");
            MessageBox.Show(help.ToString(), "OpenMaps.WPF");
            help = null;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            App.Current.Shutdown();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsDialog().ShowDialog();
        }

        private void SetScale_Click(object sender, RoutedEventArgs e)
        {
            if (results.Count == 0 || MapImage.Source == null) return;

            if (setScaleMode)
            {
                scheduleWindow?.Show();
            }
            else
            {
                MessageBox.Show("Now please select two points at the map and enter real distance between them.", "Points selection mode", MessageBoxButton.OK, MessageBoxImage.Information);
                setScaleMode = true;
            }
        }

        private void MapImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (results.Count == 0) return;

            var coords = GetCoordsRelatedToImage(e);

            if (secondPoint)
            {
                p2 = coords;

                double distPixels = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
                double distReal = 0;

                var dialog = new InputBox();
                if (dialog.ShowDialog() == true)
                {
                    distReal = double.Parse(dialog.Distance);
                    scheduleWindow = new Schedule(distPixels / distReal, results);
                    scheduleWindow.Show();
                }
            }
            else
            {
                p1 = coords;
                secondPoint = true;
            }
        }

        private System.Drawing.Point GetCoordsRelatedToImage(MouseButtonEventArgs e)
        {
            var controlSpacePosition = e.GetPosition(MapImage);
            var image = new Bitmap(map);

            var x = (int)Math.Floor(controlSpacePosition.X * image.Width / MapImage.ActualWidth);
            var y = (int)Math.Floor(controlSpacePosition.Y * image.Height / MapImage.ActualHeight);

            return new System.Drawing.Point(x, y);
        }
    }
}