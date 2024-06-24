using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using TourPlanner.Models;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Repository;
using TourPlanner.ViewModels.Utils;
using TourPlanner.Views;
using System.ComponentModel;
using System.Collections.ObjectModel;
using TourPlanner.Persistence.Utils;
using log4net;
using TourPlanner.Exceptions;
using System.Reflection;

namespace TourPlanner.ViewModels
{
    public class TourLogListControlViewModel : INotifyPropertyChanged
    {
        private readonly ITourRepository _tourRepository;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly TourLogListControlViewModel _instance = new();
        public static TourLogListControlViewModel Instance
        {
            get { return _instance; }
        }


        public TourLogListControlViewModel()
        {
            _tourRepository = TourRepository.Instance;

            _changeTourCommand = new RelayCommand(ChangeTour);
            _addLogCommand = new RelayCommand(AddLog);
            _editLogCommand = new RelayCommand(EditLog);
            _deleteLogCommand = new RelayCommand(DeleteLog);
        }


        private TourViewModel? _tour;
        public TourViewModel? Tour
        {
            get => _tour;
            set
            {
                _tour = value;
                OnPropertyChanged(nameof(Tour));
            }
        }


        private void ChangeTour(object obj)
        {
            if (obj is TourViewModel tour)
            {
                Tour = tour;
            }
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
                    MessageBox.Show($"Tour log successfully deleted");
                }
            }
            catch (DALException)
            {
                MessageBox.Show($"Tour log could not be deleted");
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
                LogWindow logWindow = new ();

                if (obj is TourLogViewModel tourLogViewModel)
                    logWindow.DataContext = new TourLogViewModel(tourLogViewModel.TourLog);
                logWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                if (logWindow.ShowDialog() == true)
                {
                    if (logWindow.DataContext is TourLogViewModel submittedData) // Now you can use submittedData to save or process further
                    {                       
                        await UpdateTourLog(submittedData);
                        LogsChangedNow(submittedData.TourId);
                        MessageBox.Show($"Changes to the tour log have been successfully applied");
                    }
                }
                logWindow.DataContext = null;
            }
            catch (DALException)
            {
                MessageBox.Show($"Tour log could not be updated");
            }
            catch (Exception ex)
            {
                log.Error($"Unknown error updating tour log: {ex}");
            }
        }

        private async Task UpdateTourLog(TourLogViewModel submittedData)
        {
            try
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
                    TourDate = submittedData.TourLog.TourDate,
                };
                await _tourRepository.UpdateTourLogAsync(tourLogEntity);
            }
            catch(Exception) { throw; }
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
                {
                    if (logWindow.DataContext is TourLogViewModel submittedData)
                    {
                        await SaveTourLog(submittedData);
                        LogsChangedNow(submittedData.TourId);
                        MessageBox.Show($"New tour log successfully created");
                    }
                }
            }
            catch (DALException)
            {
                MessageBox.Show($"Tour log could not be created");
            }
            catch (Exception ex)
            {
                log.Error($"Unknown error creating tour log: {ex}");
            }
        }

        private async Task SaveTourLog(TourLogViewModel submittedData)
        {
            try
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
            catch (Exception) { throw; }
        }


        //////////////////////// Commands / Events ///////////////////////////////////////////


        public event EventHandler? LogsChanged;

        protected virtual void LogsChangedNow(object sender)
        {
            LogsChanged?.Invoke(sender, EventArgs.Empty);
        }


        private ICommand _changeTourCommand;
        public ICommand ChangeTourCommand
        {
            get { return _changeTourCommand; }
            set
            {
                _changeTourCommand = value;
                OnPropertyChanged(nameof(ChangeTourCommand));
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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //////////////////////// Commands / Events ///////////////////////////////////////////
    }
}
