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

        private readonly TourRepository _tourRepository;

        public MainViewModel(TourRepository tourRepository)
        {
            _tourRepository = tourRepository;
            LoadTours();

            ExpandedCommand = new RelayCommand(ExpandTour);
            TourCreateCommand = new RelayCommand(CreateTour);
            TourEditCommand = new RelayCommand(EditTour);
            TourReportCommand = new RelayCommand(ReportTour);
            ExportCommand = new RelayCommand(ExportTour);
            TourDeleteCommand = new RelayCommand(DeleteTour);

            SaveCommand = new RelayCommand(SaveTour);
        }

        private void LoadTours()
        {
            var tourEntities = _tourRepository.GetTours();
            var tourModels = tourEntities.Select(entity => new TourModel(entity));
            Tours = new ObservableCollection<TourViewModel>(
                tourModels.Select(model => new TourViewModel(model)));
        }


        public TourEntity TourModelToEntity(TourModel tour)
        {
            var fromAddress = tour.FromAddress;
            string[] teile = fromAddress.Split(',');
            teile[0] = teile[0].Trim();
            teile[1] = teile[1].Trim();
            string[] strasseteile = teile[0].Split(" ");
            string fromHousenumber = strasseteile.Last();
            Array.Resize(ref strasseteile, strasseteile.Length - 1);
            string fromStreet = string.Join(" ", strasseteile);
            string[] cityteile = teile[1].Split(" ");
            int fromZip; int.TryParse(cityteile[0], out fromZip);
            string fromCity = cityteile[1];

            var fromAddressEntity = _tourRepository.GetAddressByAttributes(fromStreet, fromHousenumber, fromZip, fromCity);
            if (fromAddressEntity == null) {
                int fromAddressId = _tourRepository.AddAddress(new AddressEntity { Id = 0, Street = fromStreet, Housenumber = fromHousenumber, Zip = fromZip, City = fromCity }); ; 
                fromAddressEntity = _tourRepository.GetAddressById(fromAddressId);
            } 

            var toAddress = tour.ToAddress;
            teile = toAddress.Split(',');
            teile[0] = teile[0].Trim();
            teile[1] = teile[1].Trim();
            strasseteile = teile[0].Split(" ");
            string toHousenumber = strasseteile.Last();
            Array.Resize(ref strasseteile, strasseteile.Length - 1);
            string toStreet = string.Join(" ", strasseteile);
            cityteile = teile[1].Split(" ");
            int toZip; int.TryParse(cityteile[0], out toZip);
            string toCity = cityteile[1];

            var toAddressEntity = _tourRepository.GetAddressByAttributes(toStreet, toHousenumber, toZip, toCity);
            if (toAddressEntity == null) {
                int toAddressId = _tourRepository.AddAddress(new AddressEntity { Street = toStreet, Housenumber = toHousenumber, Zip = toZip, City = toCity });
                toAddressEntity = _tourRepository.GetAddressById(toAddressId);
            }

            var tourEntity = new TourEntity {
                Id = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                FromAddressId = fromAddressEntity.Id,
                FromAddress = fromAddressEntity,
                ToAddressId = toAddressEntity.Id,
                ToAddress = toAddressEntity,
                TransportType = tour.TransportType.ToString(),
                Distance = tour.Distance,
                EstimatedTime = tour.EstimatedTime,
                Image = tour.Image,
                Popularity = tour.Popularity,
                ChildFriendliness = tour.ChildFriendliness
            };

            return tourEntity;
        }


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
        }}


        private TourViewModel? _expandedTour = null;
        public TourViewModel ExpandedTour
        {
            get { return _expandedTour; }
            set {
                if (_expandedTour != value) {
                    if (_expandedTour != null) {
                        _expandedTour.PropertyChanged -= TourViewModel_PropertyChanged; // Unsubscribe from the previously expanded tour
                    }
                    _expandedTour = value;
                    OnPropertyChanged(nameof(ExpandedTour));
                    if (_expandedTour != null) {
                        _expandedTour.PropertyChanged += TourViewModel_PropertyChanged; // Subscribe to the newly expanded tour
        }}}}

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
            if (parameter is TreeViewItem treeViewItem) {
                var tourViewModel = treeViewItem.DataContext as TourViewModel;
                
                if (tourViewModel != null) {
                    await _tourRepository.DeleteTourByIdAsync(tourViewModel.Id);
                     LoadTours();
                     MessageBox.Show($"Tour successfully deleted");                   
        }}}

        private async void SaveTour(object parameter)
        {
            // if error don't save
            if (FormTour.HasErrors) {
                MessageBox.Show($"Tour could not be saved, first handle the errors");
                return;
            }

            // Update
            if (FormTour.Tour.IsNew == null) {
                await _tourRepository.UpdateTourAsync(TourModelToEntity(FormTour.Tour));
                FormTour = null;
                LoadTours();
                MessageBox.Show($"Changes to the tour have been successfully applied");
                OnUpdateCompleted(EventArgs.Empty);
            } 
            // Create
            else {
                await _tourRepository.CreateTourAsync(TourModelToEntity(FormTour.Tour));
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
        }}}


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
