using Microsoft.Win32;
using SharedLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeafArea
{
    public partial class Table : Window
    {
        List<Leaf> leafs = new List<Leaf>();

        public Table()
        {
            InitializeComponent();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if (leafs.Count == 0) return;

            SaveFileDialog save = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv"
            };
            if (save.ShowDialog() == true)
            {
                try
                {
                    MainGrid.SelectAllCells();
                    MainGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                    ApplicationCommands.Copy.Execute(null, MainGrid);
                    string resultat = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                    string result = (string)Clipboard.GetData(DataFormats.Text);
                    MainGrid.UnselectAllCells();
                    System.IO.StreamWriter file1 = new System.IO.StreamWriter(save.FileName);
                    file1.WriteLine(result);
                    file1.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to export data. Details: " + ex.Message, "Export error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        public void PopulateData(List<ComplexObject> allItems, ComplexObject etalon, double realArea)
        {
            leafs.Clear();
            foreach(var obj in allItems)
            {
                Leaf leaf = new Leaf(obj.Id, (obj.Area / etalon.Area) * realArea, obj.Area, obj.Area / etalon.Area);
                leafs.Add(leaf);
            }
            MainGrid.ItemsSource = leafs;
        }

        public void AddItem(double area, double realArea, ComplexObject etalon, int id)
        {
            Leaf leaf = new Leaf(id, (area / etalon.Area) * realArea, area, area / etalon.Area);
            leafs.Add(leaf);

            MainGrid.ItemsSource = null;
            MainGrid.ItemsSource = leafs;
        }

        class Leaf
        {
            public int Id { get; set; }
            public double Area { get; set; }
            public double AreaP { get; set; }
            public double Percent { get; set; }

            public Leaf(int id, double area, double areap, double percent)
            {
                Id = id;
                Area = Math.Round(area, 2);
                AreaP = Math.Round(areap, 0);
                Percent = Math.Round(percent, 2) * 100;
            }
        }
    }
}