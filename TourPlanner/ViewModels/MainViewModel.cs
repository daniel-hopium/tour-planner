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


namespace TourPlanner.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TourViewModel> _tours;
        public ObservableCollection<TourViewModel> Tours
        {
            get { return _tours; }
            set
            {
                _tours = value;
                OnPropertyChanged(nameof(Tours));
            }
        }

        private readonly TourRepository _tourRepository;

        public MainViewModel(TourRepository tourRepository)
        {
            _tourRepository = tourRepository;
            LoadTours();

            ExpandedCommand = new RelayCommand(ExpandTour);
        }

        private TourViewModel? _expandedTour = null;
        public TourViewModel ExpandedTour
        {
            get { return _expandedTour; }
            set
            {
                if (_expandedTour != value)
                {
                    if (_expandedTour != null)
                    {
                        _expandedTour.PropertyChanged -= TourViewModel_PropertyChanged; // Unsubscribe from the previously expanded tour
                    }
                    _expandedTour = value;
                    OnPropertyChanged(nameof(ExpandedTour));
                    if (_expandedTour != null)
                    {
                        _expandedTour.PropertyChanged += TourViewModel_PropertyChanged; // Subscribe to the newly expanded tour
                    }
                }
            }
        }
        private void TourViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsExpanded")
            {
                var tourViewModel = sender as TourViewModel;
                if (tourViewModel != null && !tourViewModel.IsExpanded)
                {
                    // If the IsExpanded property of a TourViewModel becomes false, set ExpandedTour to null
                    ExpandedTour = null;
                }
            }
        }


        private ICommand _expandedCommand;
        public ICommand ExpandedCommand
        {
            get { return _expandedCommand; }
            set
            {
                _expandedCommand = value;
                OnPropertyChanged(nameof(ExpandedCommand));
            }
        }

        private void LoadTours()
        {
            var tourEntities = _tourRepository.GetTours();
            var tourModels = tourEntities.Select(entity => new TourModel(entity));
            Tours = new ObservableCollection<TourViewModel>(
                tourModels.Select(model => new TourViewModel(model)));
        }

        private void ExpandTour(object parameter)
        {
            if (parameter is TreeViewItem treeViewItem)
            {
                var tourViewModel = treeViewItem.DataContext as TourViewModel;
                if (tourViewModel != null)
                {
                    ExpandedTour = tourViewModel;

                    foreach (var item in Tours)
                    {
                        if (item != tourViewModel)
                        {
                            if (item != null)
                            {
                                item.IsExpanded = false;
                            }
        }}}}}


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
