using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using log4net;
using TourPlanner.Config;
using TourPlanner.Exceptions;
using TourPlanner.Models;

namespace TourPlanner.ViewModels.Utils;

public class MapCreator : IMapCreator
{
  public enum Marker
  {
    PIN_RED_16px,
    PIN_RED_32px,
    MARKER_RED_16px,
    MARKER_RED_32px
  }

  private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

  private readonly IOpenRouteService _openRouteService;

  public MapCreator(IOpenRouteService openRouteService)
  {
    _openRouteService = openRouteService;
  }

  public static int Zoom { get; set; } = 18;


  public async Task<Bitmap> GenerateImageAsync(double[][] coordinates, double[] bbox, double[] coordinatesStart,
    double[] coordinatesEnd)
  {
    try
    {
      List<GeoCoordinate> markers = new();

      GeoCoordinate startMarker = new(coordinatesStart[0], coordinatesStart[1]);
      GeoCoordinate endMarker = new(coordinatesEnd[0], coordinatesEnd[1]);
      markers.Add(startMarker);
      markers.Add(endMarker);

      var zoom = Zoom;

      const int MaxBitmapWidth = 3624;
      const int MaxBitmapHeight = 2048;

      var aspectRatio = 16.0 / 9.0;
      var currentAspectRatio = (bbox[2] - bbox[0]) / (bbox[3] - bbox[1]);

      double mal;
      double var3;
      double z;

      // Calculate the tile numbers for each corner of the bounding box
      var topLeftTile = Latlon2Tile(bbox[3], bbox[0], Zoom);
      var bottomRightTile = Latlon2Tile(bbox[1], bbox[2], Zoom);

      // Determine the number of tiles to fetch in each dimension
      var tilesX = bottomRightTile.X - topLeftTile.X + 1;
      var tilesY = bottomRightTile.Y - topLeftTile.Y + 1;

      // of bitmap
      int currentWidth;
      int currentHeight;


      if (currentAspectRatio > aspectRatio)
      {
        mal = (bbox[2] - bbox[0]) / aspectRatio / (bbox[3] - bbox[1]);
        var3 = (bbox[3] - bbox[1]) * mal;
        z = var3 - (bbox[3] - bbox[1]);

        while (true)
        {
          // Calculate tile-coordinates for current zoom-level
          topLeftTile = Latlon2Tile(bbox[3] + z / 4, bbox[0], zoom);
          bottomRightTile = Latlon2Tile(bbox[1] - z / 2, bbox[2], zoom);

          // Calculate number of tiles in x-/Y-dirction
          tilesX = bottomRightTile.X - topLeftTile.X + 1;
          tilesY = bottomRightTile.Y - topLeftTile.Y + 1;

          currentWidth = tilesX * 256;
          currentHeight = tilesY * 256;


          if (currentWidth <= MaxBitmapWidth && currentHeight <= MaxBitmapHeight) break;

          zoom--;

          if (zoom < 0) throw new InvalidOperationException("Zoom level cannot be reduced further.");
        }
      }
      else if (currentAspectRatio < aspectRatio)
      {
        mal = aspectRatio * (bbox[3] - bbox[1]) / (bbox[2] - bbox[0]);
        var3 = (bbox[2] - bbox[0]) * mal;
        z = var3 - (bbox[2] - bbox[0]);

        while (true)
        {
          topLeftTile = Latlon2Tile(bbox[3], bbox[0] - z / 2, zoom);
          bottomRightTile = Latlon2Tile(bbox[1], bbox[2] + z, zoom);

          tilesX = bottomRightTile.X - topLeftTile.X + 1;
          tilesY = bottomRightTile.Y - topLeftTile.Y + 1;

          currentWidth = tilesX * 256;
          currentHeight = tilesY * 256;


          if (currentWidth <= MaxBitmapWidth && currentHeight <= MaxBitmapHeight) break;

          zoom--;

          if (zoom < 0) throw new InvalidOperationException("Zoom level cannot be reduced further.");
        }
      }

      Point topLeftTilePixel = new(topLeftTile.X * 256, topLeftTile.Y * 256);

      List<Point> routePoints = new();

      foreach (var coord in coordinates)
      {
        var lon = coord[0];
        var lat = coord[1];

        var globalPos = LatLonToPixel(lat, lon, zoom);
        Point relativePos = new(globalPos.X - topLeftTilePixel.X, globalPos.Y - topLeftTilePixel.Y);
        routePoints.Add(new Point(relativePos.X, relativePos.Y));
      }

      Bitmap map = new(tilesX * 256, tilesY * 256, PixelFormat.Format32bppArgb);

      using (var g = Graphics.FromImage(map))
      {
        // Fetch and draw each tile
        for (var x = topLeftTile.X; x <= bottomRightTile.X; x++)
        for (var y = topLeftTile.Y; y <= bottomRightTile.Y; y++)
        {
          var tileImage = await _openRouteService.GetTileAsync(zoom, x, y);
          var xPos = (x - topLeftTile.X) * 256;
          var yPos = (y - topLeftTile.Y) * 256;
          g.DrawImage(tileImage, xPos, yPos);
          tileImage.Dispose();
        }


        // Draw route
        if (routePoints.Count > 1)
        {
          using Pen redPen = new(Color.Red) { Width = 7 };

          g.DrawLines(redPen, routePoints.ToArray());
          redPen.Dispose();
        }


        // Draw markers
        foreach (var marker in markers)
        {
          Bitmap? markerIcon;
          if (marker == startMarker)
            markerIcon = LoadMarkerIcon(Marker.MARKER_RED_16px);
          else
            markerIcon = LoadMarkerIcon(Marker.MARKER_RED_32px);
          var globalPos = LatLonToPixel(marker.Lat, marker.Lon, zoom);
          Point relativePos = new(globalPos.X - topLeftTilePixel.X, globalPos.Y - topLeftTilePixel.Y);
          g.DrawImage(markerIcon, relativePos);
          markerIcon.Dispose();
        }
      }

      return map;
    }
    catch (Exception ex)
    {
      log.Error($"Error during Map-Image genration: {ex}");
      throw new UtilsException("Error in MapCreator.GenerateImageAsync", ex);
    }
  }

  public void SaveMap(Bitmap map, string transport, string start, string end)
  {
    try
    {
      var fileName = $"{ConfigurationHelper.ImagesDirectory}{transport}_{start}_{end}.png";

      if (map != null && !File.Exists(fileName)) map.Save(fileName, ImageFormat.Png);
    }
    catch (Exception ex)
    {
      log.Error($"Map could not be saved: {ex}");
      throw new UtilsException("Error in MapCreator.SaveMap", ex);
    }
  }

  public async Task DownloadMapFromApi(TransportType transportType, double[] start, double[] end)
  {
    try
    {
      if (!File.Exists($"{ConfigurationHelper.ImagesDirectory}{transportType}_{start}_{end}.png"))
      {
        var (coordinates, bbox, _, _) = await _openRouteService.GetDirectionsFromApi(transportType,
          $"{start[0].ToString("0.######", CultureInfo.InvariantCulture)},{start[1].ToString("0.######", CultureInfo.InvariantCulture)}",
          $"{end[0].ToString("0.######", CultureInfo.InvariantCulture)},{end[1].ToString("0.######", CultureInfo.InvariantCulture)}");

        SaveMap(await GenerateImageAsync(coordinates, bbox, start, end), transportType.ToString(),
          $"{start[0]}_{start[1]}", $"{end[0]}_{end[1]}");
      }
    }
    catch (Exception ex)
    {
      log.Error($"Map could not be downloaded: {ex}");
      throw new UtilsException("Error in MapCreator.DownloadMapFromApi", ex);
    }
  }

  public BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
  {
    try
    {
      using MemoryStream memory = new();
      {
        bitmap.Save(memory, ImageFormat.Png);
        memory.Position = 0;
        BitmapImage bitmapImage = new();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = memory;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.EndInit();

        bitmapImage.Freeze();
        return bitmapImage;
      }
    }
    catch (Exception ex)
    {
      throw new UtilsException("Error in MapCreator.ConvertBitmapToBitmapImage", ex);
    }
  }

  private static Tile Latlon2Tile(double lat_deg, double lon_deg, int zoom) // bbox[3], bbox[0], zoom
  {
    var lat_rad = lat_deg * Math.PI / 180.0;
    var n = Math.Pow(2.0, zoom);
    var xTile = (int)Math.Floor((lon_deg + 180.0) / 360.0 * n);
    var yTile = (int)Math.Floor((1.0 - Math.Log(Math.Tan(lat_rad) + 1 / Math.Cos(lat_rad)) / Math.PI) / 2.0 * n);

    return new Tile(xTile, yTile);
  }

  private static Point LatLonToPixel(double lat, double lon, int zoom)
  {
    var lat_rad = lat * Math.PI / 180.0;
    var n = Math.Pow(2.0, zoom);
    var x_pixel = (int)Math.Floor((lon + 180.0) / 360.0 * n * 256);
    var y_pixel =
      (int)Math.Floor((1.0 - Math.Log(Math.Tan(lat_rad) + 1 / Math.Cos(lat_rad)) / Math.PI) / 2.0 * n * 256);

    return new Point(x_pixel, y_pixel);
  }


  private Bitmap LoadMarkerIcon(Marker marker)
  {
    var filename = marker switch
    {
      Marker.PIN_RED_16px => "pin-red_16px",
      Marker.PIN_RED_32px => "pin-red_32px",
      Marker.MARKER_RED_16px => "marker-red_16px",
      Marker.MARKER_RED_32px => "marker-red_32px",
      _ => throw new ArgumentException("Unknown marker type")
    };

    var resourcePath = $"{ConfigurationHelper.BaseDirectory}Persistence\\Resources\\{filename}.png";

    return new Bitmap(resourcePath);
  }

  public record GeoCoordinate(double Lon, double Lat)
  {
  }
}