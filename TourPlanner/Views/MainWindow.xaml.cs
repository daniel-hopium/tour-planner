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
        private readonly MainViewModel _viewModel; // { get; set;  }
        public readonly TourListControlViewModel TourListControlViewModel;
        private readonly TourViewModel _formTourViewModel;

        private Point _origin;
        private Point _start;
        private double _scale = 0.45;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
            _formTourViewModel = TourViewModel.Instance;
            _formTourViewModel.UpdateCompleted += FormTourViewModel_UpdateCompleted;
            TourListControlViewModel = TourListControlViewModel.Instance;
            TourListControlViewModel.TourToEditSelected += TourList_TourToEditSelected;
            TourListControlViewModel.NowExpandedTour += TourList_NowExpandedTour;

            MapImage.MouseWheel += Image_MouseWheel;
            MapImage.MouseLeftButtonDown += Image_MouseLeftButtonDown;
            MapImage.MouseLeftButtonUp += Image_MouseLeftButtonUp;
            MapImage.MouseMove += Image_MouseMove;
        }


        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                _scale += 0.1;
            }
            else
            {
                _scale -= 0.1;
            }

            if (_scale < 0.45) _scale = 0.45;

            scaleTransform.ScaleX = _scale;
            scaleTransform.ScaleY = _scale;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MapImage.CaptureMouse();
            _start = e.GetPosition(MapViewer);
            _origin = new Point(translateTransform.X, translateTransform.Y);
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MapImage.ReleaseMouseCapture();
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!MapImage.IsMouseCaptured) return;

            Vector v = _start - e.GetPosition(MapViewer);
            translateTransform.X = _origin.X - v.X;
            translateTransform.Y = _origin.Y - v.Y;
        }

        private void TourList_TourToEditSelected(object sender, EventArgs e)
        {
            TourListControlViewModel.ResetEditModeCommand.Execute(null);
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
            TourListControlViewModel.LoadToursCommand.Execute(null);
            MainTabControl.SelectedIndex = 0;
            Tour.IsEnabled = false;
        }


        private void TourList_NowExpandedTour(object sender, EventArgs e)
        {
            if(sender is TourViewModel)
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


        private void AddTour_Click(object sender, RoutedEventArgs e)
        {
            TourListControlViewModel.ResetEditModeCommand.Execute(null);
            _formTourViewModel.TourSetCommand.Execute(new TourViewModel());
            Tour.IsEnabled = true;
            MainTabControl.SelectedIndex = 2;
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new ();
            helpWindow.Owner = this;
            helpWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            helpWindow.ShowDialog(); // This shows the Help window as a modal dialog
        }      
    }
}