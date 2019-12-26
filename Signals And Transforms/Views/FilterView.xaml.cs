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
using SignalProcessor.Filters;

namespace SignalsAndTransforms.Views
{
    /// <summary>
    /// Interaction logic for FilterView.xaml
    /// </summary>
    public partial class FilterView : UserControl
    {
        public FilterView()
        {
            InitializeComponent();
            FilterType.ItemsSource = Enum.GetValues(typeof(FilterType)).Cast<FilterType>();
        }

        private void AddFilter_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
