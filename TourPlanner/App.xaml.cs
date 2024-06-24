using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using TourPlanner.Views;
using TourPlanner.ViewModels;
using log4net;
using log4net.Config;
using System.IO;
using System.Configuration;

namespace TourPlanner
{
    /// <summary>
    /// Interaction logic for App.xaml
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
                bool success = TourPlanner.Persistence.Utils.DatabaseManager.CheckDbConnection();
                if (!success) throw new InvalidOperationException("Database connection has not been opened.");
            }
            catch (Exception ex)
            {
                log.Error(ex);
                MessageBox.Show($"Error connecting to database", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();               
            }

           // Konfigurieren Sie log4net mit der Konfigurationsdatei
           var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("App.config"));

            /*var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();*/          

            log.Info("Application started.");

            //var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            //mainWindow.TourListControlViewModel = ServiceProvider.GetRequiredService<TourListControlViewModel>();            
            //mainWindow.Show();
            //var TourListControlViewModel = ServiceProvider.GetRequiredService<TourListControlViewModel>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            log.Info("Application exited.");
            base.OnExit(e);
        }

        /*private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<TourListControlViewModel>();
        }*/
    }
}