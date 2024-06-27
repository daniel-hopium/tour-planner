using System.Windows;
using System.Windows.Controls;
using TourPlanner.ViewModels;

namespace TourPlanner.Views;

/// <summary>
///   Interaktionslogik für TourLogListControl.xaml
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
    var editButton = sender as Button;
    if (editButton != null)
    {
      // Retrieve the TourLog object from the DataContext of the button
      var logToEdit = editButton.DataContext as TourLogViewModel;
      if (logToEdit != null) _tourLogListViewModel.EditLogCommand.Execute(logToEdit);
    }
  }

  private void TourLogDelete_OnClick(object sender, RoutedEventArgs e)
  {
    // Get the button that was clicked
    var deleteButton = sender as Button;
    if (deleteButton != null)
    {
      // Retrieve the TourLog object from the DataContext of the button
      var logToDelete = deleteButton.DataContext as TourLogViewModel;
      if (logToDelete != null) _tourLogListViewModel.DeleteLogCommand.Execute(logToDelete);
    }
  }
}