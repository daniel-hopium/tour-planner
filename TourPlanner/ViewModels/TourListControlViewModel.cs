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
using log4net;
using System.Reflection;
using TourPlanner.Exceptions;

namespace TourPlanner.ViewModels
{
    public class TourListControlViewModel : INotifyPropertyChanged
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly TourRepository _tourRepository;


        // Singleton Pattern without Dependency Injection -> all ViewModels able use same TourListControlViewModel
        private static readonly TourListControlViewModel _instance = new ();
        public static TourListControlViewModel Instance
        {
            get { return _instance; }
        }
   

        public TourListControlViewModel()
        {
            _tourRepository = TourRepository.Instance;
            LoadTours();

            _expandedCommand = new RelayCommand(ExpandTour);            
            _tourEditCommand = new RelayCommand(EditTour);
            _tourReportCommand = new RelayCommand(ReportTour);
            _exportCommand = new RelayCommand(ExportTour);
            _tourDeleteCommand = new RelayCommand(DeleteTour);
            _loadToursCommand = new RelayCommand(LoadToursFromOutside);
            _resetEditModeCommand = new RelayCommand(ResetEditModeTours);
        }


        private ObservableCollection<TourViewModel> _tours = new();

        public ObservableCollection<TourViewModel> Tours
        {
            get => _tours;
            set
            {
                _tours = value;
                OnPropertyChanged(nameof(Tours));
            }
        }


        //////////////////////// Load Tours of List ///////////////////////////////////////////

        private void LoadTours()
        {
            try
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
                else { MessageBox.Show($"No Tours created yet"); }
            }
            catch (DALException)
            {
                // MessageBox.Show($"Tours could not be loaded");  // Why when closing window or loading Tours after Logchange
            }
            catch (Exception ex)
            {
                log.Error($"Tourlist could not be loaded: {ex}");
            }
        }

        private void LoadToursFromOutside(object parameter)
        {
            LoadTours();           
        }     


        //////////////////////// Tour Expanding funcionality (just one tour at the same time expanded) ///////////////////////////////////////////

        private TourViewModel? _expandedTour = null;

        public TourViewModel? ExpandedTour
        {
            get { return _expandedTour; }
            set
            {
                if (_expandedTour != value)
                {
                    if (_expandedTour != null)
                    {
                        _expandedTour.PropertyChanged -= TourViewModel_PropertyChanged; // Unsubscribe from the previously expanded tour
                    }

                    _expandedTour = value;
                    OnPropertyChanged(nameof(ExpandedTour));
                    
                    if (_expandedTour != null)
                    {
                        _expandedTour.PropertyChanged += TourViewModel_PropertyChanged; // Subscribe to the newly expanded tour
                        _expandedTour.LoadLogsCommand.Execute(null);
                        _expandedTour.LoadMapCommand.Execute(null);
                    }

                    OnNowExpandedTour(_expandedTour);
                }
            }
        }


        private void TourViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsExpanded")
            {
                if (sender is TourViewModel tourViewModel && !tourViewModel.IsExpanded)
                {                    
                    ExpandedTour = null;  // If the IsExpanded property of a TourViewModel becomes false, set ExpandedTour to null
                }
            }
        }

        private void ExpandTour(object parameter)
        {
            if (parameter is TreeViewItem treeViewItem)
            {
                if (treeViewItem.DataContext is TourViewModel tourViewModel)
                {
                    ExpandedTour = tourViewModel;

                    foreach (var item in Tours)
                    {
                        if (item != tourViewModel && item != null)
                        {
                                item.IsExpanded = false;
                        }
                    }
                }
            }
        }       

        public event EventHandler? NowExpandedTour;
        protected virtual void OnNowExpandedTour(object? sender)
        {
            NowExpandedTour?.Invoke(sender, EventArgs.Empty);
        }


        //////////////////////// Action Bar on Tour in List ///////////////////////////////////////////

        public event EventHandler? TourToEditSelected;

        protected virtual void SelectTourToEdit(object? sender)
        {
            TourToEditSelected?.Invoke(sender, EventArgs.Empty);
        }

        private async void EditTour(object parameter)
        {
            try
            {
                if (parameter is TreeViewItem treeViewItem)
                {
                    if (treeViewItem.DataContext is TourViewModel tourViewModel)
                    {
                        if (tourViewModel.IsEditMode)
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
                            SelectTourToEdit(new TourViewModel(new TourModel(tourEntity)));
                            tourViewModel.SetEditModeCommand.Execute(true);
                        }
                    }
                }
            }
            catch (DALException)
            {
                MessageBox.Show($"Tour to edit could not be found");
            }
            catch (Exception ex)
            {
                log.Error($"Unknown error selecting tour to edit: {ex}");
            }
        }

        private void ResetEditModeTours(object parameter)
        {
            foreach (TourViewModel tour in Tours)
            {
                tour?.SetEditModeCommand.Execute(false);
            }
        }

        private async void ReportTour(object parameter)
        {
            try
            {
                if (parameter is TreeViewItem treeViewItem)
                {
                    if (treeViewItem.DataContext is TourViewModel tourViewModel)
                    {
                        await _tourRepository.GenerateTourReport(tourViewModel.Id);
                        MessageBox.Show($"Report for {tourViewModel.Name} generated successfully.");
                    }
                }
            }
            catch (DALException)
            {
                MessageBox.Show($"Data for Tour-Report could not be processed");
            }
            catch (UtilsException)
            {
                MessageBox.Show($"Tour-Report could not be generated");
            }
            catch (Exception ex)
            {
                log.Error($"Unknown error during Tour-Report generation: {ex}");
            }
        }

        private async void ExportTour(object parameter)
        {
            try
            {
                if (parameter is TreeViewItem treeViewItem)
                {
                    if (treeViewItem.DataContext is TourViewModel tourViewModel)
                    {
                        await _tourRepository.GenerateTourExportAsync(tourViewModel.Id);
                        MessageBox.Show($"Tour {tourViewModel.Name} successfully exported");
                    }
                }
            }
            catch (DALException)
            {
                MessageBox.Show($"Tour data for Export could not be loaded");
            }
            catch (UtilsException)
            {
                MessageBox.Show($"Tour-Export could not be generated");
            }
            catch (Exception ex)
            {
                log.Error($"Unknown error during Tour-Export: {ex}");
            }
        }

        private async void DeleteTour(object parameter)
        {
            try 
            { 
                if (parameter is TreeViewItem treeViewItem)
                {
                    if (treeViewItem.DataContext is TourViewModel tourViewModel)
                    {
                        await _tourRepository.DeleteTourByIdAsync(tourViewModel.Id);
                        LoadTours();
                        MessageBox.Show($"Tour successfully deleted");
                    }
                }
            }
            catch (DALException)
            {
                MessageBox.Show($"Tour could not be deleted");
            }
            catch (Exception ex)
            {
                log.Error($"Unknown error during deletion of Tour: {ex}");
            }
        }



        //////////////////////// Commands / Events ///////////////////////////////////////////


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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


        //////////////////////// Commands / Events ///////////////////////////////////////////
    }
}

