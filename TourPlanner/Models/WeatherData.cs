using Newtonsoft.Json;

namespace TourPlanner.Models;

public class WeatherData
{
  [JsonProperty("current")] public CurrentWeather Current { get; set; }
}

public class CurrentWeather
{
  [JsonProperty("temp_c")] public float TempC { get; set; }

  [JsonProperty("condition")] public Condition Condition { get; set; }
}

public class Condition
{
  [JsonProperty("text")] public string Text { get; set; }

  [JsonProperty("icon")] public string Icon { get; set; }
}