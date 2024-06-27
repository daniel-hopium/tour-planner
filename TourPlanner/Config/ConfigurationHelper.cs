using System;
using System.Configuration;
using System.IO;

namespace TourPlanner.Config;

public static class ConfigurationHelper
{
  public static string BaseDirectory => GetProjectDirectory() + "\\";
  public static string ImagesDirectory => GetFullPath("ImagesDirectory");
  public static string ReportsDirectory => GetFullPath("ReportsDirectory");
  public static string ExportsDirectory => GetFullPath("ExportsDirectory");

  private static string GetFullPath(string key)
  {
    var baseDirectory = GetProjectDirectory();
    var relativePath = ConfigurationManager.AppSettings[key];
    return baseDirectory + relativePath;
  }

  private static string GetProjectDirectory()
  {
    var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    var directoryInfo = new DirectoryInfo(baseDirectory);
    while (directoryInfo != null && directoryInfo.Name != "TourPlanner") directoryInfo = directoryInfo.Parent;

    return directoryInfo?.FullName ?? baseDirectory;
  }
}