using System;
using System.IO;
using System.Reflection;
using System.Windows;
using log4net;
using log4net.Config;
using TourPlanner.Config;
using TourPlanner.Persistence.Utils;

namespace TourPlanner;

/// <summary>
///   Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  //public IServiceProvider ServiceProvider { get; private set; }

  private static readonly ILog log = LogManager.GetLogger(typeof(App));

  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    try
    {
      var success = DatabaseManager.CheckDbConnection();
      if (!success) throw new InvalidOperationException("Database connection has not been opened.");
    }
    catch (Exception ex)
    {
      log.Error(ex);
      MessageBox.Show("Error connecting to database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      Shutdown();
    }

    // Konfigurieren Sie log4net mit der Konfigurationsdatei
    ConfigureLog4Net();
    log.Info("Application started.");
  }

  protected override void OnExit(ExitEventArgs e)
  {
    log.Info("Application exited.");
    base.OnExit(e);
  }

  private void ConfigureLog4Net()
  {
    var projectDirectory = ConfigurationHelper.BaseDirectory;
    var logFilePath = projectDirectory + "logfile.log";

    // Set the LogFileName property
    GlobalContext.Properties["LogFileName"] = logFilePath;

    // Configure log4net
    var repository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
    XmlConfigurator.Configure(repository, new FileInfo("App.config"));
  }
}