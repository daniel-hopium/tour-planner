using System;
using System.ComponentModel;
using System.Configuration;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Newtonsoft.Json;
using TourPlanner.Models;
using TourPlanner.ViewModels.Utils;
using TourPlanner.Views;

namespace TourPlanner.ViewModels;

public class WeatherViewModel : IWeatherViewModel
{
  private string _selectedCity;
  private string _weatherCondition;
  private string _weatherIconSource;
  private string _weatherTemperature;

  public WeatherViewModel()
  {
    OpenCitySelectionCommand = new RelayCommand(OpenCitySelection);
    SelectedCity = "Vienna"; // Default city
    FetchWeatherData();
  }

  public string WeatherTemperature
  {
    get => _weatherTemperature;
    set
    {
      _weatherTemperature = value;
      OnPropertyChanged();
    }
  }

  public string WeatherCondition
  {
    get => _weatherCondition;
    set
    {
      _weatherCondition = value;
      OnPropertyChanged();
    }
  }

  public string WeatherIconSource
  {
    get => _weatherIconSource;
    set
    {
      _weatherIconSource = value;
      OnPropertyChanged();
    }
  }

  public string SelectedCity
  {
    get => _selectedCity;
    set
    {
      _selectedCity = value;
      OnPropertyChanged();
      FetchWeatherData();
    }
  }

  public ICommand OpenCitySelectionCommand { get; }

  public async void FetchWeatherData()
  {
    try
    {
      var apiKey = ConfigurationManager.AppSettings["WeatherApiKey"] ?? throw new InvalidOperationException();
      var url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={SelectedCity}&aqi=no";

      using (var client = new HttpClient())
      {
        var response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
          var responseBody = await response.Content.ReadAsStringAsync();
          var weatherData = JsonConvert.DeserializeObject<WeatherData>(responseBody);

          WeatherTemperature = weatherData?.Current?.TempC != null ? $"{weatherData.Current.TempC}°C" : "N/A";
          WeatherCondition = weatherData?.Current?.Condition?.Text ?? "N/A";
          WeatherIconSource = weatherData?.Current?.Condition?.Icon != null
            ? $"http:{weatherData.Current.Condition.Icon}"
            : "";
        }
        else
        {
          // If the status code is not successful, set default values
          WeatherTemperature = "N/A";
          WeatherCondition = "N/A";
          WeatherIconSource = "";
        }
      }
    }
    catch (Exception ex)
    {
      WeatherTemperature = "N/A";
      WeatherCondition = "N/A";
      WeatherIconSource = "";
    }
  }

  public event PropertyChangedEventHandler PropertyChanged;

  private void OpenCitySelection(object parameter)
  {
    var citySelectionWindow = new CitySelectionWindow();
    if (citySelectionWindow.ShowDialog() == true) SelectedCity = citySelectionWindow.SelectedCity;
  }

  protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}