using System.Windows;

namespace TourPlanner.Views;

public partial class CitySelectionWindow : Window
{
  public CitySelectionWindow()
  {
    InitializeComponent();
  }

  public string SelectedCity { get; private set; }

  private void OkButton_Click(object sender, RoutedEventArgs e)
  {
    SelectedCity = CityNameTextBox.Text;
    DialogResult = true;
  }
}