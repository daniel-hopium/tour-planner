using System.Windows;
using System.Windows.Controls;
using FontAwesome.WPF;
using TourPlanner.ViewModels;
using TourPlanner.Views.Utils;

namespace TourPlanner.Views;

/// <summary>
///   Interaktionslogik für TourListControl.xaml
/// </summary>
public partial class TourListControl : UserControl
{
  private readonly TourListControlViewModel _tourListViewModel;

  public TourListControl()
  {
    InitializeComponent();
    _tourListViewModel = TourListControlViewModel.Instance;
    DataContext = _tourListViewModel;
  }


  // set isExpanded of other than the one now expanded item to false -> collapse so just one tour at same time expanded
  private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
  {
    if (sender is TreeViewItem treeViewItem) _tourListViewModel.ExpandedCommand.Execute(treeViewItem);
  }


  private void Tour_MouseLeftButtonDown(object sender, RoutedEventArgs e)
  {
    // sender is an clicked ImageAwesome-element
    var clickedImage = sender as ImageAwesome;
    // find parent TreeViewItem
    var parentTreeViewItem = XamlHelper.FindVisualParent<TreeViewItem>(clickedImage);

    if (parentTreeViewItem != null)
    {
      if (clickedImage.Name == "TourEdit") _tourListViewModel.TourEditCommand.Execute(parentTreeViewItem);
      if (clickedImage.Name == "TourReport") _tourListViewModel.TourReportCommand.Execute(parentTreeViewItem);
      if (clickedImage.Name == "TourExport") _tourListViewModel.ExportCommand.Execute(parentTreeViewItem);
      if (clickedImage.Name == "TourDelete") _tourListViewModel.TourDeleteCommand.Execute(parentTreeViewItem);
    }
  }
}