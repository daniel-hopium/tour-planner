using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using log4net;
using TourPlanner.Config;
using TourPlanner.Exceptions;
using TourPlanner.Persistence.Entities;

namespace TourPlanner.Persistence.Utils;

public static class ExportGenerator
{
  private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

  public static async Task ExportTourAsync(TourEntity tour)
  {
    try
    {
      var csvPath =
        $"{ConfigurationHelper.ExportsDirectory}tour_{tour.Name}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.csv";

      var tourLogs = tour.Logs.ToList();

      var config = new CsvConfiguration(CultureInfo.InvariantCulture)
      {
        Delimiter = ",",
        Encoding = Encoding.UTF8,
        Quote = '"',
        Escape = '"',
        BadDataFound = null
      };

      await using var writer = new StreamWriter(csvPath, false, Encoding.UTF8);
      using var csv = new CsvWriter(writer, config);
      {
        // write Tour info header
        csv.WriteField("Name");
        csv.WriteField("Description");
        csv.WriteField("FromAddress");
        csv.WriteField("ToAddress");
        csv.WriteField("TransportType");
        csv.WriteField("Distance");
        csv.WriteField("EstimatedTime");
        csv.WriteField("Image");
        csv.WriteField("Popularity");
        csv.WriteField("ChildFriendliness");
        csv.NextRecord();

        // write tour data
        csv.WriteField(tour.Name);
        csv.WriteField(tour.Description);
        csv.WriteField(tour.FromAddress.ToString());
        csv.WriteField(tour.ToAddress.ToString());
        csv.WriteField(tour.TransportType);
        csv.WriteField(tour.Distance);
        csv.WriteField(tour.EstimatedTime);
        csv.WriteField(tour.Image);
        csv.WriteField(tour.Popularity);
        csv.WriteField(tour.ChildFriendliness.ToString());

        csv.NextRecord();
        csv.NextRecord();

        // write Tourlog info header
        csv.WriteField("TourDate");
        csv.WriteField("Comment");
        csv.WriteField("Difficulty");
        csv.WriteField("Distance");
        csv.WriteField("TotalTime");
        csv.WriteField("Rating");
        csv.NextRecord();

        // write Tourlog data-rows
        foreach (var log in tourLogs)
        {
          csv.WriteField(log.TourDate);
          csv.WriteField(log.Comment);
          csv.WriteField(log.Difficulty);
          csv.WriteField(log.Distance);
          csv.WriteField(log.TotalTime);
          csv.WriteField(log.Rating);
          csv.NextRecord();
        }
      }
    }
    catch (Exception ex)
    {
      log.Error($"Exportfile could not be generated: {ex}");
      throw new UtilsException("Error in ExportGenerator.ExportTourAsync", ex);
    }
  }
}