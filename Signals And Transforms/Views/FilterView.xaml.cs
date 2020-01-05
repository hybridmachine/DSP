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
using SignalsAndTransforms.Managers;
using SignalsAndTransforms.Models;
using SignalsAndTransforms.View_Models;

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
            try
            {
                WorkBookManager manager = WorkBookManager.Manager();
                Filter newFilter = new Filter();
                newFilter.IsActive = true;
                newFilter.Name = FilterName.Text;
                newFilter.CutoffFrequencySamplingFrequencyPercentage = double.Parse(CutoffFrequencyPercentage.Text);
                newFilter.FilterLength = int.Parse(FilterLength.Text);
                newFilter.FilterType = (FilterType)Enum.Parse(typeof(FilterType), FilterType.SelectedItem.ToString());

                FilterViewModel model = DataContext as FilterViewModel;
                model?.AddFilter(newFilter);

                FilterName.Text = String.Empty;
                CutoffFrequencyPercentage.Text = String.Empty;
                FilterLength.Text = String.Empty;
                FilterType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // TODO log it and warn the user
            }
        }
    }
}
