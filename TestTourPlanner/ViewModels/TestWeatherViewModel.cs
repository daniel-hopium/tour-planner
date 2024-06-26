using NUnit.Framework;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TourPlanner.ViewModels;
using Newtonsoft.Json;
using TourPlanner.Models;
using TourPlanner.Views;

namespace TestTourPlanner.ViewModels
{
    [TestFixture]
    public class WeatherViewModelTests
    {
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;
        private WeatherViewModel _weatherViewModel;

        [SetUp]
        public void Setup()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _weatherViewModel = new WeatherViewModel();
        }

        [Test]
        public void PropertyChangedEvent_ShouldBeTriggered_WhenWeatherTemperatureChanges()
        {
            // Arrange
            var eventTriggered = false;
            _weatherViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(WeatherViewModel.WeatherTemperature))
                {
                    eventTriggered = true;
                }
            };

            // Act
            _weatherViewModel.WeatherTemperature = "30.0°C";

            // Assert
            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void PropertyChangedEvent_ShouldBeTriggered_WhenWeatherConditionChanges()
        {
            // Arrange
            var eventTriggered = false;
            _weatherViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(WeatherViewModel.WeatherCondition))
                {
                    eventTriggered = true;
                }
            };

            // Act
            _weatherViewModel.WeatherCondition = "Rainy";

            // Assert
            Assert.IsTrue(eventTriggered);
        }

        [Test]
        public void PropertyChangedEvent_ShouldBeTriggered_WhenSelectedCityChanges()
        {
            // Arrange
            var eventTriggered = false;
            _weatherViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(WeatherViewModel.SelectedCity))
                {
                    eventTriggered = true;
                }
            };

            // Act
            _weatherViewModel.SelectedCity = "Paris";

            // Assert
            Assert.IsTrue(eventTriggered);
        }
    }
}
