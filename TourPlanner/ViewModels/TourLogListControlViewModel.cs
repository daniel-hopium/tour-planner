using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using log4net;
using TourPlanner.Exceptions;
using TourPlanner.Models;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Repository;
using TourPlanner.UtilsForUnittests;
using TourPlanner.ViewModels.Utils;
using TourPlanner.Views;

namespace TourPlanner.ViewModels;

public class TourLogListControlViewModel : INotifyPropertyChanged, ITourLogListControlViewModel
{
  private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

  private readonly IMessageBoxService _messageBoxService;
  private readonly ITourRepository _tourRepository;

  private ICommand _addLogCommand;


  private ICommand _changeTourCommand;

  private ICommand _deleteLogCommand;


  private ICommand _editLogCommand;


  private TourViewModel? _tour;


  public TourLogListControlViewModel()
  {
    _tourRepository = TourRepository.Instance;
    _messageBoxService = new MessageBoxService();

    _changeTourCommand = new RelayCommand(ChangeTour);
    _addLogCommand = new RelayCommand(AddLog);
    _editLogCommand = new RelayCommand(EditLog);
    _deleteLogCommand = new RelayCommand(DeleteLog);
  }

  public static TourLogListControlViewModel Instance { get; } = new();

  public TourViewModel? Tour
  {
    get => _tour;
    set
    {
      _tour = value;
      OnPropertyChanged(nameof(Tour));
    }
  }

  public event PropertyChangedEventHandler? PropertyChanged;


  //////////////////////// Commands / Events ///////////////////////////////////////////
  public event EventHandler? LogsChanged;

  public ICommand ChangeTourCommand
  {
    get => _changeTourCommand;
    set
    {
      _changeTourCommand = value;
      OnPropertyChanged(nameof(ChangeTourCommand));
    }
  }

  public ICommand EditLogCommand
  {
    get => _editLogCommand;
    set
    {
      _editLogCommand = value;
      OnPropertyChanged(nameof(EditLogCommand));
    }
  }

  public ICommand DeleteLogCommand
  {
    get => _deleteLogCommand;
    set
    {
      _deleteLogCommand = value;
      OnPropertyChanged(nameof(DeleteLogCommand));
    }
  }

  public ICommand AddLogCommand
  {
    get => _addLogCommand;
    set
    {
      _addLogCommand = value;
      OnPropertyChanged(nameof(AddLogCommand));
    }
  }


  private void ChangeTour(object obj)
  {
    if (obj is TourViewModel tour) Tour = tour;
  }

  private async void DeleteLog(object obj)
  {
    try
    {
      int tourId;
      if (obj is TourLogViewModel tourLogViewModel)
      {
        tourId = tourLogViewModel.TourId;
        await _tourRepository.DeleteTourLogByIdAsync(tourLogViewModel.Id);
        LogsChangedNow(tourId);
        _messageBoxService.Show("Tour log successfully deleted");
      }
    }
    catch (DALException)
    {
      _messageBoxService.Show("Tour log could not be deleted");
    }
    catch (Exception ex)
    {
      log.Error($"Unknown error deleting tour log: {ex}");
    }
  }

  private async void EditLog(object obj)
  {
    try
    {
      LogWindow logWindow = new();

      if (obj is TourLogViewModel tourLogViewModel)
        logWindow.DataContext = new TourLogViewModel(tourLogViewModel.TourLog);
      logWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

      if (logWindow.ShowDialog() == true)
        if (logWindow.DataContext is TourLogViewModel
            submittedData) // Now you can use submittedData to save or process further
        {
          await UpdateTourLog(submittedData);
          LogsChangedNow(submittedData.TourId);
          _messageBoxService.Show("Changes to the tour log have been successfully applied");
        }

      logWindow.DataContext = null;
    }
    catch (DALException)
    {
      _messageBoxService.Show("Tour log could not be updated");
    }
    catch (Exception ex)
    {
      log.Error($"Unknown error updating tour log: {ex}");
    }
  }

  private async Task UpdateTourLog(TourLogViewModel submittedData)
  {
    var tourLogEntity = new TourLogEntity
    {
      Id = submittedData.Id,
      TourId = submittedData.TourId,
      Distance = submittedData.TourLog.Distance,
      TotalTime = submittedData.TourLog.TotalTime,
      Rating = submittedData.TourLog.Rating,
      Comment = submittedData.TourLog.Comment,
      Difficulty = submittedData.TourLog.Difficulty,
      TourDate = submittedData.TourLog.TourDate
    };
    await _tourRepository.UpdateTourLogAsync(tourLogEntity);
  }

  private async void AddLog(object obj)
  {
    try
    {
      LogWindow logWindow = new()
      {
        DataContext = new TourLogViewModel(new TourLogModel()),
        WindowStartupLocation = WindowStartupLocation.CenterOwner
      };

      if (logWindow.ShowDialog() == true)
        if (logWindow.DataContext is TourLogViewModel submittedData)
        {
          await SaveTourLog(submittedData);
          LogsChangedNow(submittedData.TourId);
          _messageBoxService.Show("New tour log successfully created");
        }
    }
    catch (DALException)
    {
      _messageBoxService.Show("Tour log could not be created");
    }
    catch (Exception ex)
    {
      log.Error($"Unknown error creating tour log: {ex}");
    }
  }

  private async Task SaveTourLog(TourLogViewModel submittedData)
  {
    var tourLogEntity = new TourLogEntity
    {
      TourId = Tour.Id,
      Distance = submittedData.TourLog.Distance,
      TotalTime = submittedData.TourLog.TotalTime,
      Rating = submittedData.TourLog.Rating,
      Comment = submittedData.TourLog.Comment,
      Difficulty = submittedData.TourLog.Difficulty,
      TourDate = submittedData.TourLog.TourDate
    };
    await _tourRepository.CreateTourLogAsync(tourLogEntity);
  }

  protected virtual void LogsChangedNow(object sender)
  {
    LogsChanged?.Invoke(sender, EventArgs.Empty);
  }

  protected virtual void OnPropertyChanged(string propertyName)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }
}