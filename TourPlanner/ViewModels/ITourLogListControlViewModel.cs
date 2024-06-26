using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TourPlanner.ViewModels
{
    public interface ITourLogListControlViewModel : INotifyPropertyChanged
    {
        event EventHandler? LogsChanged;
        event PropertyChangedEventHandler? PropertyChanged;
        ICommand ChangeTourCommand { get; }
        ICommand EditLogCommand { get; }
        ICommand DeleteLogCommand { get; }
        ICommand AddLogCommand { get; }
    }
}
