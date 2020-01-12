using SharedLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace OpenMaps
{
    public partial class Schedule : Window
    {
        class RouteItem
        {
            public int Id { get; set; }
            public System.Drawing.Point Location { get; set; }
            public string Time { get; set; } = "12:00";
            public double Distance { get; set; }
            public double Speed { get; set; }
        }
        List<RouteItem> list = new List<RouteItem>();

        public Schedule(double scale, List<Result> results)//scale in px/m
        {
            InitializeComponent();
            if (scale == 0)
            {
                MessageBox.Show("Scale is equal 0, which is impossible. Please, check whether you have entered correct value!", "Incorrect scale!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            for(int i = 0; i < results.Count; i++)
            {
                var item = new RouteItem
                {
                    Id = i + 1,
                    Location = results[i].CenterPoint
                };
                if (i > 0)
                    item.Distance = GetDistance(results[i - 1].CenterPoint, results[i].CenterPoint, scale);
                list.Add(item);
            }

            MainGrid.ItemsSource = list;
        }

        private double GetDistance(System.Drawing.Point p1, System.Drawing.Point p2, double scale)
        {
            double distPixels = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            return Math.Round(distPixels / scale, 0);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void MainGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Regex rg = new Regex("^[0-1][0-9]:[0-5][0-9]$");
            if (!rg.IsMatch((e.EditingElement as TextBox).Text))
            {
                MessageBox.Show("Please, make sure you entered time in format HH:MM.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetSpeed_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < list.Count; i++)
            {
                string time1 = list[i - 1].Time;
                string time2 = list[i].Time;
                double dist = list[i].Distance;

                double h1 = double.Parse(time1.Substring(0, 2));
                double m1 = double.Parse(time1.Substring(3, 2));
                double h2 = double.Parse(time2.Substring(0, 2));
                double m2 = double.Parse(time2.Substring(3, 2));

                double interval = -1;
                if (h2 == h1)
                {
                    if (m2 > m1)
                    {
                        interval = m2 - m1;
                    }
                }
                else if (h2 > h1)
                {
                    if (m2 > m1)
                    {
                        interval = m2 - m1 + 60 * (h2 - h1);
                    }
                    else if (m2 < m1)
                    {
                        interval = (60 - m1) + m2 + 60 * (h2 - h1 - 1);
                    }
                    else
                    {
                        interval = 60 * (h2 - h1);
                    }
                }

                if (interval > 0)
                {
                    list[i].Speed = Math.Round(list[i].Distance / (interval * 60), 2);
                }
            }

            MainGrid.ItemsSource = null;
            MainGrid.ItemsSource = list;
        }
    }
}