using System.Windows;
using System.Windows.Controls;

namespace TourPlanner.Views;

public partial class SearchBarControl : UserControl
{
  public static readonly DependencyProperty SearchTextProperty =
    DependencyProperty.Register("SearchText", typeof(string), typeof(SearchBarControl),
      new PropertyMetadata(string.Empty));

  public SearchBarControl()
  {
    InitializeComponent();
    //SearchTextBox.GotFocus += RemovePlaceholder;
    //SearchTextBox.LostFocus += ShowPlaceholder;
    SearchTextBox.TextChanged += SearchTextBox_TextChanged;
  }

  public string SearchText
  {
    get => (string)GetValue(SearchTextProperty);
    set => SetValue(SearchTextProperty, value);
  }

  private void RemovePlaceholder(object sender, RoutedEventArgs e)
  {
    /*if (SearchTextBox.Text == "Search...")
    {
        SearchTextBox.Text = "";
        SearchTextBox.Foreground = new SolidColorBrush(Colors.Black);
    }*/
  }

  private void ShowPlaceholder(object sender, RoutedEventArgs e)
  {
    /*if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
    {
        SearchTextBox.Text = "Search...";
        SearchTextBox.Foreground = new SolidColorBrush(Colors.Gray);
    }*/
  }

  private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
  {
    SearchText = SearchTextBox.Text;
  }
}