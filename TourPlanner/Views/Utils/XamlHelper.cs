using System.Windows;
using System.Windows.Media;

namespace TourPlanner.Views.Utils;

public static class XamlHelper
{
  // search recursive for the parent-XAML-element of type T
  public static T? FindVisualParent<T>(DependencyObject child) where T : DependencyObject
  {
    var parentObject = VisualTreeHelper.GetParent(child);

    // check for right type or if already got to end
    if (parentObject == null)
      return null;

    var parent = parentObject as T;
    if (parent != null)
      return parent;
    return FindVisualParent<T>(parentObject);
  }

  public static T? FindFirstVisualChild<T>(DependencyObject dependencyObject) where T : DependencyObject
  {
    if (dependencyObject != null)
      for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
      {
        var child = VisualTreeHelper.GetChild(dependencyObject, i);
        if (child != null && child is T) return (T)child;

        var childOfChild = FindFirstVisualChild<T>(child);
        if (childOfChild != null) return childOfChild;
      }

    return null;
  }
}