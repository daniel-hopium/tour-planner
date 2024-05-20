using System;

namespace TourPlanner.ViewModels.Utils;

public static class Helper
{
    public static DateOnly? ExtractAndConvertDatePart(string dateTimeStr)
    {
        // Try to find the index of the first space which might indicate the start of the time part
        int firstSpaceIndex = dateTimeStr.IndexOf(' ');
        string datePart = dateTimeStr;
        if (firstSpaceIndex != -1)
        {
            // Extract only the date part up to the first space
            datePart = dateTimeStr.Substring(0, firstSpaceIndex);
        }

        // Split the date string into components [MM, DD, YYYY]
        string[] dateComponents = datePart.Split('/');
        if (dateComponents.Length == 3)
        {
            // Try to parse the month, day, and year
            if (int.TryParse(dateComponents[0], out int month) &&
                int.TryParse(dateComponents[1], out int day) &&
                int.TryParse(dateComponents[2], out int year))
            {
                // Attempt to create a DateOnly object from the parts
                try
                {
                    return new DateOnly(year, month, day);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // Handle or log the error as needed
                    Console.WriteLine("Failed to create DateOnly: " + ex.Message);
                    return null;
                }
            }
        }

        return null; // Return null if parsing fails or the format is incorrect
    }


}