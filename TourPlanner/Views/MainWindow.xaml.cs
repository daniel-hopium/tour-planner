using System;
using System.Windows;
using TourPlanner.ViewModels;
using System.Windows.Controls;
using FontAwesome.WPF;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TourPlanner.Models;


namespace TourPlanner.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel; // { get; set;  }
        private TourListControlViewModel _tourListControlViewModel;
        private TourViewModel _formTourViewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            _formTourViewModel = TourViewModel.Instance;
            _formTourViewModel.UpdateCompleted += FormTourViewModel_UpdateCompleted;
            _tourListControlViewModel = TourListControlViewModel.Instance;
            _tourListControlViewModel.TourToEditSelected += TourList_TourToEditSelected;
            _tourListControlViewModel.NowExpandedTour += TourList_NowExpandedTour;
            //Loaded += MainWindow_Loaded;
        }

        //public bool errorsChanged = false;

        private void TourList_TourToEditSelected(object sender, EventArgs e)
        {
            _tourListControlViewModel.ResetEditModeCommand.Execute(null);
            if(sender != null)
            {
                _formTourViewModel.TourSetCommand.Execute(sender);
                Tour.IsEnabled = true;
                MainTabControl.SelectedIndex = 2;
            }
            else
            {
                Tour.IsEnabled = false;
                MainTabControl.SelectedIndex = 0;
            }
        }

        private void FormTourViewModel_UpdateCompleted(object sender, EventArgs e)
        {           
            _tourListControlViewModel.LoadToursCommand.Execute(null);
            MainTabControl.SelectedIndex = 0;
            Tour.IsEnabled = false;
        }


        private void TourList_NowExpandedTour(object sender, EventArgs e)
        {
            if(sender is TourViewModel tourViewModel)
            {
                Route.IsEnabled = true;
                Logs.IsEnabled = true;
            }
            else
            {
                Route.IsEnabled = false;
                Logs.IsEnabled = false;
            }
        }

        /*private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }*/

        private void AddTour_Click(object sender, RoutedEventArgs e)
        {
            _tourListControlViewModel.ResetEditModeCommand.Execute(null);
            _formTourViewModel.TourSetCommand.Execute(new TourViewModel());
            Tour.IsEnabled = true;
            MainTabControl.SelectedIndex = 2;
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.Owner = this;
            helpWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            helpWindow.ShowDialog(); // This shows the Help window as a modal dialog
        }
        

        private void AddLog_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.AddLogCommand.Execute(sender);
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
                    // Perform the deletion or any other action
                    _viewModel.EditLogCommand.Execute(logToEdit);
                }
            }
        }

        private void TourLogDelete_OnClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Delete Button clicked");
            // Get the button that was clicked
            Button deleteButton = sender as Button;
            if (deleteButton != null)
            {
                // Retrieve the TourLog object from the DataContext of the button
                TourLogViewModel logToDelete = deleteButton.DataContext as TourLogViewModel;
                if (logToDelete != null)
                {
                    // Perform the deletion or any other action
                    _viewModel.DeleteLogCommand.Execute(logToDelete);
                }
            }
        }
    }
}