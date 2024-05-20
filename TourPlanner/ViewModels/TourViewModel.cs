using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using TourPlanner.Models;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Repository;
using TourPlanner.Persistence.Utils;
using TourPlanner.ViewModels.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TourPlanner.Mapper;

namespace TourPlanner.ViewModels
{
    public class TourViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private TourModel _tour;

        public TourModel Tour
        {
            get { return _tour; }
            private set
            {
                if (_tour != value)
                {
                    _tour = value;
                    OnPropertyChanged(nameof(Tour));
                }
            }
        }

        private ObservableCollection<TourLogViewModel> _tourLogs;

        public ObservableCollection<TourLogViewModel> TourLogs
        {
            get => _tourLogs;
            set
            {
                _tourLogs = value;
                OnPropertyChanged(nameof(TourLogs));
            }
        }

        public Array TransportTypes => Enum.GetValues(typeof(TourPlanner.Models.TransportType));

        private TourMapper _tourMapper;

        private readonly TourRepository _tourRepository;

        // Singleton Pattern without Dependency Injection -> all ViewModels able use same TourListControlViewModel
        private static readonly TourViewModel _instance = new TourViewModel();
        public static TourViewModel Instance
        {
            get
            {
                return _instance;
            }
        }
        static TourViewModel()
        {

        }

        public TourViewModel() // (TourModel tour)
        {
            _tour = new TourModel();
            _tourRepository = TourRepository.Instance;
            _tourMapper = new TourMapper();

            TourSetCommand = new RelayCommand(SetTour);
            SaveCommand = new RelayCommand(SaveTour);
            SetEditModeCommand = new RelayCommand(SetEditMode);
        }

        public TourViewModel(TourModel tour)
        {
            _tour = tour;
            _tourRepository = TourRepository.Instance;
            _tourMapper = new TourMapper();

            SaveCommand = new RelayCommand(SaveTour);
            SetEditModeCommand = new RelayCommand(SetEditMode);
        }


        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }

        private bool _isEditMode;

        public bool IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                if (_isEditMode != value)
                {
                    _isEditMode = value;
                    OnPropertyChanged(nameof(IsEditMode));
                }
            }
        }


        public int Id
        {
            get { return _tour.Id; }
            set
            {
                _tour.Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get { return _tour.Name; }
            set
            {
                _tour.Name = value;
                ValidateProperty(nameof(Name));
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Description
        {
            get { return _tour.Description; }
            set
            {
                _tour.Description = value;
                ValidateProperty(nameof(Description));
                OnPropertyChanged(nameof(Description));
            }
        }

        public string FromAddress
        {
            get { return _tour.FromAddress; }
            set
            {  
                _tour.FromAddress = value;
                ValidateProperty(nameof(FromAddress));
                OnPropertyChanged(nameof(FromAddress));
            }
        }

        public string ToAddress
        {
            get { return _tour.ToAddress; }
            set
            {
                _tour.ToAddress = value;               
                ValidateProperty(nameof(ToAddress));
                OnPropertyChanged(nameof(ToAddress));
            }
        }

        public TourPlanner.Models.TransportType TransportType
        {
            get { return _tour.TransportType; }
            set
            {
                _tour.TransportType = value;
                OnPropertyChanged(nameof(TransportType));
            }
        }

        public double Distance
        {
            get { return _tour.Distance; }
            set
            {
                _tour.Distance = value;
                OnPropertyChanged(nameof(Distance));
            }
        }

        public int EstimatedTime
        {
            get { return _tour.EstimatedTime; }
            set
            {
                _tour.EstimatedTime = value;
                OnPropertyChanged(nameof(EstimatedTime));
            }
        }

        public string Image
        {
            get { return _tour.Image; }
            set
            {
                _tour.Image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        public int Popularity
        {
            get { return _tour.Popularity; }
            set
            {
                _tour.Popularity = value;
                OnPropertyChanged(nameof(Popularity));
            }
        }

        public int ChildFriendliness
        {
            get { return _tour.ChildFriendliness; }
            set
            {
                _tour.ChildFriendliness = value;
                OnPropertyChanged(nameof(ChildFriendliness));
            }
        }

        public async void LoadLogs()
        {
            var logs = await _tourRepository.GetLogsByTourIdAsync(Id);
            TourLogs = new ObservableCollection<TourLogViewModel>(logs.Select(log => new TourLogViewModel(new TourLogModel(log))));
            
            
        }

        public void ClearLogs()
        {
            TourLogs.Clear();
            _tourLogs.Clear();
        }


         public IEnumerable GetErrors(string propertyName)
         {
             string pattern = @"^[A-Za-zßüöä ]+ +\d+[A-Za-z]? *, *\d{4,} +[A-Za-zßüöä ]+$";

             if (propertyName == "Name" && string.IsNullOrEmpty(Name))
             {                
                 yield return "Name can not be empty";
             }
             else if (propertyName == "Description" && string.IsNullOrEmpty(Description))
             {
                 yield return "Description can not be empty";
             }
             else if (propertyName == "FromAddress" && string.IsNullOrEmpty(FromAddress))
             {
                 yield return "From-Address can not be empty";
             }
             else if (propertyName == "ToAddress" && string.IsNullOrEmpty(ToAddress))
             {
                 yield return "To-Address can not be empty";
             }
             else if (propertyName == "FromAddress" && !Regex.IsMatch(FromAddress, pattern))
             {
                 yield return "From-Address need to follow the pattern 'Street 12, 1234 City'";
             }
             else if (propertyName == "ToAddress" && !Regex.IsMatch(ToAddress, pattern))
             {
                 yield return "To-Address need to follow the pattern 'Street 12, 1234 City'";
             }

             yield break;
         }

         public bool HasErrors => GetErrors("Name").OfType<string>().Any() ||
                                  GetErrors("Description").OfType<string>().Any() ||
                                  GetErrors("FromAddress").OfType<string>().Any() ||
                                  GetErrors("ToAddress").OfType<string>().Any(); 

         private void ValidateProperty(string propertyName)
         {
            OnErrorsChanged(propertyName);
        }


        private void SetTour(object parameter)
        {
            if(parameter is TourViewModel tourViewModel)
            {
                Tour = tourViewModel.Tour;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(FromAddress));
                OnPropertyChanged(nameof(ToAddress));
            }          
        }      

        private async void SaveTour(object parameter)
        {
            // if error don't save
            if (HasErrors)
            {
                MessageBox.Show($"Tour could not be saved, first handle the errors");
                return;
            }

            // Update
            if (_tour.IsNew == null)
            {
                await _tourRepository.UpdateTourAsync(_tourMapper.TourModelToEntity(_tour));
                _tour = new TourModel();
                MessageBox.Show($"Changes to the tour have been successfully applied");
                OnUpdateCompleted(EventArgs.Empty);
            }
            // Create
            else
            {
                await _tourRepository.CreateTourAsync(_tourMapper.TourModelToEntity(_tour));
                _tour = new TourModel();
                MessageBox.Show($"New tour successfully created");
                OnUpdateCompleted(EventArgs.Empty);
            }
        }

        private void SetEditMode(object parameter)
        {
            if (parameter is bool editing)
            {
                IsEditMode = editing;
            }
           
        }

        public event EventHandler UpdateCompleted;

        protected virtual void OnUpdateCompleted(EventArgs e)
        {
            UpdateCompleted?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
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

        private ICommand _tourSetCommand;

        public ICommand TourSetCommand
        {
            get { return _tourSetCommand; }
            set
            {
                _tourSetCommand = value;
                OnPropertyChanged(nameof(TourSetCommand));
            }
        }

        private ICommand _setEditModeCommand;

        public ICommand SetEditModeCommand
        {
            get { return _setEditModeCommand; }
            set
            {
                _setEditModeCommand = value;
                OnPropertyChanged(nameof(SetEditModeCommand));
            }
        }
    }
}