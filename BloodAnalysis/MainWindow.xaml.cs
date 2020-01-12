using Microsoft.Win32;
using SharedLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace BloodAnalysis
{
    public partial class MainWindow : Window
    {
        Table table;
        string fileName = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            table = new Table();
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
                MainImage.Source = new BitmapImage(new Uri(dialog.FileName));
                fileName = dialog.FileName;
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

        private void GetResult_Click(object sender, RoutedEventArgs e)
        {
            if (MainImage.Source == null) return;

            var image = SourceBitmapConverter.BitmapFromSource(MainImage.Source);
            image = CvProcessor.AdaptiveThreshold(image, 200, 101);
            Bitmap result;
            List<BloodObjects> objects = new List<BloodObjects>();
            ContoursEngine.GetAllObjects(image, out result, out objects, "");
            MainImage.Source = SourceBitmapConverter.ImageSourceFromBitmap(result);
            table.Show();
            table.Activate();
            table.PopulateData(objects, MainImage, fileName);
        }

        private void Table_Click(object sender, RoutedEventArgs e)
        {
            table.Show();
            table.Activate();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            MainImage.Source = null;
            table.Close();
            table = new Table();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder help = new StringBuilder();
            help.Append("This program will help you determine the number of objects of different species on the photo of blood cells");
            help.Append(Environment.NewLine);
            help.Append("Order of work next:");
            help.Append(Environment.NewLine);
            help.Append("1. Load image.");
            help.Append(Environment.NewLine);
            help.Append("2. Press 'Processing -> Get result");
            help.Append(Environment.NewLine);
            help.Append("3. Use 'Table' window for filter");
            help.Append(Environment.NewLine);
            help.Append("4. Save report as .xlsx file.");
            help.Append(Environment.NewLine);
            help.Append(Environment.NewLine);
            help.Append("NOTE: be sure that program can work incorrect if there are many gluded objects.");
            help.Append(Environment.NewLine);
            help.Append(Environment.NewLine);
            help.Append(Environment.NewLine);
            help.Append("Developer: Vladimir Kravchenko, 2019 - 2020\n");
            help.Append("BloodAnalysis.WPF v3.0");
            MessageBox.Show(help.ToString());
            help = null;
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