using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using OxyPlot.Wpf;
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

        private void DecibelPlotMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_ClickHandler(DecibelPlotLineSeries, sender, e);
        }

        private void StepPlotMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_ClickHandler(StepPlotLineSeries, sender, e);
        }

        private void FrequencyPlotMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_ClickHandler(FrequencyPlotLineSeries, sender, e);
        }

        private void ImpulsePlotMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_ClickHandler(ImpulsePlotLineSeries, sender, e);
        }

        private void MenuItem_ClickHandler(LineSeries lineSeries, object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;

            if (menuItem != null)
            {
                if (menuItem.Header.ToString() == Properties.Resources.CTXT_PLOT_MARKER_TYPE_NONE)
                {
                    lineSeries.MarkerType = OxyPlot.MarkerType.None;
                }
                else if (menuItem.Header.ToString() == Properties.Resources.CTXT_PLOT_MARKER_TYPE_SQUARE)
                {
                    lineSeries.MarkerType = OxyPlot.MarkerType.Square;
                }
                else if (menuItem.Header.ToString() == Properties.Resources.CTXT_PLOT_MARKER_TYPE_TRIANGLE)
                {
                    lineSeries.MarkerType = OxyPlot.MarkerType.Triangle;
                }

            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = $"{Properties.Resources.FILTER_IMPORT_EXPORT_FILE_EXTENSION} (*{Properties.Resources.FILTER_IMPORT_EXPORT_FILE_EXTENSION})|*{Properties.Resources.FILTER_IMPORT_EXPORT_FILE_EXTENSION}|{Properties.Resources.ALL_FILES} (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter fileWriter = File.CreateText(saveFileDialog.FileName))
                    {
                        
                    }
                } catch (Exception ex)
                {
                    // TODO log and warn user
                }
            }
        }
    }
}
