using System;
using System.ComponentModel;
using System.Windows.Input;

namespace TourPlanner.ViewModels;

public interface ITourListControlViewModel : INotifyPropertyChanged
{
  ICommand ExpandedCommand { get; }
  ICommand TourReportCommand { get; }
  ICommand ExportCommand { get; }
  ICommand TourDeleteCommand { get; }
  ICommand LoadToursCommand { get; }
  ICommand ResetEditModeCommand { get; }
  ICommand TourEditCommand { get; }
  event EventHandler? NowExpandedTour;
  event EventHandler? TourToEditSelected;
  event PropertyChangedEventHandler? PropertyChanged;
}