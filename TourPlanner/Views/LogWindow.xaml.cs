using System.Windows;
using System.Windows.Controls;
using TourPlanner.ViewModels;

namespace TourPlanner.Views;

public partial class LogWindow : Window
{
    public LogWindow()
    {
        InitializeComponent();
    }

    private void SaveLogButton_Click(object sender, RoutedEventArgs e)
    {
        
        TourLogViewModel viewModel = this.DataContext as TourLogViewModel;
        if (viewModel!.HasErrors)
        {
            MessageBox.Show("Please correct the errors before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else if(viewModel!.AreFieldsEmpty())
        {
            MessageBox.Show("Some fields are empty", "Empty Fields", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        else
        {
            this.DialogResult = true; // This will close the window only if valid
            this.Close();
        }
        
    }
}