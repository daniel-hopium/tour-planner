using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TourPlanner.ViewModels
{
    public interface ITourViewModel
    {
        event PropertyChangedEventHandler? PropertyChanged;
        event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        ICommand LoadMapCommand { get; }
        ICommand LoadLogsCommand { get; }
        ICommand SaveCommand { get; }
        ICommand TourSetCommand { get; }
        ICommand SetEditModeCommand { get; }
        ICommand CalculateCommand { get; }
    }
}
