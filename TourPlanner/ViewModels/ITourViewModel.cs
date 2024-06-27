using System;
using System.ComponentModel;
using System.Windows.Input;

namespace TourPlanner.ViewModels;

public interface ITourViewModel
{
  ICommand LoadMapCommand { get; }
  ICommand LoadLogsCommand { get; }
  ICommand SaveCommand { get; }
  ICommand TourSetCommand { get; }
  ICommand SetEditModeCommand { get; }
  ICommand CalculateCommand { get; }
  event PropertyChangedEventHandler? PropertyChanged;
  event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
}