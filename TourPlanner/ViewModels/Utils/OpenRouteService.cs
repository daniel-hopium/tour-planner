using System;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TourPlanner.Exceptions;
using TourPlanner.Models;

namespace TourPlanner.ViewModels.Utils;

public class Tile
{
  public Tile(int x, int y)
  {
    X = x;
    Y = y;
  }

  public int X { get; }
  public int Y { get; }
}

public class OpenRouteService : IOpenRouteService
{
  private static readonly HttpClient client = new();

  private static readonly string? _api_key = ConfigurationManager.AppSettings["OpenRouteApiKey"];

  private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

  public async Task<(double[] coordinates, bool success)> GetParametersFromApi(string text)
  {
    try
    {
      var apiUrl =
        $"https://api.openrouteservice.org/geocode/search?api_key={_api_key}&text={Uri.EscapeDataString(text)}";

      var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
      var response = await client.SendAsync(request);
      response.EnsureSuccessStatusCode();

      var responseBody = await response.Content.ReadAsStringAsync();
      dynamic? jsonResponse = JsonConvert.DeserializeObject(responseBody);

      if (jsonResponse == null)
      {
        double[] coordinat = { 0.0, 0.0 };
        return (coordinat, false);
      }

      var coordinates = ((JArray)jsonResponse.features.First.geometry.coordinates).Select(jv => (double)jv)
        .ToArray();

      return (coordinates, true);
    }
    catch (Exception ex)
    {
      log.Error($"Could not GetParametersFromApi: {ex}");
      throw new UtilsException("Error in OpenRouteService.GetParametersFromApi", ex);
    }
  }

  public async Task<Bitmap> GetTileAsync(int zoom, double x_tile, double y_tile)
  {
    try
    {
      var tileUrl = $"https://tile.openstreetmap.org/{zoom}/{x_tile}/{y_tile}.png";

      var request = new HttpRequestMessage(HttpMethod.Get, tileUrl);
      request.Headers.Add("User-Agent", "TourPlanner/1.0");

      var response = await client.SendAsync(request);
      if (response.IsSuccessStatusCode)
      {
        using var responseStream = await response.Content.ReadAsStreamAsync();
        {
          return new Bitmap(responseStream);
        }
      }

      return CreateDefaultBitmap();
    }
    catch (Exception ex)
    {
      log.Error($"Could not GetTilesFromApi: {ex}");
      throw new UtilsException("Error in OpenRouteService.GetTileAsync", ex);
    }
  }

  public async Task<(double[][] coordinates, double[] bbox, double distance, double duration)> GetDirectionsFromApi(
    TransportType transportType, string start, string end)
  {
    try
    {
      var transport = string.Empty;
      switch (transportType)
      {
        case TransportType.bike:
          transport = "cycling-regular";
          break;
        case TransportType.hike:
          transport = "foot-hiking";
          break;
        case TransportType.running:
          transport = "foot-walking";
          break;
        case TransportType.car:
          transport = "driving-car";
          break;
      }

      var apiUrl =
        $"https://api.openrouteservice.org/v2/directions/{transport}?api_key={_api_key}&start={Uri.EscapeDataString(start)}&end={Uri.EscapeDataString(end)}";

      var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
      var response = await client.SendAsync(request);
      response.EnsureSuccessStatusCode();

      var responseBody = await response.Content.ReadAsStringAsync();
      dynamic? jsonResponse = JsonConvert.DeserializeObject(responseBody);

      if (jsonResponse != null)
      {
        var coordinates = ((JArray)jsonResponse.features.First.geometry.coordinates)
          .Select(jv => jv.Select(coord => (double)coord).ToArray()).ToArray();
        var bbox = ((JArray)jsonResponse.bbox).Select(jv => (double)jv).ToArray();
        double distance = jsonResponse.features.First.properties.segments.First.distance;
        double duration = jsonResponse.features.First.properties.segments.First.duration;
        return (coordinates, bbox, distance, duration);
      }

      throw new InvalidOperationException("No route found");
    }
    catch (Exception ex)
    {
      log.Error($"Could not GetDirectionsFromApi: {ex}");
      throw new UtilsException("Error in OpenRouteService.GetDirectionsFromApi", ex);
    }
  }

  private static Bitmap CreateDefaultBitmap()
  {
    try
    {
      var width = 256;
      var height = 256;
      Bitmap defaultBitmap = new(width, height);
      using (var g = Graphics.FromImage(defaultBitmap))
      {
        g.Clear(Color.Gray); // Set background color to gray
        g.DrawString("No Image", new Font("Arial", 16), Brushes.Red, new PointF(50, 100));
      }

      return defaultBitmap;
    }
    catch (Exception ex)
    {
      throw new UtilsException("Error in OpenRouteService.CreateDefaultBitmap", ex);
    }
  }
}