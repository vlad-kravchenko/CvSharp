using System.Windows;

namespace OpenMaps
{
    public partial class SettingsDialog : Window
    {
        public SettingsDialog()
        {
            InitializeComponent();

            cbDrawBounds.IsChecked = DrawingSettings.DrawBounds;
            cbDrawKeyPoints.IsChecked = DrawingSettings.DrawKeyPoints;
            cbDrawRoute.IsChecked = DrawingSettings.DrawRoute;

            OK.Focus();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DrawingSettings.DrawBounds = cbDrawBounds.IsChecked == true;
            DrawingSettings.DrawKeyPoints = cbDrawKeyPoints.IsChecked == true;
            DrawingSettings.DrawRoute = cbDrawRoute.IsChecked == true;
            Close();
        }
    }
}