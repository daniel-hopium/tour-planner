using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using TourPlanner.Models;
using TourPlanner.ViewModels.Utils;
using TourPlanner.Views;

namespace TourPlanner.ViewModels
{
    public class WeatherViewModel : INotifyPropertyChanged
    {
        private string _weatherTemperature;
        private string _weatherCondition;
        private string _weatherIconSource;
        private string _selectedCity;

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

        public WeatherViewModel()
        {
            OpenCitySelectionCommand = new RelayCommand(OpenCitySelection);
            SelectedCity = "Vienna"; // Default city
            FetchWeatherData();
        }

        public async void FetchWeatherData()
        {
            try
            {
                string apiKey = "87b1cd01af0e4917aff191346242606"; // Replace with your actual API key
                string url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={SelectedCity}&aqi=no";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        WeatherData weatherData = JsonConvert.DeserializeObject<WeatherData>(responseBody);

                        WeatherTemperature = weatherData?.Current?.TempC != null ? $"{weatherData.Current.TempC}°C" : "N/A";
                        WeatherCondition = weatherData?.Current?.Condition?.Text ?? "N/A";
                        WeatherIconSource = weatherData?.Current?.Condition?.Icon != null ? $"http:{weatherData.Current.Condition.Icon}" : "";
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
                // Handle exceptions
                WeatherTemperature = "N/A";
                WeatherCondition = "N/A";
                WeatherIconSource = "";
            }
        }

        private void OpenCitySelection(object parameter)
        {
            var citySelectionWindow = new CitySelectionWindow();
            if (citySelectionWindow.ShowDialog() == true)
            {
                SelectedCity = citySelectionWindow.SelectedCity;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    
}
