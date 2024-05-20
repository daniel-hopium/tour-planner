using System;
using System.ComponentModel;
using TourPlanner.Persistence.Repository;
using TourPlanner.Models;
using System.Windows.Input;
using TourPlanner.ViewModels.Utils;
using TourPlanner.Persistence.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows;
using TourPlanner.Views;
using TourPlanner.Mapper;


namespace TourPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private TourListControlViewModel _tourListControlViewModel;

        private readonly ITourRepository _tourRepository; //weg

        public MainViewModel()
        {
            _tourRepository = TourRepository.Instance;            //weg 
            _tourListControlViewModel = TourListControlViewModel.Instance;
            _tourListControlViewModel.NowExpandedTour += TourList_NowExpandedTour;

            AddLogCommand = new RelayCommand(AddLog);
            EditLogCommand = new RelayCommand(EditLog);
            DeleteLogCommand = new RelayCommand(DeleteLog);
        }

        private void TourList_NowExpandedTour(object sender, EventArgs e)
        {
            if(sender is TourViewModel tourViewModel)
            {
                ExpandedTour = tourViewModel;
            } 
            else
            {
                ExpandedTour = null;
            }
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
            get 
            { 
                return _expandedTour; 
            }
            set 
            {
                /*if (_expandedTour != null) {
                    _expandedTour.PropertyChanged -= TourViewModel_PropertyChanged; // Unsubscribe from the previously expanded tour
                    //_expandedTour.ClearLogs();
                }*/
                _expandedTour = value;
                OnPropertyChanged(nameof(ExpandedTour));
                /*if (_expandedTour != null) {
                    _expandedTour.PropertyChanged += TourViewModel_PropertyChanged; // Subscribe to the newly expanded tour
                    //_expandedTour.LoadLogs();
                }*/               
            }
        }


        /*private TourViewModel? _formTour;
         
        public TourViewModel FormTour
        {
            get
            {
                return _formTour;
            }
            set
            {
               if (_formTour != value)
                {
                    // if (_formTour != null) { _formTour.ErrorsChanged -= FormTour_ErrorsChanged; }
                    _formTour = value;
                    // if (_formTour != null) { _formTour.ErrorsChanged += FormTour_ErrorsChanged; }
                    OnPropertyChanged(nameof(FormTour));
                }
            }
        }*/     
             

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
    }
}