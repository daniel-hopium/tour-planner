using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FontAwesome.WPF;

namespace TourPlanner.Views.Utils;

public class IconButton : Button
{
  public static readonly DependencyProperty IconProperty =
    DependencyProperty.Register("Icon", typeof(FontAwesomeIcon), typeof(IconButton),
      new PropertyMetadata(FontAwesomeIcon.None));

  public static readonly DependencyProperty IconColorProperty =
    DependencyProperty.Register("IconColor", typeof(Brush), typeof(IconButton),
      new PropertyMetadata(Brushes.Black));

  public FontAwesomeIcon Icon
  {
    get => (FontAwesomeIcon)GetValue(IconProperty);
    set => SetValue(IconProperty, value);
  }

  public Brush IconColor
  {
    get => (Brush)GetValue(IconColorProperty);
    set => SetValue(IconColorProperty, value);
  }
}