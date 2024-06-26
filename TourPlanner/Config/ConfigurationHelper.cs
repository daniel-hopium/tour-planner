using System;
using System.Configuration;
using System.IO;

namespace TourPlanner.Config;

public class ConfigurationHelper
{
    public static string GetFullPath(string key)
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string relativePath = ConfigurationManager.AppSettings[key];
        return Path.Combine(baseDirectory, relativePath);
    }

    public static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;
    public static string ImagesDirectory => GetFullPath("ImagesDirectory");
    public static string ReportsDirectory => GetFullPath("ReportsDirectory");
    public static string ExportsDirectory => GetFullPath("ExportsDirectory");
    public static string OpenRouteApiKey => ConfigurationManager.AppSettings["OpenRouteApiKey"];
}