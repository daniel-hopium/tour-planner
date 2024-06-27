using System.ComponentModel;
using System.Windows.Input;

namespace TourPlanner.ViewModels;

public interface IWeatherViewModel : INotifyPropertyChanged
{
  string WeatherTemperature { get; set; }
  string WeatherCondition { get; set; }
  string WeatherIconSource { get; set; }
  string SelectedCity { get; set; }

  ICommand OpenCitySelectionCommand { get; }

  void FetchWeatherData();
}