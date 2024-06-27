using System;
using System.Windows;
using System.Windows.Controls;
using TourPlanner.ViewModels;
using TourPlanner.Views.Utils;

namespace TourPlanner.Views;

/// <summary>
///   Interaktionslogik für TourFormControl.xaml
/// </summary>
public partial class TourFormControl : UserControl
{
  private readonly TourViewModel _formTourViewModel; // {  get; set; }

  public TourFormControl()
  {
    InitializeComponent();
    _formTourViewModel = TourViewModel.Instance;
    DataContext = _formTourViewModel;
  }

  private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
  {
    try
    {
      var textBox = sender as TextBox;
      if (textBox != null)
        // Dispatcher.BeginInvoke to automaticly reload UI
        textBox.Dispatcher.BeginInvoke(new Action(() =>
        {
          // refresh validation
          var binding = textBox.GetBindingExpression(TextBox.TextProperty);
          if (binding != null)
          {
            // TextBox/Target-element
            var targetElement = binding.Target as FrameworkElement;

            if (targetElement != null)
            {
              var textBlock = XamlHelper.FindFirstVisualChild<TextBlock>(targetElement);

              if (textBlock != null)
              {
                // refresh validation of TextBlocks
                var textBindingExpression = textBlock.GetBindingExpression(TextBlock.TextProperty);
                textBindingExpression?.UpdateTarget();
              }
            }
          }
        }));
    }
    catch (Exception)
    {
    }
  }
}