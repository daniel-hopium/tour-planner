using System.Windows;

namespace TourPlanner.Views
{
    public partial class CitySelectionWindow : Window
    {
        public string SelectedCity { get; private set; }

        public CitySelectionWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedCity = CityNameTextBox.Text;
            DialogResult = true;
        }
    }
}