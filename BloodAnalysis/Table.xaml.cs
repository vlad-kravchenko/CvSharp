using SharedLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BloodAnalysis
{
    public partial class Table : Window
    {
        List<BloodObjects> allObjects = new List<BloodObjects>();
        System.Windows.Controls.Image mainImage;
        Bitmap all;
        string fileName = string.Empty;

        public Table()
        {
            InitializeComponent();
            foreach (var item in Enum.GetValues(typeof(Group)))
            {
                Filter.Items.Add(item);
            }
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Filter.SelectedItem == null || (Group)Filter.SelectedItem == Group.All)
            {
                MainGrid.ItemsSource = null;
                MainGrid.ItemsSource = allObjects;
                mainImage.Source = SourceBitmapConverter.ImageSourceFromBitmap(all);
            }
            else
            {
                MainGrid.ItemsSource = allObjects.Where(a => a.Group == (Group)Filter.SelectedItem);
                var result = ContoursEngine.DrawObjectsOnImage(new Bitmap(fileName), allObjects.Where(a => a.Group == (Group)Filter.SelectedItem).ToList());
                mainImage.Source = SourceBitmapConverter.ImageSourceFromBitmap(result);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        public void PopulateData(List<BloodObjects> objects, System.Windows.Controls.Image img, string fileName)
        {
            allObjects = objects;
            MainGrid.ItemsSource = allObjects;
            mainImage = img;
            this.fileName = fileName;
            all = SourceBitmapConverter.BitmapFromSource(mainImage.Source);
            RecountSummary();
        }

        private void RecountSummary()
        {
            List<Summary> summaryItems = new List<Summary>();
            summaryItems.Add(new Summary(Group.Interest, allObjects.Count(e => e.Group == Group.Interest)));
            summaryItems.Add(new Summary(Group.Small, allObjects.Count(e => e.Group == Group.Small)));
            summaryItems.Add(new Summary(Group.Unknown, allObjects.Count(e => e.Group == Group.Unknown)));
            SummaryGrid.ItemsSource = summaryItems;
        }

        class Summary
        {
            public Group Group { get; set; }
            public int Count { get; set; }

            public Summary(Group group, int count)
            {
                Group = group;
                Count = count;
            }
        }
    }
}