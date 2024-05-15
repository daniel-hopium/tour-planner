using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TourPlanner.Models;
using TourPlanner.Persistence.Repository;
using TourPlanner.Persistence.Utils;
using TourPlanner.ViewModels.Utils;


namespace TourPlanner.ViewModels
{
    public class TourLogViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private TourLogModel _tourLog;

        public TourLogModel TourLog
        {
            get { return _tourLog; }
        }

        private readonly TourRepository _tourRepository = new TourRepository(new TourPlannerDbContext());

        private Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public TourLogViewModel(TourLogModel tourLog)
        {
            _tourLog = tourLog;
        }

        

        public int Id
        {
            get { return _tourLog.Id; }
            set
            {
                _tourLog.Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        private string _tourDateString;
        public string TourDate
        {
            get
            {
                /*if (_tourDateString != _tourLog.TourDate.ToString() && _tourDateString != string.Empty)
                {
                    return _tourDateString;
                }*/
                return _tourLog.TourDate.ToString();
            }
            set
            {
                _tourDateString = value;
                DateOnly? dateOnly = Helper.ExtractAndConvertDatePart(value);
                
                if (dateOnly.HasValue)
                {
                    _tourLog.TourDate = (dateOnly.Value);
                    ValidateProperty(nameof(TourDate));
                    OnPropertyChanged(nameof(TourDate));
                }
                else { AddError(nameof(TourDate), "Invalid Date"); }
            }
        }

        public string Comment
        {
            get { return _tourLog.Comment; }
            set
            {
                _tourLog.Comment = value;
                ValidateProperty(nameof(Comment));
                OnPropertyChanged(nameof(Comment));
            }
        }

        private string _difficultyString = string.Empty;
        public string Difficulty
        {
            get
            {
                if (_difficultyString != _tourLog.Difficulty.ToString() && _difficultyString != string.Empty)
                {
                    return _difficultyString;
                }
                return _tourLog.Difficulty.ToString();
            }
            set
            {
                _difficultyString = value;

                if (int.TryParse(value, out int result))
                {
                    _tourLog.Difficulty = result;
                    ValidateProperty(nameof(Difficulty));
                    OnPropertyChanged(nameof(Difficulty));
                }
                else { AddError(nameof(Difficulty), "Difficulty must be a integer"); }
            }
        }

        private string _distanceString = string.Empty;
        public string? Distance
        {
            get
            {
                if (_distanceString != _tourLog.Distance.ToString() && _distanceString != string.Empty)
                {
                    return _distanceString;
                }
                return _tourLog.Distance.ToString();
            }
            set
            {
                _distanceString = value;

                if (double.TryParse(value, out double result))
                {
                    _tourLog.Distance = result;
                    ValidateProperty(nameof(Distance));
                    OnPropertyChanged(nameof(Distance));
                }
                else { AddError(nameof(Distance), "Distance must be a decimal number e.g. 1.2"); }
            }
        }

        private string _totalTimeString = string.Empty;
        public string TotalTime
        {
            get
            {
                if (_totalTimeString != _tourLog.TotalTime.ToString() && _totalTimeString != string.Empty)
                {
                    return _totalTimeString;
                }
                return _tourLog.TotalTime.ToString();
            }
            set
            {
                _totalTimeString = value;

                if (int.TryParse(value, out int result))
                {
                    _tourLog.TotalTime = result;
                    ValidateProperty(nameof(TotalTime));
                    OnPropertyChanged(nameof(TotalTime));
                }
                else
                {
                    AddError(nameof(TotalTime), "TotalTime has to be an integer");
                }
            }
        }

        private string _ratingString = string.Empty;
        public string Rating
        {
            get
            {
                if (_ratingString != _tourLog.Rating.ToString() && _ratingString != string.Empty)
                {
                    return _ratingString;
                }
                return _tourLog.Rating.ToString();
            }
            set
            {
                _ratingString = value;

                if (int.TryParse(value, out int result))
                {
                    _tourLog.Rating = result;
                    ValidateProperty(nameof(Rating));
                    OnPropertyChanged(nameof(Rating));
                }
                else
                {
                    AddError(nameof(Rating), "Rating has to be an integer");
                }
            }
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
            if (!string.IsNullOrEmpty(propertyName) && _errors.ContainsKey(propertyName))
            {
                yield return _errors[propertyName];
            }
            yield break;
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
            {
                _errors[propertyName] = new List<string>();
            }
            if (!_errors[propertyName].Contains(error))
            {
                _errors[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        private void RemoveError(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        public bool HasErrors => _errors.Count > 0;

        private void ValidateProperty(string propertyName)
        {
            string patternDifRat = @"^[0-5]$";

            if (propertyName == "TourDate")
            {
                if (string.IsNullOrEmpty(TourDate.ToString()))
                {
                    AddError(nameof(TourDate), "TourDate can not be empty");
                }
                else if (propertyName == "TourDate" && _tourLog.TourDate > DateOnly.FromDateTime(DateTime.Now))
                {
                    AddError(nameof(TourDate), "TourDate has to be in the past");
                }
                else
                {
                    RemoveError(nameof(TourDate));
                }
            }
            else if (propertyName == "Comment")
            {
                if (string.IsNullOrEmpty(Comment))
                {
                    AddError(nameof(Comment), "Comment can not be empty");
                }
                else
                {
                    RemoveError(nameof(Comment));
                }
            }
            else if (propertyName == "Difficulty")
            {
                if (string.IsNullOrEmpty(Difficulty))
                {
                    AddError(nameof(Difficulty), "Difficulty can not be empty");
                }
                else if (!Regex.IsMatch(Difficulty, patternDifRat))
                {
                    AddError(nameof(Difficulty), "Difficulty must be a number between 0 and 5");
                }
                else
                {
                    RemoveError(nameof(Difficulty));
                }
            }
            else if (propertyName == "Distance")
            {
                RemoveError(nameof(Distance));
            }
            else if (propertyName == "TotalTime")
            {
                if (string.IsNullOrEmpty(TotalTime))
                {
                    AddError(nameof(TotalTime), "TotalTime can not be empty");
                }
                else
                {
                    RemoveError(nameof(TotalTime));
                }

            }
            else if (propertyName == "Rating")
            {
                if (string.IsNullOrEmpty(Rating))
                {
                    AddError(nameof(Rating), "Rating can not be empty");
                }
                else if (!Regex.IsMatch(Rating, patternDifRat))
                {
                    AddError(nameof(Rating), "Rating must be a number between 0 and 5");
                }
                else
                {
                    RemoveError(nameof(Rating));
                }
            }
        }
        
        public bool AreFieldsEmpty()
        {
            return string.IsNullOrWhiteSpace(TourDate) ||
                   string.IsNullOrWhiteSpace(Difficulty) ||
                   string.IsNullOrWhiteSpace(Distance) ||
                   string.IsNullOrWhiteSpace(Comment) ||
                   string.IsNullOrWhiteSpace(TotalTime);
        }
    }
}
