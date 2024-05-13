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
        if (viewModel != null && viewModel.HasErrors == false && viewModel.Comment != null) // Fix empty fields when opening the window
        {
            this.DialogResult = true; // This will close the window only if valid
            this.Close();
        }
        else
        {
            MessageBox.Show("Please correct the errors before saving.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
    }
}