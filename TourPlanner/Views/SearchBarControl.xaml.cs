using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TourPlanner.Views
{
    public partial class SearchBarControl : UserControl
    {
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SearchBarControl), new PropertyMetadata(string.Empty));

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public SearchBarControl()
        {
            InitializeComponent();
            //SearchTextBox.GotFocus += RemovePlaceholder;
            //SearchTextBox.LostFocus += ShowPlaceholder;
            SearchTextBox.TextChanged += SearchTextBox_TextChanged;
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
}