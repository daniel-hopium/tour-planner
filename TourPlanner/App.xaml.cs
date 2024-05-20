using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using TourPlanner.Views;
using TourPlanner.ViewModels;

namespace TourPlanner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            //var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            //mainWindow.TourListControlViewModel = ServiceProvider.GetRequiredService<TourListControlViewModel>();            
            //mainWindow.Show();
            //var TourListControlViewModel = ServiceProvider.GetRequiredService<TourListControlViewModel>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            /*services.AddSingleton<MainWindow>();
            services.AddSingleton<TourViewModel>();*/
            // Weitere Services hier hinzufügen
            //services.AddSingleton<MainViewModel>();
            //services.AddTransient<MainWindow>();

            //services.AddSingleton<TourListControlViewModel>();
        }
    }
}