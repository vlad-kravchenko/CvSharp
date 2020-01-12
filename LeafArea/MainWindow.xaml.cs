using Microsoft.Win32;
using SharedLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LeafArea
{
    public partial class MainWindow : Window
    {
        Table table = new Table();
        string fileName = string.Empty;
        bool isEtalon = true;
        ComplexObject etalonObject;
        double realArea = 0;
        int global_id = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                CheckFileExists = false,
                CheckPathExists = true,
                Multiselect = false,
                Title = "Select image",
                Filter = "Image files|*.jpg;*.jpeg;*.png"
            };
            if (dialog.ShowDialog() == true)
            {
                Clear_Click(null, null);
                fileName = dialog.FileName;
                MainImage.Source = new BitmapImage(new Uri(dialog.FileName));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (MainImage.Source == null) return;

            SaveFileDialog save = new SaveFileDialog
            {
                Filter = "JPG Files (*.jpg)|*.jpg"
            };
            if (save.ShowDialog() == true)
            {
                JpegBitmapEncoder jpegBitmapEncoder = new JpegBitmapEncoder();
                jpegBitmapEncoder.Frames.Add(BitmapFrame.Create(MainImage.Source as BitmapSource));
                using (FileStream fileStream = new FileStream(save.FileName, FileMode.Create))
                    jpegBitmapEncoder.Save(fileStream);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            MainImage.Source = null;
            fileName = string.Empty;
            table.Close();
            table = new Table();
            InteractiveMode.IsChecked = false;
            isEtalon = true;
            etalonObject = null;
            realArea = 0;
            OneByOne.IsEnabled = true;
            InteractiveMode.IsEnabled = true;
            OneByOne.IsChecked = false;
            InteractiveMode.IsChecked = false;
            global_id = 0;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Table_Click(object sender, RoutedEventArgs e)
        {
            table.Show();
            table.Activate();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder help = new StringBuilder();
            help.Append("This program will allow you to determine the area of objects of complex shapes.");
            help.Append(Environment.NewLine);
            help.Append("Order of work next:");
            help.Append(Environment.NewLine);
            help.Append("1. Load image.");
            help.Append(Environment.NewLine);
            help.Append("2. Click on the etalon object on the photo and enter its real area.");
            help.Append(Environment.NewLine);
            help.Append("3. View table with results.");
            help.Append(Environment.NewLine);
            help.Append("4. Save report as .xlsx file.");
            help.Append(Environment.NewLine);
            help.Append(Environment.NewLine);
            help.Append(Environment.NewLine);
            help.Append("Developer: Vladimir Kravchenko, 2019 - 2020\n");
            help.Append("LeafArea.WPF v3.0");
            MessageBox.Show(help.ToString());
            help = null;
        }

        private void MainImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (InteractiveMode.IsChecked != true) return;
            var coords = GetCoordsRelatedToImage(e);
            if (coords.X < 0 || coords.Y < 0) return;

            if (isEtalon)
            {
                var dialog = new InputBox();
                if (dialog.ShowDialog() == true)
                {
                    realArea = double.Parse(dialog.LeafArea);
                    Bitmap result;
                    List<ComplexObject> allItems = new List<ComplexObject>();
                    ContoursEngine.GetAllObjects(SourceBitmapConverter.BitmapFromSource(MainImage.Source), coords, out result, out etalonObject, out allItems);
                    if (OneByOne.IsChecked != true)
                    {
                        MainImage.Source = SourceBitmapConverter.ImageSourceFromBitmap(result);
                        if (allItems == null) return;
                        table.PopulateData(allItems, etalonObject, realArea);
                        OneByOne.IsEnabled = false;
                        OneByOne.IsChecked = false;
                    }
                    else
                    {
                        double area = 0;
                        ContoursEngine.GetSingleContour(SourceBitmapConverter.BitmapFromSource(MainImage.Source), coords, global_id, SourceBitmapConverter.BitmapFromSource(MainImage.Source), out result, out area);
                        MainImage.Source = SourceBitmapConverter.ImageSourceFromBitmap(result);
                        global_id++;
                    }
                    isEtalon = false;
                    table.Show();
                    table.Activate();
                }
            }
            else
            {
                if (OneByOne.IsChecked == true)
                {
                    Bitmap result;
                    double area = 0;
                    ContoursEngine.GetSingleContour(SourceBitmapConverter.BitmapFromSource(MainImage.Source), coords, global_id, SourceBitmapConverter.BitmapFromSource(MainImage.Source), out result, out area);
                    MainImage.Source = SourceBitmapConverter.ImageSourceFromBitmap(result);
                    if (area != 0)
                    {
                        table.AddItem(area, realArea, etalonObject, global_id);
                        global_id++;
                    }
                    OneByOne.IsEnabled = false;
                    OneByOne.IsChecked = true;
                }
            }         
        }

        private System.Drawing.Point GetCoordsRelatedToImage(MouseButtonEventArgs e)
        {
            var controlSpacePosition = e.GetPosition(MainImage);
            var image = new Bitmap(fileName);

            var x = (int)Math.Floor(controlSpacePosition.X * image.Width / MainImage.ActualWidth);
            var y = (int)Math.Floor(controlSpacePosition.Y * image.Height / MainImage.ActualHeight);

            return new System.Drawing.Point(x, y);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            App.Current.Shutdown();
        }

        private void Process_Click(object sender, RoutedEventArgs e)
        {
            if (MainImage.Source == null) return;

            Processing processing = new Processing(MainImage);
            processing.Show();
        }
    }
}