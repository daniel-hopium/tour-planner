using System;
using System.Windows;
using TourPlanner.Persistence.Utils;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourPlanner.Persistence.Repository;
using TourPlanner.ViewModels;
using System.Windows.Controls;
using FontAwesome.WPF;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace TourPlanner.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            var dbContext = new TourPlannerDbContext();
            var tourRepository = new TourRepository(dbContext);
            _viewModel = new MainViewModel(tourRepository);
            DataContext = _viewModel;
            _viewModel.UpdateCompleted += ViewModel_UpdateCompleted;
            //Loaded += MainWindow_Loaded;
        }

        public bool errorsChanged = false;

        // set isExpanded of other than the one now expanded item to false -> collapse so just one tour at same time expanded
        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem) { _viewModel.ExpandedCommand.Execute(treeViewItem); }
        }


        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            // Suchen Sie rekursiv nach dem übergeordneten Element vom Typ T
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // Überprüfen, ob das übergeordnete Element vom richtigen Typ ist
            if (parentObject == null)
                return null;

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                // Rekursiv das übergeordnete Element suchen
                return FindVisualParent<T>(parentObject);
            }
        }

        private void Tour_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            // Zugriff auf das ImageAwesome-Element, auf das geklickt wurde
            ImageAwesome clickedImage = sender as ImageAwesome;
            // Zugriff auf das übergeordnete TreeViewItem
            TreeViewItem parentTreeViewItem = FindVisualParent<TreeViewItem>(clickedImage);

            if (parentTreeViewItem != null)
            {      
                if (clickedImage.Name == "TourEdit")
                {                
                    // MessageBox.Show($"Edit {parentTreeViewItem}");
                    _viewModel.TourEditCommand.Execute(parentTreeViewItem);
                    Tour.IsEnabled = true;
                    MainTabControl.SelectedIndex = 2;
                }
                if (clickedImage.Name == "TourReport") { _viewModel.TourReportCommand.Execute(parentTreeViewItem); }
                if (clickedImage.Name == "TourExport") { _viewModel.ExportCommand.Execute(parentTreeViewItem); }
                if (clickedImage.Name == "TourDelete") { _viewModel.TourDeleteCommand.Execute(parentTreeViewItem); }
            }
        }

        private void ViewModel_UpdateCompleted(object sender, EventArgs e)
        {
            MainTabControl.SelectedIndex = 0;
            Tour.IsEnabled = false;
        }

        /*private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //ShowAllTours();
        }*/

        private void AddTour_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.TourCreateCommand.Execute(sender);
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

        private void AddLogButton_Click(object sender, RoutedEventArgs e)
        {
            LogWindow logWindow = new LogWindow();
            logWindow.Owner = this;
            logWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            logWindow.ShowDialog(); // This shows the Help window as a modal dialog
        }
    }
}