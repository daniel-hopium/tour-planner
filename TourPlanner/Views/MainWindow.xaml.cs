using System;
using System.Windows;
using TourPlanner.Persistence.Utils;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourPlanner.Persistence.Repository;
using TourPlanner.ViewModels;
using System.Windows.Controls;


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
            //Loaded += MainWindow_Loaded;
        }

        // set isExpanded of other than the one now expanded item to false -> collapse so just one tour at same time expanded
        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem treeViewItem)
            {
                _viewModel.ExpandedCommand.Execute(treeViewItem);
            }
        }

        /*private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //ShowAllTours();
        }*/

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.ShowDialog(); // This shows the Help window as a modal dialog
        }

        private void LogButton_Click(object sender, RoutedEventArgs e)
        {
            LogWindow logWindow = new LogWindow();
            logWindow.ShowDialog(); // This shows the Help window as a modal dialog
        }
    }
}