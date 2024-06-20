using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using TourPlanner.Models;
using TourPlanner.Persistence.Repository;
using TourPlanner.ViewModels.Utils;
using TourPlanner.ViewModels;
using TourPlanner;
using TourPlanner.Persistence.Utils;

namespace TourPlanner.ViewModels
{
    public class TourListControlViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TourViewModel> _tours = new ObservableCollection<TourViewModel>();

        public ObservableCollection<TourViewModel> Tours
        {
            get => _tours;
            set
            {
                _tours = value;
                OnPropertyChanged(nameof(Tours));
            }
        }

        // Singleton Pattern without Dependency Injection -> all ViewModels able use same TourListControlViewModel
        private static readonly TourListControlViewModel _instance = new TourListControlViewModel();
        public static TourListControlViewModel Instance
        {
            get
            {
                return _instance;
            }
        }
        static TourListControlViewModel()
        {
            
        }

        private readonly TourRepository _tourRepository;

        public TourListControlViewModel()
        {
            _tourRepository = TourRepository.Instance;
            LoadTours();

            ExpandedCommand = new RelayCommand(ExpandTour);            
            TourEditCommand = new RelayCommand(EditTour);
            TourReportCommand = new RelayCommand(ReportTour);
            ExportCommand = new RelayCommand(ExportTour);
            TourDeleteCommand = new RelayCommand(DeleteTour);
            LoadToursCommand = new RelayCommand(LoadToursFromOutside);
            ResetEditModeCommand = new RelayCommand(ResetEditModeTours);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 

        private void LoadTours()
        {
            var tourEntities = _tourRepository.GetTours();
            var tourModels = tourEntities.Select(entity => new TourModel(entity));

            Tours.Clear();
            foreach (var tour in tourModels)
            {
                Tours.Add(new TourViewModel(tour));
            }
            if (Tours.Any())
            {
                Tours.First().IsExpanded = true;
            }
        }

        private void LoadToursFromOutside(object parameter)
        {
            LoadTours();           
        }


        private TourViewModel? _expandedTour = null;

        public TourViewModel ExpandedTour
        {
            get { return _expandedTour; }
            set
            {
                if (_expandedTour != value)
                {
                    if (_expandedTour != null)
                    {
                        _expandedTour.PropertyChanged -= TourViewModel_PropertyChanged; // Unsubscribe from the previously expanded tour
                        _expandedTour.ClearLogs();
                    }

                    _expandedTour = value;
                    OnPropertyChanged(nameof(ExpandedTour));
                    
                    if (_expandedTour != null)
                    {
                        _expandedTour.PropertyChanged += TourViewModel_PropertyChanged; // Subscribe to the newly expanded tour
                        _expandedTour.LoadLogs();
                        _expandedTour.LoadMap();
                    }

                    OnNowExpandedTour(_expandedTour);
                }
            }
        }

        private void TourViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsExpanded")
            {
                var tourViewModel = sender as TourViewModel;

                if (tourViewModel != null && !tourViewModel.IsExpanded)
                {
                    // If the IsExpanded property of a TourViewModel becomes false, set ExpandedTour to null
                    ExpandedTour = null;
                }
            }
        }

        private void ExpandTour(object parameter)
        {
            if (parameter is TreeViewItem treeViewItem)
            {
                var tourViewModel = treeViewItem.DataContext as TourViewModel;

                if (tourViewModel != null)
                {
                    ExpandedTour = tourViewModel;

                    foreach (var item in Tours)
                    {
                        if (item != tourViewModel)
                        {
                            if (item != null)
                            {
                                item.IsExpanded = false;
                            }
                        }
                    }
                }
            }
        }       

        public event EventHandler NowExpandedTour;
        protected virtual void OnNowExpandedTour(object sender)
        {
            NowExpandedTour?.Invoke(sender, EventArgs.Empty);
        }


        public event EventHandler TourToEditSelected;

        protected virtual void SelectTourToEdit(object sender)
        {
            TourToEditSelected?.Invoke(sender, EventArgs.Empty);
        }

        private async void EditTour(object parameter)
        {
            if (parameter is TreeViewItem treeViewItem)
            {
                var tourViewModel = treeViewItem.DataContext as TourViewModel;

                if (tourViewModel != null)
                {
                    if(tourViewModel.IsEditMode)
                    {
                        tourViewModel.SetEditModeCommand.Execute(false);
                        SelectTourToEdit(null);
                        return;
                    }

                    // get Data for this tour of DB and make a new tourviewmodel/ a copy so data of listet tour is not changed before saved in DB
                    var getTourEntity = _tourRepository.GetTourByIdAsync(tourViewModel.Id);
                    var tourEntity = await getTourEntity;

                    if (tourEntity != null)
                    {                       
                        //ToEditTour = new TourViewModel(new TourModel(tourEntity));
                        SelectTourToEdit(new TourViewModel(new TourModel(tourEntity)));
                        //_toEditTour.TourSetCommand.Execute(treeViewItem);
                        tourViewModel.SetEditModeCommand.Execute(true);
                    }
                }
            }
        }

        private void ResetEditModeTours(object parameter)
        {
            foreach (TourViewModel tour in Tours)
            {
                if (tour != null)
                {
                        tour.SetEditModeCommand.Execute(false);
                }
            }
        }

        private void ReportTour(object parameter)
        {
        }

        private void ExportTour(object parameter)
        {
        }

        private async void DeleteTour(object parameter)
        {
            if (parameter is TreeViewItem treeViewItem)
            {
                var tourViewModel = treeViewItem.DataContext as TourViewModel;

                if (tourViewModel != null)
                {
                    await _tourRepository.DeleteTourByIdAsync(tourViewModel.Id);
                    LoadTours();
                    MessageBox.Show($"Tour successfully deleted");
                }
            }
        }
     

        private ICommand _expandedCommand;

        public ICommand ExpandedCommand
        {
            get { return _expandedCommand; }
            set
            {
                _expandedCommand = value;
                OnPropertyChanged(nameof(ExpandedCommand));
            }
        }

        private ICommand _tourCreateCommand;

        public ICommand TourCreateCommand
        {
            get { return _tourCreateCommand; }
            set
            {
                _tourCreateCommand = value;
                OnPropertyChanged(nameof(TourCreateCommand));
            }
        }

        private ICommand _tourEditCommand;

        public ICommand TourEditCommand
        {
            get { return _tourEditCommand; }
            set
            {
                _tourEditCommand = value;
                OnPropertyChanged(nameof(TourEditCommand));
            }
        }

        private ICommand _saveCommand;

        public ICommand SaveCommand
        {
            get { return _saveCommand; }
            set
            {
                _saveCommand = value;
                OnPropertyChanged(nameof(SaveCommand));
            }
        }

        private ICommand _tourReportCommand;

        public ICommand TourReportCommand
        {
            get { return _tourReportCommand; }
            set
            {
                _tourReportCommand = value;
                OnPropertyChanged(nameof(TourReportCommand));
            }
        }

        private ICommand _exportCommand;

        public ICommand ExportCommand
        {
            get { return _exportCommand; }
            set
            {
                _exportCommand = value;
                OnPropertyChanged(nameof(ExportCommand));
            }
        }

        private ICommand _tourDeleteCommand;

        public ICommand TourDeleteCommand
        {
            get { return _tourDeleteCommand; }
            set { 
                _tourDeleteCommand = value;
                OnPropertyChanged(nameof(TourDeleteCommand));
            }
        }

        private ICommand _loadToursCommand;
        public ICommand LoadToursCommand
        {
            get { return _loadToursCommand; }
            set
            {
                _loadToursCommand = value;
                OnPropertyChanged(nameof(LoadToursCommand));
            }
        }

        private ICommand _resetEditModeCommand;
        public ICommand ResetEditModeCommand
        {
            get { return _resetEditModeCommand; }
            set
            {
                _resetEditModeCommand = value;
                OnPropertyChanged(nameof(ResetEditModeCommand));
            }
        }
    }
}

