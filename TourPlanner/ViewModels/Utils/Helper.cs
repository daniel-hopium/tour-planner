using System;
using System.Reflection;
using log4net;

namespace TourPlanner.ViewModels.Utils;

public static class Helper
{
  private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

  public static DateOnly? ExtractAndConvertDatePart(string dateTimeStr)
  {
    // Try to find the index of the first space which might indicate the start of the time part
    var firstSpaceIndex = dateTimeStr.IndexOf(' ');
    var datePart = dateTimeStr;
    if (firstSpaceIndex != -1)
      // Extract only the date part up to the first space
      datePart = dateTimeStr[..firstSpaceIndex];

    // Split the date string into components [MM, DD, YYYY]
    var dateComponents = datePart.Split('/');
    if (dateComponents.Length == 3)
      // Try to parse the month, day, and year
      if (int.TryParse(dateComponents[0], out var month) &&
          int.TryParse(dateComponents[1], out var day) &&
          int.TryParse(dateComponents[2], out var year))
        // Attempt to create a DateOnly object from the parts
        try
        {
          return new DateOnly(year, month, day);
        }
        catch (ArgumentOutOfRangeException ex)
        {
          log.Error("Failed to create DateOnly: " + ex.Message);
          return null;
        }

    return null;
  }
}