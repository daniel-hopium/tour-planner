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
using System.Configuration;
using log4net;
using TourPlanner.Exceptions;
using System.Reflection;
using TourPlanner.UtilsForUnittests;

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


        private ObservableCollection<TourLogViewModel> _tourLogs = new();

        public ObservableCollection<TourLogViewModel> TourLogs
        {
            get => _tourLogs;
            set
            {
                _tourLogs = value;
                OnPropertyChanged(nameof(TourLogs));
            }
        }


        public static Array TransportTypes => Enum.GetValues(typeof(TourPlanner.Models.TransportType));


        private readonly ITourRepository _tourRepository;
        private readonly IMapCreator _mapCreator;
        private readonly IMessageBoxService _messageBoxService;
        private readonly IOpenRouteService _openRouteService;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Singleton Pattern without Dependency Injection -> all ViewModels able use same TourListControlViewModel
        private static readonly TourViewModel _instance = new ();
        public static TourViewModel Instance
        {
            get
            {
                return _instance;
            }
        }


        public TourViewModel() 
        {
            _tour = new TourModel();
            _tourRepository = TourRepository.Instance;
            _openRouteService = new OpenRouteService();
            _mapCreator = new MapCreator(_openRouteService);
            _messageBoxService = new MessageBoxService();
            
            _loadMapCommand = new RelayCommand(LoadMap);
            _loadLogsCommand = new RelayCommand(LoadLogs);
            _calculateCommand = new RelayCommand(CalculateRoute);
            _tourSetCommand = new RelayCommand(SetTour);
            _saveCommand = new RelayCommand(SaveTour);
            _setEditModeCommand = new RelayCommand(SetEditMode);
        }

        public TourViewModel(TourModel tour)
        {
            _tour = tour;
            _tourRepository = TourRepository.Instance;
            _openRouteService = new OpenRouteService();
            _mapCreator = new MapCreator(_openRouteService);
            _messageBoxService = new MessageBoxService();
            
            _loadMapCommand = new RelayCommand(LoadMap);
            _loadLogsCommand = new RelayCommand(LoadLogs);
            _tourSetCommand = new RelayCommand(SetTour);
            _calculateCommand = new RelayCommand(CalculateRoute);
            _saveCommand = new RelayCommand(SaveTour);
            _setEditModeCommand = new RelayCommand(SetEditMode);
        }
        public TourViewModel(ITourRepository tourRepository, TourModel tourModel, IMapCreator mapCreator, IMessageBoxService messageBox, IOpenRouteService openRouteService)
        {
            _tour = tourModel;
            _tourRepository = tourRepository;
            _mapCreator = mapCreator;
            _messageBoxService = messageBox;
            _openRouteService = openRouteService;

            _loadMapCommand = new RelayCommand(LoadMap);
            _loadLogsCommand = new RelayCommand(LoadLogs);
            _calculateCommand = new RelayCommand(CalculateRoute);
            _tourSetCommand = new RelayCommand(SetTour);
            _saveCommand = new RelayCommand(SaveTour);
            _setEditModeCommand = new RelayCommand(SetEditMode);
        }


        private bool? _isNew;
        public bool? IsNew
        {
            get { return _tour.IsNew; }
            set
            {
                 _tour.IsNew = value;
                 OnPropertyChanged(nameof(IsNew));
            }
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

        private double[]? _coordinatesStart;
        private double[]? _coordinatesEnd;

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

        private Bitmap? _bitmap;

        private BitmapImage? _map; 
        public BitmapImage? Map
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

        public int? ChildFriendliness
        {
            get { return _tour.ChildFriendliness; }
            set
            {
                _tour.ChildFriendliness = value;
                OnPropertyChanged(nameof(ChildFriendliness));
            }
        }

        private async void LoadLogs(object obj)
        {
            try
            {
                var logs = await _tourRepository.GetLogsByTourIdAsync(Id);

                TourLogs.Clear();
                foreach (var log in logs)
                {
                    TourLogs.Add(new TourLogViewModel(new TourLogModel(log)));
                }
            }
            catch (DALException)
            {
                _messageBoxService.Show($"Tour log could not be loaded");
            }
            catch (Exception ex)
            {
                log.Error($"Unknown error loading tour log: {ex}");
            }
        }

        private void LoadMap(object obj)
        {
            try
            {
                Map = new BitmapImage(new Uri(ConfigurationManager.AppSettings["ImagesDirectory"] + $"{Image}"));
            }
            catch (Exception ex)
            {
                log.Error($"Map could not be loaded: {ex}");
                _messageBoxService.Show($"Map could not be loaded");
            }
        }


         public IEnumerable GetErrors(string? propertyName)
         {
            string pattern = @"^(?:\d{4,}\s[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*(,\s*[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*(\s\d+[a-zA-Z]?)?)?(,\s*[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*)?|([A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*(,\s*[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*(\s\d+[a-zA-Z]?)?)?,\s*[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*))$";

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

         public bool HasErrors => GetErrors(nameof(Name)).OfType<string>().Any() ||
                                  GetErrors(nameof(Description)).OfType<string>().Any() ||
                                  GetErrors(nameof(FromAddress)).OfType<string>().Any() ||
                                  GetErrors(nameof(ToAddress)).OfType<string>().Any(); 

         private void ValidateProperty(string propertyName)
         {
            OnErrorsChanged(propertyName);
        }

        private async void CalculateRoute(object parameter)
        {
            await CalculateRoute();
        }

        private async Task CalculateRoute()
        {
            try
            {
                _bitmap?.Dispose();

                (double[] coordinatesStart, bool successStart) = await _openRouteService.GetParametersFromApi(FromAddress);
                if (successStart) _coordinatesStart = coordinatesStart;
                (double[] coordinatesEnd, bool successEnd) = await _openRouteService.GetParametersFromApi(ToAddress);
                if (successEnd) _coordinatesEnd = coordinatesEnd;

                if (successStart && successEnd)
                {
                    (double[][] coordinates, double[] bbox, double distance, double duration) = await _openRouteService.GetDirectionsFromApi(TransportType, $"{coordinatesStart[0].ToString("0.######", CultureInfo.InvariantCulture)},{coordinatesStart[1].ToString("0.######", CultureInfo.InvariantCulture)}", $"{coordinatesEnd[0].ToString("0.######", CultureInfo.InvariantCulture)},{coordinatesEnd[1].ToString("0.######", CultureInfo.InvariantCulture)}");

                    Distance = distance / 1000;
                    EstimatedTime = (int)duration / 60;

                    _bitmap = await _mapCreator.GenerateImageAsync(coordinates, bbox, coordinatesStart, coordinatesEnd);
                    Map = _mapCreator.ConvertBitmapToBitmapImage(_bitmap);
                }

                else
                {
                    _messageBoxService.Show($"No route found!");

                    Distance = 0;
                    EstimatedTime = 0;
                    Map = null;
                }
            }
            catch (UtilsException)
            {
                _messageBoxService.Show($"Route could not be loaded");
            }
            catch (Exception ex)
            {
                log.Error($"Unknown Error Calculating Route: {ex}");
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
                    LoadMap(null);
                } else
                {
                    Map = null;
                }
            }          
        }      

        private async void SaveTour(object parameter)
        {
            try
            {
                // if error don't save
                if (HasErrors)
                {
                    _messageBoxService.Show($"Tour could not be saved, first handle the errors");
                    return;
                }

                // calculate route and save map in filesystem
                await CalculateRoute();

                _mapCreator.SaveMap(_bitmap, TransportType.ToString(), $"{_coordinatesStart[0]}_{_coordinatesStart[1]}", $"{_coordinatesEnd[0]}_{_coordinatesEnd[1]}");
                Image = $"{TransportType}_{_coordinatesStart[0]}_{_coordinatesStart[1]}_{_coordinatesEnd[0]}_{_coordinatesEnd[1]}.png";

                //  Create
                if (_tour.IsNew == true)
                {                   
                   await _tourRepository.CreateTourAsync(TourMapper.TourModelToEntity(_tour, _tourRepository));
                    _tour = new TourModel();
                    _messageBoxService.Show($"New tour successfully created");
                    OnUpdateCompleted(EventArgs.Empty);
                }
                // Update
                else
                {
                     await _tourRepository.UpdateTourAsync(TourMapper.TourModelToEntity(_tour, _tourRepository));
                    _tour = new TourModel();
                    _messageBoxService.Show($"Changes to the tour have been successfully applied");
                    OnUpdateCompleted(EventArgs.Empty);              
                }
            }
            catch (DALException)
            {
                _messageBoxService.Show($"Tour could not be saved");
            }
            catch (UtilsException)
            {
                _messageBoxService.Show($"Route could not be calculated");
            }
            catch (Exception ex)
            {
                log.Error($"Unknown Error saving Tour: {ex}");
            }
        }

        private void SetEditMode(object parameter)
        {
            if (parameter is bool editing)
            {
                IsEditMode = editing;
            }
           
        }



        //////////////////////// Commands / Events ///////////////////////////////////////////
        ///

        public event EventHandler? UpdateCompleted;

        protected virtual void OnUpdateCompleted(EventArgs e)
        {
            UpdateCompleted?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }


        private ICommand _loadMapCommand;
        public ICommand LoadMapCommand
        {
            get { return _loadMapCommand; }
            set
            {
                _loadMapCommand = value;
                OnPropertyChanged(nameof(LoadMapCommand));
            }
        }

        private ICommand _loadLogsCommand;
        public ICommand LoadLogsCommand
        {
            get => _loadLogsCommand;
            set
            {
                _loadLogsCommand = value;
                OnPropertyChanged(nameof(LoadLogsCommand));
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


        //////////////////////// Commands / Events ///////////////////////////////////////////
    }
}