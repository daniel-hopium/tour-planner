using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using log4net;
using TourPlanner.Exceptions;
using TourPlanner.Models;
using TourPlanner.Persistence.Repository;
using TourPlanner.UtilsForUnittests;
using TourPlanner.ViewModels.Utils;

namespace TourPlanner.ViewModels;

public class TourListControlViewModel : INotifyPropertyChanged, ITourListControlViewModel
{
  private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

  // Singleton Pattern without Dependency Injection -> all ViewModels able use same TourListControlViewModel
  private readonly IMessageBoxService _messageBoxService;

  private readonly ITourRepository _tourRepository;


  private ICommand _expandedCommand;


  //////////////////////// Tour Expanding funcionality (just one tour at the same time expanded) ///////////////////////////////////////////
  private TourViewModel? _expandedTour;

  private ICommand _exportCommand;

  private ObservableCollection<TourViewModel> _filteredTours = new();

  private ICommand _loadToursCommand;

  private ICommand _resetEditModeCommand;

  private string _searchText = "";

  private ICommand _tourDeleteCommand;

  private ICommand _tourEditCommand;


  private ICommand _tourReportCommand;


  private ObservableCollection<TourViewModel> _tours = new();


  public TourListControlViewModel()
  {
    _tourRepository = TourRepository.Instance;
    _messageBoxService = new MessageBoxService();
    LoadTours();

    _expandedCommand = new RelayCommand(ExpandTour);
    _tourEditCommand = new RelayCommand(EditTour);
    _tourReportCommand = new RelayCommand(ReportTour);
    _exportCommand = new RelayCommand(ExportTour);
    _tourDeleteCommand = new RelayCommand(DeleteTour);
    _loadToursCommand = new RelayCommand(LoadToursFromOutside);
    _resetEditModeCommand = new RelayCommand(ResetEditModeTours);
  }

  public TourListControlViewModel(ITourRepository tourRepository, IMessageBoxService messageBoxService)
  {
    _tourRepository = tourRepository;
    _messageBoxService = messageBoxService;

    _expandedCommand = new RelayCommand(ExpandTour);
    _tourEditCommand = new RelayCommand(EditTour);
    _tourReportCommand = new RelayCommand(ReportTour);
    _exportCommand = new RelayCommand(ExportTour);
    _tourDeleteCommand = new RelayCommand(DeleteTour);
    _loadToursCommand = new RelayCommand(LoadToursFromOutside);
    _resetEditModeCommand = new RelayCommand(ResetEditModeTours);
  }

  public static TourListControlViewModel Instance { get; } = new();

  public ObservableCollection<TourViewModel> Tours
  {
    get => _tours;
    set
    {
      _tours = value;
      OnPropertyChanged(nameof(Tours));
    }
  }

  public ObservableCollection<TourViewModel> FilteredTours
  {
    get => _filteredTours;
    set
    {
      _filteredTours = value;
      OnPropertyChanged(nameof(FilteredTours));
    }
  }

  public string SearchText
  {
    get => _searchText;
    set
    {
      if (_searchText != value)
      {
        _searchText = value;
        OnPropertyChanged(nameof(SearchText));
        PerformSearch();
      }
    }
  }

  public TourViewModel? ExpandedTour
  {
    get => _expandedTour;
    set
    {
      if (_expandedTour != value)
      {
        if (_expandedTour != null)
          _expandedTour.PropertyChanged -=
            TourViewModel_PropertyChanged; // Unsubscribe from the previously expanded tour

        _expandedTour = value;
        OnPropertyChanged(nameof(ExpandedTour));

        if (_expandedTour != null)
        {
          _expandedTour.PropertyChanged +=
            TourViewModel_PropertyChanged; // Subscribe to the newly expanded tour
          _expandedTour.LoadLogsCommand.Execute(null);
          _expandedTour.LoadMapCommand.Execute(null);
        }

        OnNowExpandedTour(_expandedTour);
      }
    }
  }


  //////////////////////// Commands / Events ///////////////////////////////////////////
  public event PropertyChangedEventHandler? PropertyChanged;

  public event EventHandler? NowExpandedTour;


  //////////////////////// Action Bar on Tour in List ///////////////////////////////////////////
  public event EventHandler? TourToEditSelected;

  public ICommand ExpandedCommand
  {
    get => _expandedCommand;
    set
    {
      _expandedCommand = value;
      OnPropertyChanged(nameof(ExpandedCommand));
    }
  }

  public ICommand TourEditCommand
  {
    get => _tourEditCommand;
    set
    {
      _tourEditCommand = value;
      OnPropertyChanged(nameof(TourEditCommand));
    }
  }

  public ICommand TourReportCommand
  {
    get => _tourReportCommand;
    set
    {
      _tourReportCommand = value;
      OnPropertyChanged(nameof(TourReportCommand));
    }
  }

  public ICommand ExportCommand
  {
    get => _exportCommand;
    set
    {
      _exportCommand = value;
      OnPropertyChanged(nameof(ExportCommand));
    }
  }

  public ICommand TourDeleteCommand
  {
    get => _tourDeleteCommand;
    set
    {
      _tourDeleteCommand = value;
      OnPropertyChanged(nameof(TourDeleteCommand));
    }
  }

  public ICommand LoadToursCommand
  {
    get => _loadToursCommand;
    set
    {
      _loadToursCommand = value;
      OnPropertyChanged(nameof(LoadToursCommand));
    }
  }

  public ICommand ResetEditModeCommand
  {
    get => _resetEditModeCommand;
    set
    {
      _resetEditModeCommand = value;
      OnPropertyChanged(nameof(ResetEditModeCommand));
    }
  }

  internal void PerformSearch()
  {
    if (string.IsNullOrWhiteSpace(SearchText))
    {
      FilteredTours = new ObservableCollection<TourViewModel>(Tours);
    }
    else
    {
      var lowerSearchText = SearchText.ToLower();
      var searchResults = Tours.Where(t =>
        t.Name.ToLower().Contains(lowerSearchText) ||
        t.Description.ToLower().Contains(lowerSearchText) ||
        t.FromAddress.ToLower().Contains(lowerSearchText) ||
        t.ToAddress.ToLower().Contains(lowerSearchText) ||
        t.TransportType.ToString().ToLower().Contains(lowerSearchText) ||
        t.TourLogs.Any(log =>
          log.Comment.ToLower().Contains(lowerSearchText) || 
          log.Rating.ToString().Contains(lowerSearchText) ||
          log.Difficulty.ToString().Contains(lowerSearchText) ||
          log.TotalTime.ToString().Contains(lowerSearchText) ||
          log.TourDate.ToString().Contains(lowerSearchText))
      ).ToList();

      // Ranking based on popularity and child-friendliness
      var rankedResults = searchResults
        .OrderByDescending(t => t.Popularity)
        .ThenByDescending(t => t.ChildFriendliness)
        .ToList();

      FilteredTours = new ObservableCollection<TourViewModel>(rankedResults);
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
      foreach (var tour in tourModels) Tours.Add(new TourViewModel(tour));
      PerformSearch();

      if (Tours.Any())
        //Tours.First().IsExpanded = true;
        FilteredTours.First().IsExpanded = true;
      else
        _messageBoxService.Show("No Tours created yet");
    }
    catch (DALException)
    {
      // _messageBoxService.Show($"Tours could not be loaded");  // Why when closing window or loading Tours after Logchange
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


  private void TourViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == "IsExpanded")
      if (sender is TourViewModel tourViewModel && !tourViewModel.IsExpanded)
        ExpandedTour =
          null; // If the IsExpanded property of a TourViewModel becomes false, set ExpandedTour to null
  }

  private void ExpandTour(object parameter)
  {
    if (parameter is TreeViewItem treeViewItem)
      if (treeViewItem.DataContext is TourViewModel tourViewModel)
      {
        ExpandedTour = tourViewModel;

        foreach (var item in Tours)
          if (item != tourViewModel && item != null)
            item.IsExpanded = false;
      }
  }

  protected virtual void OnNowExpandedTour(object? sender)
  {
    NowExpandedTour?.Invoke(sender, EventArgs.Empty);
  }

  protected virtual void SelectTourToEdit(object? sender)
  {
    TourToEditSelected?.Invoke(sender, EventArgs.Empty);
  }

  private async void EditTour(object parameter)
  {
    try
    {
      if (parameter is TreeViewItem treeViewItem)
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
    catch (DALException)
    {
      _messageBoxService.Show("Tour to edit could not be found");
    }
    catch (Exception ex)
    {
      log.Error($"Unknown error selecting tour to edit: {ex}");
    }
  }

  private void ResetEditModeTours(object parameter)
  {
    foreach (var tour in Tours) tour?.SetEditModeCommand.Execute(false);
  }

  private async void ReportTour(object parameter)
  {
    try
    {
      if (parameter is TreeViewItem treeViewItem)
        if (treeViewItem.DataContext is TourViewModel tourViewModel)
        {
          await _tourRepository.GenerateTourReport(tourViewModel.Id);
          _messageBoxService.Show($"Report for {tourViewModel.Name} generated successfully.");
        }
    }
    catch (DALException)
    {
      _messageBoxService.Show("Data for Tour-Report could not be processed");
    }
    catch (UtilsException)
    {
      _messageBoxService.Show("Tour-Report could not be generated");
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
        if (treeViewItem.DataContext is TourViewModel tourViewModel)
        {
          await _tourRepository.GenerateTourExportAsync(tourViewModel.Id);
          _messageBoxService.Show($"Tour {tourViewModel.Name} successfully exported");
        }
    }
    catch (DALException)
    {
      _messageBoxService.Show("Tour data for Export could not be loaded");
    }
    catch (UtilsException)
    {
      _messageBoxService.Show("Tour-Export could not be generated");
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
        if (treeViewItem.DataContext is TourViewModel tourViewModel)
        {
          await _tourRepository.DeleteTourByIdAsync(tourViewModel.Id);
          LoadTours();
          _messageBoxService.Show("Tour successfully deleted");
        }
    }
    catch (DALException)
    {
      _messageBoxService.Show("Tour could not be deleted");
    }
    catch (Exception ex)
    {
      log.Error($"Unknown error during deletion of Tour: {ex}");
    }
  }

  protected virtual void OnPropertyChanged(string propertyName)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}