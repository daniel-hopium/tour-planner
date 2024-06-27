using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using log4net;
using TourPlanner.Config;
using TourPlanner.Exceptions;
using TourPlanner.Persistence.Entities;

namespace TourPlanner.Persistence.Utils;

public record TourSummary(int Id, string Name, int AverageTime, double AverageDistance, double AverageRating)
{
}

public static class ReportGenerator
{
  private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

  public static async Task GenerateTourReportAsync(TourEntity tour)
  {
    try
    {
      var tourLogs = tour.Logs.ToList();

      await using FileStream fs =
        new(
          ConfigurationHelper.ReportsDirectory +
          $"{tour.Name}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_Report.pdf", FileMode.Create,
          FileAccess.Write, FileShare.None);
      {
        PdfWriter writer = new(fs);
        PdfDocument pdf = new(writer);
        Document document = new(pdf);

        // load image
        var imageData = ImageDataFactory.Create(ConfigurationHelper.ImagesDirectory + tour.Image);
        Image img = new(imageData);
        document.Add(img);
        document.Add(new Paragraph(" "));

        // Add a title
        var title = new Paragraph($"{tour.Name}")
          .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
          .SetFontSize(20);
        document.Add(title);

        document.Add(new Paragraph(tour.Description));

        document.Add(new Paragraph(" "));
        document.Add(new Paragraph($"From: {tour.FromAddress}"));
        document.Add(new Paragraph($"To: {tour.ToAddress}"));
        document.Add(new Paragraph($"Transport Type: {tour.TransportType}"));
        document.Add(new Paragraph($"Distance (km): {tour.Distance}"));
        document.Add(new Paragraph($"Estimated Time: {tour.EstimatedTime}"));
        document.Add(new Paragraph($"Popularity: {tour.Popularity}"));
        document.Add(new Paragraph($"Childfriendliness: {tour.ChildFriendliness}"));

        document.Add(new Paragraph(" "));
        var subtitle = new Paragraph("Tour Logs:")
          .SetFontSize(18);
        document.Add(subtitle);

        // Create a table with columns
        Table table = new(6);
        table.AddCell(new Cell().Add(new Paragraph("Tour Date")));
        table.AddCell(new Cell().Add(new Paragraph("Comment")));
        table.AddCell(new Cell().Add(new Paragraph("Difficulty")));
        table.AddCell(new Cell().Add(new Paragraph("Distance")));
        table.AddCell(new Cell().Add(new Paragraph("Total Time")));
        table.AddCell(new Cell().Add(new Paragraph("Rating")));

        // Add data to the table
        foreach (var item in tourLogs)
        {
          table.AddCell(new Cell().Add(new Paragraph(item.TourDate.ToString())));
          table.AddCell(new Cell().Add(new Paragraph(item.Comment)));
          table.AddCell(new Cell().Add(new Paragraph(item.Difficulty.ToString())));
          table.AddCell(new Cell().Add(new Paragraph(item.Distance.ToString())));
          table.AddCell(new Cell().Add(new Paragraph(item.TotalTime.ToString())));
          table.AddCell(new Cell().Add(new Paragraph(item.Rating.ToString())));
        }

        document.Add(table);
        document.Close();
      }
    }
    catch (Exception ex)
    {
      log.Error($"Tour-Report generation failed: {ex}");
      throw new UtilsException("Error in ReportGenerator.GenerateTourReportAsync", ex);
    }
  }


  public static async Task GenerateSummarizeReportAsync(List<TourSummary> toursummary)
  {
    try
    {
      await using FileStream fs =
        new(
          ConfigurationHelper.ReportsDirectory +
          $"Summarize_Report_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.pdf", FileMode.Create,
          FileAccess.Write, FileShare.None);
      {
        PdfWriter writer = new(fs);
        PdfDocument pdf = new(writer);
        Document document = new(pdf);

        // Add a title
        var title = new Paragraph("Summarize Report")
          .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
          .SetFontSize(20);
        document.Add(title);

        document.Add(new Paragraph(" "));

        Table table = new(4);
        var nameHeader = new Paragraph("Tour Name")
          .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
          .SetFontSize(14);
        table.AddCell(new Cell().Add(nameHeader));
        table.AddCell(new Cell().Add(new Paragraph("avg. Time(min)")));
        table.AddCell(new Cell().Add(new Paragraph("avg. Distance(km)")));
        table.AddCell(new Cell().Add(new Paragraph("avg. Rating")));

        foreach (var item in toursummary)
        {
          var name = new Paragraph(item.Name)
            .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD));
          table.AddCell(new Cell().Add(name));

          table.AddCell(new Cell().Add(new Paragraph(item.AverageTime.ToString())));
          table.AddCell(new Cell().Add(new Paragraph(item.AverageDistance.ToString())));
          table.AddCell(new Cell().Add(new Paragraph(item.AverageRating.ToString())));
        }

        document.Add(table);
        document.Close();
      }
    }
    catch (Exception ex)
    {
      log.Error($"Summarize-Report generation failed: {ex}");
      throw new UtilsException("Error in ReportGenerator.GenerateSummarizeReportAsync", ex);
    }
  }
}