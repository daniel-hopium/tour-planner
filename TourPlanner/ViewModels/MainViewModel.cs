using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Persistence.Repository;
using TourPlanner.Models;
using System.Windows.Input;
using TourPlanner.ViewModels.Utils;
using System.Windows.Controls;
using TourPlanner.Persistence.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows;
using System.Collections;
using TourPlanner.Mapper;
using TourPlanner.Views;


namespace TourPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TourViewModel> _tours;
        public ObservableCollection<TourViewModel> Tours
        {
            get { return _tours; }
            set {
                _tours = value;
                OnPropertyChanged(nameof(Tours));
        }}

        public Array TransportTypes => Enum.GetValues(typeof(TourPlanner.Models.TransportType));

        private readonly ITourRepository _tourRepository;
        private readonly TourMapper _tourMapper;

        public MainViewModel(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
            _tourMapper = new TourMapper(tourRepository);
            LoadTours();

            ExpandedCommand = new RelayCommand(ExpandTour);
            TourCreateCommand = new RelayCommand(CreateTour);
            TourEditCommand = new RelayCommand(EditTour);
            TourReportCommand = new RelayCommand(ReportTour);
            ExportCommand = new RelayCommand(ExportTour);
            TourDeleteCommand = new RelayCommand(DeleteTour);

            SaveCommand = new RelayCommand(SaveTour);
            
            AddLogCommand = new RelayCommand(AddLog);
            EditLogCommand = new RelayCommand(EditLog);
            DeleteLogCommand = new RelayCommand(DeleteLog);
        }

        private async void DeleteLog(object obj)
        {
            if (obj is TourLogViewModel tourLogViewModel)
             await _tourRepository.DeleteTourLogByIdAsync(tourLogViewModel.Id);
            _expandedTour.LoadLogs();
            MessageBox.Show($"Tour log successfully deleted");
        }

        private void EditLog(object obj)
        {
            
            LogWindow logWindow = new LogWindow();
            
            if (obj is TourLogViewModel tourLogViewModel)
                logWindow.DataContext = new TourLogViewModel(tourLogViewModel.TourLog);
            logWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            if (logWindow.ShowDialog() == true)
            {
                TourLogViewModel submittedData = logWindow.DataContext as TourLogViewModel;
                if (submittedData != null)
                {
                    // Now you can use submittedData to save or process further
                    UpdateTourLog(submittedData);
                    MessageBox.Show($"Changes to the tour log have been successfully applied");
                }
            }
            logWindow.DataContext = null;
        }

        private async void UpdateTourLog(TourLogViewModel submittedData)
        {
            var tourLogEntity = new TourLogEntity
            {
                Id = submittedData.Id,
                TourId = _expandedTour.Id,
                Distance = submittedData.TourLog.Distance,
                TotalTime = submittedData.TourLog.TotalTime,
                Rating = submittedData.TourLog.Rating,
                Comment  = submittedData.TourLog.Comment,
                Difficulty = submittedData.TourLog.Difficulty,
                TourDate = submittedData.TourLog.TourDate,
                
                
            };
            await _tourRepository.UpdateTourLogAsync(tourLogEntity);
           
            _expandedTour.ClearLogs();
            _expandedTour.LoadLogs();
        }

        private void AddLog(object obj)
        {
            LogWindow logWindow = new LogWindow();
            logWindow.DataContext = new TourLogViewModel(new TourLogModel());
            logWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            if (logWindow.ShowDialog() == true)
            {
                TourLogViewModel submittedData = logWindow.DataContext as TourLogViewModel;
                if (submittedData != null)
                {
                    // Now you can use submittedData to save or process further
                    SaveTourLog(submittedData);
                    MessageBox.Show($"Tour Log successfully added", "Success", MessageBoxButton.OK); 
                }
            }
        }

        private async void SaveTourLog(TourLogViewModel submittedData)
        {
            var tourLogEntity = new TourLogEntity
            {
                TourId = _expandedTour.Id,
                Distance = submittedData.TourLog.Distance,
                TotalTime = submittedData.TourLog.TotalTime,
                Rating = submittedData.TourLog.Rating,
                Comment  = submittedData.TourLog.Comment,
                Difficulty = submittedData.TourLog.Difficulty,
                TourDate = submittedData.TourLog.TourDate,
                

            };
            await _tourRepository.CreateTourLogAsync(tourLogEntity);
            
            _expandedTour.LoadLogs();
        }

        private void LoadTours()
        {
            var tourEntities = _tourRepository.GetTours();
            if (tourEntities != null)
            {
                var tourModels = tourEntities.Select(entity => new TourModel(entity));
                Tours = new ObservableCollection<TourViewModel>(
                    tourModels.Select(model => new TourViewModel(model)));
        }}

      
        private ICommand _expandedCommand;
        public ICommand ExpandedCommand
        {
            get { return _expandedCommand; }
            set {
                _expandedCommand = value;
                OnPropertyChanged(nameof(ExpandedCommand));
        }}

        private ICommand _tourCreateCommand;
        public ICommand TourCreateCommand
        {
            get { return _tourCreateCommand; }
            set {
                _tourCreateCommand = value;
                OnPropertyChanged(nameof(TourCreateCommand));
        }}

        private ICommand _tourEditCommand;
        public ICommand TourEditCommand
        {
            get { return _tourEditCommand; }
            set {
                _tourEditCommand = value;
                OnPropertyChanged(nameof(TourEditCommand));
        }}

        private ICommand _tourReportCommand;
        public ICommand TourReportCommand
        {
            get { return _tourReportCommand; }
            set {
                _tourReportCommand = value;
                OnPropertyChanged(nameof(TourReportCommand));
        }}

        private ICommand _exportCommand;
        public ICommand ExportCommand
        {
            get { return _exportCommand; }
            set {
                _exportCommand = value;
                OnPropertyChanged(nameof(ExportCommand));
        }}

        private ICommand _tourDeleteCommand;
        public ICommand TourDeleteCommand
        {
            get { return _tourDeleteCommand; }
            set {
                _tourDeleteCommand = value;
                OnPropertyChanged(nameof(TourDeleteCommand));
        }}

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get { return _saveCommand; }
            set {
                _saveCommand = value;
                OnPropertyChanged(nameof(SaveCommand));
            }
        }

        private ICommand _editLogCommand;

        public ICommand EditLogCommand
        {
            get { return _editLogCommand; }
            set
            {
                _editLogCommand = value;
                OnPropertyChanged(nameof(EditLogCommand));
            }
        }

        private ICommand _deleteLogCommand;

        public ICommand DeleteLogCommand
        {
            get { return _deleteLogCommand; }
            set
            {
                _deleteLogCommand = value;
                OnPropertyChanged(nameof(DeleteLogCommand));
            }
        }

        private ICommand _addLogCommand;

        public ICommand AddLogCommand
        {
            get { return _addLogCommand; }
            set
            {
                _addLogCommand = value;
                OnPropertyChanged(nameof(AddLogCommand));
            }
        }


        private TourViewModel? _expandedTour = null;
        public TourViewModel ExpandedTour
        {
            get { return _expandedTour; }
            set {
                if (_expandedTour != value) {
                    if (_expandedTour != null) {
                        _expandedTour.PropertyChanged -= TourViewModel_PropertyChanged; // Unsubscribe from the previously expanded tour
                        _expandedTour.ClearLogs();
                    }
                    _expandedTour = value;
                    OnPropertyChanged(nameof(ExpandedTour));
                    if (_expandedTour != null) {
                        _expandedTour.PropertyChanged += TourViewModel_PropertyChanged; // Subscribe to the newly expanded tour
                        _expandedTour.LoadLogs();
                    }
                }
            }
        }

        private void TourViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsExpanded") {
                var tourViewModel = sender as TourViewModel;

                if (tourViewModel != null && !tourViewModel.IsExpanded) {
                    // If the IsExpanded property of a TourViewModel becomes false, set ExpandedTour to null
                    ExpandedTour = null;
        }}}
    
        private void ExpandTour(object parameter)
        {
            if (parameter is TreeViewItem treeViewItem) {
                var tourViewModel = treeViewItem.DataContext as TourViewModel;
                
                if (tourViewModel != null) {
                    ExpandedTour = tourViewModel;

                    foreach (var item in Tours) {
                        if (item != tourViewModel) {
                            if (item != null) {
                                item.IsExpanded = false;
        }}}}}}

        private void CreateTour(object parameter)
        { 
            FormTour = new TourViewModel(new TourModel());    
        }

        private async void EditTour(object parameter)
        {
            if(parameter is TreeViewItem treeViewItem) {
                var tourViewModel = treeViewItem.DataContext as TourViewModel;
                
                if (tourViewModel != null) {
                    // get Data for this tour of DB and make a new tourviewmodel/ a copy so data of listet tour is not changed before saved in DB
                    var getTourEntity = _tourRepository.GetTourByIdAsync(tourViewModel.Id);
                    var tourEntity = await getTourEntity;

                    if (tourEntity != null) {
                        FormTour = new TourViewModel ( new TourModel(tourEntity));
        }}}}

        private void ReportTour(object parameter)
        {

        }

        private void ExportTour(object parameter)
        {

        }

        private async void DeleteTour(object parameter)
        {
            TourViewModel tourViewModel = null;

            if (parameter is TreeViewItem treeViewItem) {
                tourViewModel = (TourViewModel)(treeViewItem.DataContext);
            } 
            else if (parameter is TourViewModel) {
                tourViewModel = (TourViewModel)parameter;
            }
            if (tourViewModel != null) {
                    await _tourRepository.DeleteTourByIdAsync(tourViewModel.Id);
                     LoadTours();
                     MessageBox.Show($"Tour successfully deleted");                   
        }}

        private async void SaveTour(object parameter)
        {
            // if error don't save
            if (FormTour.HasErrors) {
                MessageBox.Show($"Tour could not be saved, first handle the errors");
                return;
            }

            // Update
            if (FormTour.Tour.IsNew == null) {
                await _tourRepository.UpdateTourAsync(_tourMapper.TourModelToEntity(FormTour.Tour));
                FormTour = null;
                LoadTours();
                MessageBox.Show($"Changes to the tour have been successfully applied");
                OnUpdateCompleted(EventArgs.Empty);
            } 
            // Create
            else {
                await _tourRepository.CreateTourAsync(_tourMapper.TourModelToEntity(FormTour.Tour));
                FormTour = null;
                LoadTours();
                MessageBox.Show($"New tour successfully created");
                OnUpdateCompleted(EventArgs.Empty);
        }}


        public event EventHandler UpdateCompleted;
        protected virtual void OnUpdateCompleted(EventArgs e)
        {
            UpdateCompleted?.Invoke(this, e);
        }


        private TourViewModel? _formTour = null;
        public TourViewModel FormTour
        {
            get { return _formTour; }
            set {
                if (_formTour != value) {
                    // if (_formTour != null) { _formTour.ErrorsChanged -= FormTour_ErrorsChanged; }
                    _formTour = value;
                    // if (_formTour != null) { _formTour.ErrorsChanged += FormTour_ErrorsChanged; }
                    OnPropertyChanged(nameof(FormTour));
                }
            }
        }
        
        private TourLogViewModel? _formTourLog = new TourLogViewModel(new TourLogModel());

        public TourLogViewModel FormTourLog
        {
            get { return _formTourLog; }
            set
            {
                if (_formTourLog != value)
                {
                    // if (_formTour != null) { _formTour.ErrorsChanged -= FormTour_ErrorsChanged; }
                    _formTourLog = value;
                    // if (_formTour != null) { _formTour.ErrorsChanged += FormTour_ErrorsChanged; }
                    OnPropertyChanged(nameof(FormTourLog));
                }
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /*private void FormTour_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
           
        }*/
    }
}
