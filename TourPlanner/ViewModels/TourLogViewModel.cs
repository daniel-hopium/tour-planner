using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using TourPlanner.Models;
using TourPlanner.ViewModels.Utils;

namespace TourPlanner.ViewModels;

public class TourLogViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
  private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


  private readonly Dictionary<string, List<string>> _errors = new();

  private string _difficultyString = string.Empty;

  private string _distanceString = string.Empty;

  private string _ratingString = string.Empty;

  private string _totalTimeString = string.Empty;


  private string? _tourDateString;


  public TourLogViewModel(TourLogModel tourLog)
  {
    TourLog = tourLog;
  }

  public TourLogModel TourLog { get; }

  public int Id
  {
    get => TourLog.Id;
    set
    {
      TourLog.Id = value;
      OnPropertyChanged(nameof(Id));
    }
  }

  public int TourId => TourLog.TourId;

  public string TourDate
  {
    get =>
      /*if (_tourDateString != _tourLog.TourDate.ToString() && _tourDateString != string.Empty)
      {
          return _tourDateString;
      }*/
      TourLog.TourDate.ToString();
    set
    {
      _tourDateString = value;
      try
      {
        var dateOnly = Helper.ExtractAndConvertDatePart(value);

        if (dateOnly.HasValue)
        {
          TourLog.TourDate = dateOnly.Value;
          ValidateProperty(nameof(TourDate));
          OnPropertyChanged(nameof(TourDate));
        }
        else
        {
          AddError(nameof(TourDate), "Invalid Date");
        }
      }
      /*catch (UtilsException)
      {
          MessageBox.Show($"Tour Date could not be extacted");
      }*/
      catch (Exception ex)
      {
        log.Error($"Error Convert Date: {ex}");
      }
    }
  }

  public string Comment
  {
    get => TourLog.Comment;
    set
    {
      TourLog.Comment = value;
      ValidateProperty(nameof(Comment));
      OnPropertyChanged(nameof(Comment));
    }
  }

  public string Difficulty
  {
    get
    {
      if (_difficultyString != TourLog.Difficulty.ToString() && _difficultyString != string.Empty)
        return _difficultyString;
      return TourLog.Difficulty.ToString();
    }
    set
    {
      _difficultyString = value;

      if (int.TryParse(value, out var result))
      {
        TourLog.Difficulty = result;
        ValidateProperty(nameof(Difficulty));
        OnPropertyChanged(nameof(Difficulty));
      }
      else
      {
        AddError(nameof(Difficulty), "Difficulty must be a integer");
      }
    }
  }

  public string Distance
  {
    get
    {
      if (_distanceString != TourLog.Distance.ToString() && _distanceString != string.Empty)
        return _distanceString;
      return TourLog.Distance.ToString();
    }
    set
    {
      _distanceString = value;

      if (double.TryParse(value, out var result))
      {
        TourLog.Distance = result;
        ValidateProperty(nameof(Distance));
        OnPropertyChanged(nameof(Distance));
      }
      else
      {
        AddError(nameof(Distance), "Distance must be a decimal number e.g. 1.2");
      }
    }
  }

  public string TotalTime
  {
    get
    {
      if (_totalTimeString != TourLog.TotalTime.ToString() && _totalTimeString != string.Empty)
        return _totalTimeString;
      return TourLog.TotalTime.ToString();
    }
    set
    {
      _totalTimeString = value;

      if (int.TryParse(value, out var result))
      {
        TourLog.TotalTime = result;
        ValidateProperty(nameof(TotalTime));
        OnPropertyChanged(nameof(TotalTime));
      }
      else
      {
        AddError(nameof(TotalTime), "TotalTime has to be an integer");
      }
    }
  }

  public string Rating
  {
    get
    {
      if (_ratingString != TourLog.Rating.ToString() && _ratingString != string.Empty) return _ratingString;
      return TourLog.Rating.ToString();
    }
    set
    {
      _ratingString = value;

      if (int.TryParse(value, out var result))
      {
        TourLog.Rating = result;
        ValidateProperty(nameof(Rating));
        OnPropertyChanged(nameof(Rating));
      }
      else
      {
        AddError(nameof(Rating), "Rating has to be an integer");
      }
    }
  }

  public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

  public IEnumerable GetErrors(string? propertyName)
  {
    if (!string.IsNullOrEmpty(propertyName) && _errors.TryGetValue(propertyName, out var value)) yield return value;
  }

  public bool HasErrors => _errors.Count > 0;


  public event PropertyChangedEventHandler? PropertyChanged;

  protected virtual void OnPropertyChanged(string propertyName)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }

  protected virtual void OnErrorsChanged(string propertyName)
  {
    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
  }

  private void AddError(string propertyName, string error)
  {
    if (!_errors.TryGetValue(propertyName, out var value))
    {
      value = new List<string>();
      _errors[propertyName] = value;
    }

    if (!value.Contains(error))
    {
      value.Add(error);
      OnErrorsChanged(propertyName);
    }
  }

  private void RemoveError(string propertyName)
  {
    if (!_errors.ContainsKey(propertyName)) return;
    _errors.Remove(propertyName);
    OnErrorsChanged(propertyName);
  }

  private void ValidateProperty(string propertyName)
  {
    var patternDifRat = @"^[0-5]$";

    if (propertyName == "TourDate")
    {
      if (string.IsNullOrEmpty(TourDate))
        AddError(nameof(TourDate), "TourDate can not be empty");
      else if (propertyName == "TourDate" && TourLog.TourDate > DateOnly.FromDateTime(DateTime.Now))
        AddError(nameof(TourDate), "TourDate has to be in the past");
      else
        RemoveError(nameof(TourDate));
    }
    else if (propertyName == "Comment")
    {
      if (string.IsNullOrEmpty(Comment))
        AddError(nameof(Comment), "Comment can not be empty");
      else
        RemoveError(nameof(Comment));
    }
    else if (propertyName == "Difficulty")
    {
      if (string.IsNullOrEmpty(Difficulty))
        AddError(nameof(Difficulty), "Difficulty can not be empty");
      else if (!Regex.IsMatch(Difficulty, patternDifRat))
        AddError(nameof(Difficulty), "Difficulty must be a number between 0 and 5");
      else
        RemoveError(nameof(Difficulty));
    }
    else if (propertyName == "Distance")
    {
      if (string.IsNullOrEmpty(Distance))
        AddError(nameof(Distance), "Distance can not be empty");
      else
        RemoveError(nameof(Distance));
    }
    else if (propertyName == "TotalTime")
    {
      if (string.IsNullOrEmpty(TotalTime))
        AddError(nameof(TotalTime), "TotalTime can not be empty");
      else
        RemoveError(nameof(TotalTime));
    }
    else if (propertyName == "Rating")
    {
      if (string.IsNullOrEmpty(Rating))
        AddError(nameof(Rating), "Rating can not be empty");
      else if (!Regex.IsMatch(Rating, patternDifRat))
        AddError(nameof(Rating), "Rating must be a number between 0 and 5");
      else
        RemoveError(nameof(Rating));
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