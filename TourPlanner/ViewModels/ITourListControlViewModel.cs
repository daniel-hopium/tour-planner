using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TourPlanner.ViewModels
{
    public interface ITourListControlViewModel : INotifyPropertyChanged
    {
        event EventHandler? NowExpandedTour;
        event EventHandler? TourToEditSelected;
        event PropertyChangedEventHandler? PropertyChanged;
        ICommand ExpandedCommand { get; }
        ICommand TourReportCommand { get; }
        ICommand ExportCommand { get; }
        ICommand TourDeleteCommand { get; }
        ICommand LoadToursCommand { get; }
        ICommand ResetEditModeCommand { get; }
        ICommand TourEditCommand { get; }
    }
}
