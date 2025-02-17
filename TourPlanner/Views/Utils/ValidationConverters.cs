﻿using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace TourPlanner.Views.Utils;

public class ValidationErrorConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is ReadOnlyObservableCollection<ValidationError> errors && errors.Count > 0)
      return errors[0].ErrorContent;
    return string.Empty;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}