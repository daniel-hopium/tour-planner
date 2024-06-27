using System.ComponentModel;
using System.Windows.Input;

namespace TourPlanner.ViewModels;

public interface IMainViewModel : INotifyPropertyChanged
{
  TourViewModel? ExpandedTour { get; set; }
  WeatherViewModel WeatherViewModel { get; set; }
  ICommand SummarizeReportCommand { get; set; }
  ICommand ImportTourCommand { get; set; }

  // Methods
  void SummarizeReport(object obj);
  void ImportTour(object obj);
}