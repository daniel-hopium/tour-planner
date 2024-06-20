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
using System.Net;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.IO;
using System.Globalization;
using System.Windows.Shapes;
using System.Security.Cryptography.Xml;

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

        private string api_key = "5b3ce3597851110001cf6248e9475f9d6e4b47b5b37083ff7839b81f";

        private string _uri_project = $"C:\\Users\\anste\\Documents\\Informatik_Bachelor_2022-2025\\Informatik-4.SemesterSS24\\SWEN2\\tour_planer_da\\TourPlanner"; 

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

            CalculateCommand = new RelayCommand(CalculateRoute);
            TourSetCommand = new RelayCommand(SetTour);
            SaveCommand = new RelayCommand(SaveTour);
            SetEditModeCommand = new RelayCommand(SetEditMode);
        }

        public TourViewModel(TourModel tour)
        {
            _tour = tour;
            _tourRepository = TourRepository.Instance;
            _tourMapper = new TourMapper();

            CalculateCommand = new RelayCommand(CalculateRoute);
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

        private double[] _coordinatesStart;
        private double[] _coordinatesEnd;

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

        private Bitmap _bitmap;

        private BitmapImage _map; 
        public BitmapImage Map
        {
            get { return _map; }
            set
            {
                _map = value;
                OnPropertyChanged(nameof(Map));
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


        public void LoadMap()
        {
            Map = new BitmapImage(new Uri(_uri_project + $"{Image}"));
        }


         public IEnumerable GetErrors(string propertyName)
         {
            string pattern = @"^(?:(\d{4,}\s)?[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*(,\s*[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*(\s\d+[a-zA-Z]?)?)?(,\s*[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*)?|([A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*(,\s*[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*(\s\d+[a-zA-Z]?)?)?,\s*[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*))$";

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
                 yield return "'1234 City, Street 12, Country' (street&country or street&zip and streetnumber can be omitted)";  // From-Address need to follow the pattern 
            }
             else if (propertyName == "ToAddress" && !Regex.IsMatch(ToAddress, pattern))
             {
                 yield return "'1234 City, Street 12, Country' (street&country or street&zip and streetnumber can be omitted)"; // To-Address need to follow the pattern 
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

        private void CalculateRoute(object parameter)
        {
            CalculateRoute();
        }

        private void CalculateRoute()
        {
            //if(_bitmap != null) { _bitmap.Dispose();  }
            
            (double[] coordinatesStart, double[] bboxStart, bool successStart) = OpenRouteService.GetParametersFromApi(api_key, FromAddress);
            if(successStart) _coordinatesStart = coordinatesStart;
            (double[] coordinatesEnd, double[] bboxEnd, bool successEnd) = OpenRouteService.GetParametersFromApi(api_key, ToAddress);
            if(successEnd) _coordinatesEnd = coordinatesEnd;

            if(successStart && successEnd)
            {
                (double[][] coordinates, double[] bbox, double distance, double duration) = OpenRouteService.GetDirectionsFromApi(api_key, TransportType, $"{coordinatesStart[0].ToString("0.######", CultureInfo.InvariantCulture)},{coordinatesStart[1].ToString("0.######", CultureInfo.InvariantCulture)}", $"{coordinatesEnd[0].ToString("0.######", CultureInfo.InvariantCulture)},{coordinatesEnd[1].ToString("0.######", CultureInfo.InvariantCulture)}");

                Distance = distance / 1000;
                EstimatedTime = (int)duration / 60;

                _bitmap = MapCreator.GenerateImage(coordinates, bbox, coordinatesStart, coordinatesEnd);
                Map =  MapCreator.ConvertBitmapToBitmapImage( _bitmap );                
            }

            else
            {
                MessageBox.Show($"No route found!");

                Distance = 0;
                EstimatedTime = 0;
            }
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
                OnPropertyChanged(nameof(Distance));
                OnPropertyChanged(nameof(EstimatedTime));
                OnPropertyChanged(nameof(TransportType));

                if(_tour.IsNew == null) 
                {
                    Map = new BitmapImage(new Uri(_uri_project + $"{Image}"));
                } else
                {
                    Map = null;
                }
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
                CalculateRoute();

                MapCreator.SaveMap(_bitmap, TransportType.ToString(), $"{_coordinatesStart[0]}_{_coordinatesStart[1]}", $"{_coordinatesEnd[0]}_{_coordinatesEnd[1]}");
                Image = $"/Persistence/Images/map_{TransportType.ToString()}_{_coordinatesStart[0]}_{_coordinatesStart[1]}_{_coordinatesEnd[0]}_{_coordinatesEnd[1]}.png";
                
                await _tourRepository.UpdateTourAsync(_tourMapper.TourModelToEntity(_tour));
                _tour = new TourModel();
                MessageBox.Show($"Changes to the tour have been successfully applied");
                OnUpdateCompleted(EventArgs.Empty);
            }
            // Create
            else
            {
                CalculateRoute();

                MapCreator.SaveMap(_bitmap, TransportType.ToString(), $"{_coordinatesStart[0]}_{_coordinatesStart[1]}", $"{_coordinatesEnd[0]}_{_coordinatesEnd[1]}");
                Image = $"/Persistence/Images/map_{TransportType.ToString()}_{_coordinatesStart[0]}_{_coordinatesStart[1]}_{_coordinatesEnd[0]}_{_coordinatesEnd[1]}.png";

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

        private ICommand _calculateCommand;

        public ICommand CalculateCommand
        {
            get { return _calculateCommand; }
            set
            {
                _calculateCommand = value;
                OnPropertyChanged(nameof(CalculateCommand));
            }
        }
    }
}