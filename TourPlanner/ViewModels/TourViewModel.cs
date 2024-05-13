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
using TourPlanner.Models;
using TourPlanner.Persistence.Entities;
using TourPlanner.Persistence.Repository;
using TourPlanner.Persistence.Utils;

namespace TourPlanner.ViewModels
{
    public class TourViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private TourModel _tour;

        public TourModel Tour
        {
            get { return _tour; }
        }

        private bool _isExpanded;

        private readonly TourRepository _tourRepository = new TourRepository(new TourPlannerDbContext());

        public TourViewModel(TourModel tour)
        {
            _tour = tour;
            LoadLogsForExpandedTour(tour);
        }

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

        private async void LoadLogsForExpandedTour(TourModel tourModel)
        {
            var logs = await _tourRepository.GetLogsByTourIdAsync(tourModel.Id);
            _tour.Logs = new ObservableCollection<TourLogModel>(logs.Select(log => new TourLogModel(log)));
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
                                 GetErrors("ToAddress").OfType<string>()
                                     .Any(); //OnPropertyChanged(nameof(Description)); 

        private void ValidateProperty(string propertyName)
        {
            OnErrorsChanged(propertyName);
        }
    }
}