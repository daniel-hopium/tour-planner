using Moq;
using TourPlanner.ViewModels;

namespace TestTourPlanner.ViewModels;

[TestFixture]
public class WeatherViewModelTests
{
  [SetUp]
  public void Setup()
  {
    _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
    _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
    _weatherViewModel = new WeatherViewModel();
  }

  private Mock<HttpMessageHandler> _httpMessageHandlerMock;
  private HttpClient _httpClient;
  private WeatherViewModel _weatherViewModel;

  [Test]
  public void PropertyChangedEvent_ShouldBeTriggered_WhenWeatherTemperatureChanges()
  {
    // Arrange
    var eventTriggered = false;
    _weatherViewModel.PropertyChanged += (sender, args) =>
    {
      if (args.PropertyName == nameof(WeatherViewModel.WeatherTemperature)) eventTriggered = true;
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
      if (args.PropertyName == nameof(WeatherViewModel.WeatherCondition)) eventTriggered = true;
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
      if (args.PropertyName == nameof(WeatherViewModel.SelectedCity)) eventTriggered = true;
    };

    // Act
    _weatherViewModel.SelectedCity = "Paris";

    // Assert
    Assert.IsTrue(eventTriggered);
  }
}