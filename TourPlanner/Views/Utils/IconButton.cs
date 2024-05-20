using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace TourPlanner.Views.Utils
{
    public class IconButton : Button
    {
        public static readonly DependencyProperty IconProperty =
           DependencyProperty.Register("Icon", typeof(FontAwesomeIcon), typeof(IconButton), new PropertyMetadata(FontAwesomeIcon.None));

        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.Register("IconColor", typeof(Brush), typeof(IconButton), new PropertyMetadata(Brushes.Black));

        public FontAwesomeIcon Icon
        {
            get { return (FontAwesomeIcon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public Brush IconColor
        {
            get { return (Brush)GetValue(IconColorProperty); }
            set { SetValue(IconColorProperty, value); }
        }
    }
}
