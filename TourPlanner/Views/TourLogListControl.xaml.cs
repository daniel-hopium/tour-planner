using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TourPlanner.ViewModels;

namespace TourPlanner.Views
{
    /// <summary>
    /// Interaktionslogik für TourLogListControl.xaml
    /// </summary>
    public partial class TourLogListControl : UserControl
    {
        private readonly TourLogListControlViewModel _tourLogListViewModel;

        public TourLogListControl()
        {
            InitializeComponent();
            _tourLogListViewModel = TourLogListControlViewModel.Instance;
            DataContext = _tourLogListViewModel;
        }


        private void TourLogEdit_OnClick(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked
            Button editButton = sender as Button;
            if (editButton != null)
            {
                // Retrieve the TourLog object from the DataContext of the button
                TourLogViewModel logToEdit = editButton.DataContext as TourLogViewModel;
                if (logToEdit != null)
                {
                    _tourLogListViewModel.EditLogCommand.Execute(logToEdit);
                }
            }
        }

        private void TourLogDelete_OnClick(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked
            Button deleteButton = sender as Button;
            if (deleteButton != null)
            {
                // Retrieve the TourLog object from the DataContext of the button
                TourLogViewModel logToDelete = deleteButton.DataContext as TourLogViewModel;
                if (logToDelete != null)
                {
                    _tourLogListViewModel.DeleteLogCommand.Execute(logToDelete);
                }
            }
        }
    }
}
