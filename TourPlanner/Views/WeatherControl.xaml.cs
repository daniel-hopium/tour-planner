using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.ViewModels;

namespace TourPlanner.Views;

public partial class WeatherUserControl : UserControl
{
  public WeatherUserControl()
  {
    InitializeComponent();
  }

  private void WeatherInfo_Click(object sender, MouseButtonEventArgs e)
  {
    if (DataContext is WeatherViewModel viewModel) viewModel.OpenCitySelectionCommand.Execute(null);
  }
}