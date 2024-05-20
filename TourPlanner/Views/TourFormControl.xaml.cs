using System;
using System.Collections.Generic;
using System.Linq;
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
        private TourViewModel _formTourViewModel {  get; set; }

        public TourFormControl()
        {
            InitializeComponent();
            _formTourViewModel = TourViewModel.Instance;
            DataContext = _formTourViewModel;           
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                // Verwenden Sie Dispatcher.BeginInvoke, um sicherzustellen, dass die UI aktualisiert wird
                textBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    // Aktualisieren Sie die Validierungsnachricht
                    var binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    if (binding != null)
                    {
                        // Zugriff auf das Target-Element des Bindings (normalerweise das TextBox-Steuerelement)
                        var targetElement = binding.Target as FrameworkElement;

                        if (targetElement != null)
                        {
                            // Finden Sie das erste TextBlock-Element als Kind des Target-Elements
                            var textBlock = XamlHelper.FindFirstVisualChild<TextBlock>(targetElement);

                            if (textBlock != null)
                            {
                                // Aktualisieren Sie die Validierungsnachricht des gefundenen TextBlocks
                                var textBindingExpression = textBlock.GetBindingExpression(TextBlock.TextProperty);
                                textBindingExpression?.UpdateTarget();
                            }
                        }
                    }
                }));
            }
        }

    }
}
