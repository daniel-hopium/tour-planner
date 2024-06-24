using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TourPlanner.ViewModels;
using TourPlanner.Views.Utils;

namespace TourPlanner.Views
{
    /// <summary>
    /// Interaktionslogik für TourFormControl.xaml
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
                {
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
            }
            catch (Exception) { }
        }

    }
}
