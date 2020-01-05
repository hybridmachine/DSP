﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
using SignalProcessor.Filters;

namespace SignalsAndTransforms.Views
{
    /// <summary>
    /// Interaction logic for FilterItemView.xaml
    /// </summary>
    public partial class FilterItemView : UserControl
    {
        public FilterItemView()
        {
            InitializeComponent();

            FilterType.ItemsSource = Enum.GetValues(typeof(FilterType)).Cast<FilterType>(); 
        }
    }

    public class FilterTypeToSelectedIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {
                FilterType filterType = (FilterType)value;
                return (int)filterType;
            }
            catch (Exception ex)
            {
                // TODO log
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            try
            {
                return (FilterType)value;
            }
            catch (Exception ex)
            {
                // TODO log
                return FilterType.LOWPASS;
            }
        }
    }
}
