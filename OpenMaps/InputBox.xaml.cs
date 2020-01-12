using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace OpenMaps
{
    public partial class InputBox : Window
    {
        public InputBox()
        {
            InitializeComponent();
            Area.Focus();
        }

        public string Distance { get { return Area.Text; } }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Area_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var regex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");
            if (regex.IsMatch(e.Text) && !(e.Text == "." && ((TextBox)sender).Text.Contains(e.Text)))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void Area_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && !string.IsNullOrEmpty(Area.Text))
                OK_Click(null, null);
        }
    }
}