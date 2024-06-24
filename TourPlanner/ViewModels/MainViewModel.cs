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
using Microsoft.Win32;
using System.Globalization;
using System.Net;
using System.IO;
using System.Configuration;
using TourPlanner.Exceptions;
using log4net;
using System.Reflection;


namespace TourPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TourListControlViewModel _tourListControlViewModel;
        private readonly TourLogListControlViewModel _tourLogListControlViewModel;

        private readonly ITourRepository _tourRepository;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MainViewModel()
        {
            _tourRepository = TourRepository.Instance;            
            _tourListControlViewModel = TourListControlViewModel.Instance;
            _tourListControlViewModel.NowExpandedTour += TourList_NowExpandedTour;
            _tourLogListControlViewModel = TourLogListControlViewModel.Instance;
            _tourLogListControlViewModel.LogsChanged += LogList_LogsChanged;

            _summarizeReportCommand = new RelayCommand(SummarizeReport);
            _importTourCommand = new RelayCommand(ImportTour);
        }


        private TourViewModel? _expandedTour = null;
        public TourViewModel? ExpandedTour
        {
            get { return _expandedTour; }
            set
            {
                _expandedTour = value;
                OnPropertyChanged(nameof(ExpandedTour));
            }
        }

        private void LogList_LogsChanged(object? sender, EventArgs e)
        {
            _tourListControlViewModel.LoadToursCommand.Execute(null);
        }


        private void TourList_NowExpandedTour(object? sender, EventArgs e)
        {
            if(sender is TourViewModel tourViewModel)
            {
                ExpandedTour = tourViewModel;
            } 
            else
            {
                ExpandedTour = null;
            }

            _tourLogListControlViewModel.ChangeTourCommand.Execute(ExpandedTour);
        }


        //////////////////////// Summary Report ///////////////////////////////////////////
        private async void SummarizeReport(object obj)
        {
            try
            {
                await _tourRepository.GenerateSummarizeReportAsync();

                MessageBox.Show($"Summarize Report successfully created");
            }
            catch (DALException)
            {
                MessageBox.Show($"Error occured during calculating the Summarize Report");
            }
            catch (UtilsException)
            {
                MessageBox.Show($"Summarize Report could not be generated");
            }
        }


        //////////////////////// Import Tour ///////////////////////////////////////////
        private async void ImportTour(object obj)
        {
            try
            {
                OpenFileDialog openFileDialog = new()
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*"
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;

                    (TourPlanner.Models.TransportType transportType, double[] start, double[] end) = await _tourRepository.ImportTourAsync(filePath);

                    await MapCreator.DownloadMapFromApi(transportType, start, end);

                    _tourListControlViewModel.LoadToursCommand.Execute(null);
                    MessageBox.Show($"Tour successfully created by Import");
                }
            }
            catch (DALException)
            {
                MessageBox.Show($"Tour from Import could not be saved");
            }
            catch (UtilsException)
            {
                MessageBox.Show($"Map of Tour could not be downloaded");  // delete created tour ?
            }
            catch (Exception ex)
            {
                log.Error("Import failed", ex);
                MessageBox.Show($"Import failed");
            }
        }

   

        //////////////////////// Commands / Events ///////////////////////////////////////////

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private ICommand _summarizeReportCommand;

        public ICommand SummarizeReportCommand
        {
            get { return _summarizeReportCommand; }
            set
            {
                _summarizeReportCommand = value;
                OnPropertyChanged(nameof(SummarizeReportCommand));
            }
        }

        private ICommand _importTourCommand;
        public ICommand ImportTourCommand
        {
            get { return _importTourCommand; }
            set
            {
                _importTourCommand = value;
                OnPropertyChanged(nameof(ImportTourCommand));
            }
        }

        //////////////////////// Commands / Events ///////////////////////////////////////////
    }
}