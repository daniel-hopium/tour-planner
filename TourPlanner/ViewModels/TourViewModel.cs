using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using log4net;
using TourPlanner.Config;
using TourPlanner.Exceptions;
using TourPlanner.Mapper;
using TourPlanner.Models;
using TourPlanner.Persistence.Repository;
using TourPlanner.UtilsForUnittests;
using TourPlanner.ViewModels.Utils;

namespace TourPlanner.ViewModels;

public class TourViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
  private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

  // Singleton Pattern without Dependency Injection -> all ViewModels able use same TourListControlViewModel
  private readonly IMapCreator _mapCreator;
  private readonly IMessageBoxService _messageBoxService;
  private readonly IOpenRouteService _openRouteService;


  private readonly ITourRepository _tourRepository;

  private Bitmap? _bitmap;

  private ICommand _calculateCommand;
  private double[]? _coordinatesEnd;
  private double[]? _coordinatesStart;

  private bool _isEditMode;

  private bool _isExpanded;

  private bool? _isNew;

  private ICommand _loadLogsCommand;
  private ICommand _loadMapCommand;

  private BitmapImage? _map;
  private ICommand _saveCommand;
  private ICommand _setEditModeCommand;
  private TourModel _tour;
  private ObservableCollection<TourLogViewModel> _tourLogs = new();
  private ICommand _tourSetCommand;

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

  public TourViewModel(ITourRepository tourRepository, TourModel tourModel, IMapCreator mapCreator,
    IMessageBoxService messageBox, IOpenRouteService openRouteService)
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

  public TourModel Tour
  {
    get => _tour;
    private set
    {
      if (_tour != value)
      {
        _tour = value;
        OnPropertyChanged(nameof(Tour));
      }
    }
  }

  public ObservableCollection<TourLogViewModel> TourLogs
  {
    get => _tourLogs;
    set
    {
      _tourLogs = value;
      OnPropertyChanged(nameof(TourLogs));
    }
  }


  public static Array TransportTypes => Enum.GetValues(typeof(TransportType));

  public static TourViewModel Instance { get; } = new();

  public bool? IsNew
  {
    get => _tour.IsNew;
    set
    {
      _tour.IsNew = value;
      OnPropertyChanged(nameof(IsNew));
    }
  }

  public bool IsExpanded
  {
    get => _isExpanded;
    set
    {
      if (_isExpanded != value)
      {
        _isExpanded = value;
        OnPropertyChanged(nameof(IsExpanded));
      }
    }
  }

  public bool IsEditMode
  {
    get => _isEditMode;
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
    get => _tour.Id;
    set
    {
      _tour.Id = value;
      OnPropertyChanged(nameof(Id));
    }
  }

  public string Name
  {
    get => _tour.Name;
    set
    {
      _tour.Name = value;
      ValidateProperty(nameof(Name));
      OnPropertyChanged(nameof(Name));
    }
  }

  public string Description
  {
    get => _tour.Description;
    set
    {
      _tour.Description = value;
      ValidateProperty(nameof(Description));
      OnPropertyChanged(nameof(Description));
    }
  }

  public string FromAddress
  {
    get => _tour.FromAddress;
    set
    {
      _tour.FromAddress = value;
      ValidateProperty(nameof(FromAddress));
      OnPropertyChanged(nameof(FromAddress));
    }
  }

  public string ToAddress
  {
    get => _tour.ToAddress;
    set
    {
      _tour.ToAddress = value;
      ValidateProperty(nameof(ToAddress));
      OnPropertyChanged(nameof(ToAddress));
    }
  }

  public TransportType TransportType
  {
    get => _tour.TransportType;
    set
    {
      _tour.TransportType = value;
      OnPropertyChanged(nameof(TransportType));
    }
  }

  public double Distance
  {
    get => _tour.Distance;
    set
    {
      _tour.Distance = value;
      OnPropertyChanged(nameof(Distance));
    }
  }

  public int EstimatedTime
  {
    get => _tour.EstimatedTime;
    set
    {
      _tour.EstimatedTime = value;
      OnPropertyChanged(nameof(EstimatedTime));
    }
  }

  public BitmapImage? Map
  {
    get => _map;
    set
    {
      _map = value;
      OnPropertyChanged(nameof(Map));
    }
  }

  public string Image
  {
    get => _tour.Image;
    set
    {
      _tour.Image = value;
      OnPropertyChanged(nameof(Image));
    }
  }

  public int Popularity
  {
    get => _tour.Popularity;
    set
    {
      _tour.Popularity = value;
      OnPropertyChanged(nameof(Popularity));
    }
  }

  public int? ChildFriendliness
  {
    get => _tour.ChildFriendliness;
    set
    {
      _tour.ChildFriendliness = value;
      OnPropertyChanged(nameof(ChildFriendliness));
    }
  }

  public ICommand LoadMapCommand
  {
    get => _loadMapCommand;
    set
    {
      _loadMapCommand = value;
      OnPropertyChanged(nameof(LoadMapCommand));
    }
  }

  public ICommand LoadLogsCommand
  {
    get => _loadLogsCommand;
    set
    {
      _loadLogsCommand = value;
      OnPropertyChanged(nameof(LoadLogsCommand));
    }
  }

  public ICommand SaveCommand
  {
    get => _saveCommand;
    set
    {
      _saveCommand = value;
      OnPropertyChanged(nameof(SaveCommand));
    }
  }

  public ICommand TourSetCommand
  {
    get => _tourSetCommand;
    set
    {
      _tourSetCommand = value;
      OnPropertyChanged(nameof(TourSetCommand));
    }
  }

  public ICommand SetEditModeCommand
  {
    get => _setEditModeCommand;
    set
    {
      _setEditModeCommand = value;
      OnPropertyChanged(nameof(SetEditModeCommand));
    }
  }

  public ICommand CalculateCommand
  {
    get => _calculateCommand;
    set
    {
      _calculateCommand = value;
      OnPropertyChanged(nameof(CalculateCommand));
    }
  }

  public IEnumerable GetErrors(string? propertyName)
  {
    var pattern =
      @"^(?:\d{4,}\s[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*(,\s*[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*(\s\d+[a-zA-Z]?)?)?(,\s*[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*)?|([A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*(,\s*[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*(\s\d+[a-zA-Z]?)?)?,\s*[A-Za-zßüöäÜÖÄ]+[A-Za-zßüöäÜÖÄ\s\-]*))$";

    if (propertyName == "Name" && string.IsNullOrEmpty(Name))
      yield return "Name can not be empty";
    else if (propertyName == "Description" && string.IsNullOrEmpty(Description))
      yield return "Description can not be empty";
    else if (propertyName == "FromAddress" && string.IsNullOrEmpty(FromAddress))
      yield return "From-Address can not be empty";
    else if (propertyName == "ToAddress" && string.IsNullOrEmpty(ToAddress))
      yield return "To-Address can not be empty";
    else if (propertyName == "FromAddress" && !Regex.IsMatch(FromAddress, pattern))
      yield return
        "'1234 City, Street 12, Country' (street&country or street&zip and streetnumber can be omitted)"; // From-Address need to follow the pattern 
    else if (propertyName == "ToAddress" && !Regex.IsMatch(ToAddress, pattern))
      yield return
        "'1234 City, Street 12, Country' (street&country or street&zip and streetnumber can be omitted)"; // To-Address need to follow the pattern 
  }

  public bool HasErrors => GetErrors(nameof(Name)).OfType<string>().Any() ||
                           GetErrors(nameof(Description)).OfType<string>().Any() ||
                           GetErrors(nameof(FromAddress)).OfType<string>().Any() ||
                           GetErrors(nameof(ToAddress)).OfType<string>().Any();

  public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

  public event PropertyChangedEventHandler? PropertyChanged;

  private async void LoadLogs(object obj)
  {
    try
    {
      var logs = await _tourRepository.GetLogsByTourIdAsync(Id);

      TourLogs.Clear();
      foreach (var log in logs) TourLogs.Add(new TourLogViewModel(new TourLogModel(log)));
    }
    catch (DALException)
    {
      _messageBoxService.Show("Tour log could not be loaded");
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
      var imagesDirectory = ConfigurationHelper.ImagesDirectory;
      var imagePath = imagesDirectory + $"{Image}";
      var uriString = new Uri(imagePath).AbsoluteUri;

      log.Debug($"Attempting to load map with URI: {uriString}");

      Map = new BitmapImage(new Uri(uriString));
    }
    catch (Exception ex)
    {
      log.Error($"Map could not be loaded: {ex}");
      _messageBoxService.Show("Map could not be loaded");
    }
  }

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

      var (coordinatesStart, successStart) = await _openRouteService.GetParametersFromApi(FromAddress);
      if (successStart) _coordinatesStart = coordinatesStart;
      var (coordinatesEnd, successEnd) = await _openRouteService.GetParametersFromApi(ToAddress);
      if (successEnd) _coordinatesEnd = coordinatesEnd;

      if (successStart && successEnd)
      {
        var (coordinates, bbox, distance, duration) = await _openRouteService.GetDirectionsFromApi(
          TransportType,
          $"{coordinatesStart[0].ToString("0.######", CultureInfo.InvariantCulture)},{coordinatesStart[1].ToString("0.######", CultureInfo.InvariantCulture)}",
          $"{coordinatesEnd[0].ToString("0.######", CultureInfo.InvariantCulture)},{coordinatesEnd[1].ToString("0.######", CultureInfo.InvariantCulture)}");

        Distance = distance / 1000;
        EstimatedTime = (int)duration / 60;

        _bitmap = await _mapCreator.GenerateImageAsync(coordinates, bbox, coordinatesStart, coordinatesEnd);
        Map = _mapCreator.ConvertBitmapToBitmapImage(_bitmap);
      }

      else
      {
        _messageBoxService.Show("No route found!");

        Distance = 0;
        EstimatedTime = 0;
        Map = null;
      }
    }
    catch (UtilsException)
    {
      _messageBoxService.Show("Route could not be loaded");
    }
    catch (Exception ex)
    {
      log.Error($"Unknown Error Calculating Route: {ex}");
    }
  }


  private void SetTour(object parameter)
  {
    if (parameter is TourViewModel tourViewModel)
    {
      Tour = tourViewModel.Tour;
      OnPropertyChanged(nameof(Name));
      OnPropertyChanged(nameof(Description));
      OnPropertyChanged(nameof(FromAddress));
      OnPropertyChanged(nameof(ToAddress));
      OnPropertyChanged(nameof(Distance));
      OnPropertyChanged(nameof(EstimatedTime));
      OnPropertyChanged(nameof(TransportType));

      if (_tour.IsNew == null)
        LoadMap(null);
      else
        Map = null;
    }
  }

  private async void SaveTour(object parameter)
  {
    try
    {
      // if error don't save
      if (HasErrors)
      {
        _messageBoxService.Show("Tour could not be saved, first handle the errors");
        return;
      }

      // calculate route and save map in filesystem
      await CalculateRoute();

      _mapCreator.SaveMap(_bitmap, TransportType.ToString(), $"{_coordinatesStart[0]}_{_coordinatesStart[1]}",
        $"{_coordinatesEnd[0]}_{_coordinatesEnd[1]}");
      Image =
        $"{TransportType}_{_coordinatesStart[0]}_{_coordinatesStart[1]}_{_coordinatesEnd[0]}_{_coordinatesEnd[1]}.png";

      //  Create
      if (_tour.IsNew == true)
      {
        await _tourRepository.CreateTourAsync(TourMapper.TourModelToEntity(_tour, _tourRepository));
        _tour = new TourModel();
        _messageBoxService.Show("New tour successfully created");
        OnUpdateCompleted(EventArgs.Empty);
      }
      // Update
      else
      {
        await _tourRepository.UpdateTourAsync(TourMapper.TourModelToEntity(_tour, _tourRepository));
        _tour = new TourModel();
        _messageBoxService.Show("Changes to the tour have been successfully applied");
        OnUpdateCompleted(EventArgs.Empty);
      }
    }
    catch (DALException)
    {
      _messageBoxService.Show("Tour could not be saved");
    }
    catch (UtilsException)
    {
      _messageBoxService.Show("Route could not be calculated");
    }
    catch (Exception ex)
    {
      log.Error($"Unknown Error saving Tour: {ex}");
    }
  }

  private void SetEditMode(object parameter)
  {
    if (parameter is bool editing) IsEditMode = editing;
  }


  //////////////////////// Commands / Events ///////////////////////////////////////////
  public event EventHandler? UpdateCompleted;

  protected virtual void OnUpdateCompleted(EventArgs e)
  {
    UpdateCompleted?.Invoke(this, e);
  }

  protected virtual void OnPropertyChanged(string propertyName)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  protected virtual void OnErrorsChanged(string propertyName)
  {
    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
  }
}