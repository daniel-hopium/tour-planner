using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TourPlanner.Views.Utils;

public class ColorToTransparentConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is SolidColorBrush originalBrush)
    {
      if (originalBrush.Color == Colors.White) return new SolidColorBrush(Colors.LightBlue);

      return new SolidColorBrush(Color.FromArgb((byte)(originalBrush.Color.A * 0.7), originalBrush.Color.R,
        originalBrush.Color.G, originalBrush.Color.B));
    }

    return value;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotImplementedException();
  }
}