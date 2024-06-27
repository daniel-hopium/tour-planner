using System;
using System.ComponentModel;
using System.Windows.Input;

namespace TourPlanner.ViewModels;

public interface ITourLogListControlViewModel : INotifyPropertyChanged
{
  ICommand ChangeTourCommand { get; }
  ICommand EditLogCommand { get; }
  ICommand DeleteLogCommand { get; }
  ICommand AddLogCommand { get; }
  event EventHandler? LogsChanged;
  event PropertyChangedEventHandler? PropertyChanged;
}