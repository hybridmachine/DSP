using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
using SignalProcessor;
using SignalProcessor.Filters;
using SignalProcessor.Interfaces;
using SignalsAndTransforms.Managers;
using SignalsAndTransforms.Models;
using SignalsAndTransforms.View_Models;
using Serilog;

namespace SignalsAndTransforms.Views
{
    /// <summary>
    /// Interaction logic for FilterView.xaml
    /// </summary>
    public partial class FilterView : UserControl
    {

        private readonly WorkBookManager manager;

        public FilterView()
        {
            InitializeComponent();
            FilterType.ItemsSource = Enum.GetValues(typeof(FILTERTYPE)).Cast<FILTERTYPE>();
            manager = WorkBookManager.Manager();
        }

        private void AddFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WorkBookManager manager = WorkBookManager.Manager();
                Models.WindowedSyncFilter newFilter = new Models.WindowedSyncFilter();
                newFilter.IsActive = true;
                newFilter.Name = FilterName.Text;
                newFilter.CutoffFrequencySamplingFrequencyPercentage = double.Parse(CutoffFrequencyPercentage.Text);
                newFilter.FilterLength = int.Parse(FilterLength.Text);
                newFilter.FilterType = (FILTERTYPE)Enum.Parse(typeof(FILTERTYPE), FilterType.SelectedItem.ToString());

                FilterViewModel model = DataContext as FilterViewModel;
                model?.AddFilter(newFilter);

                FilterName.Text = String.Empty;
                CutoffFrequencyPercentage.Text = String.Empty;
                FilterLength.Text = String.Empty;
                FilterType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, ex.Message);
                // TODO warn the user
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
                        fileWriter.WriteLine(Properties.Resources.FILTER_CSV_HEADER);
                        List<double> summedFilterData = manager.ActiveWorkBook().CombinedFilterImpulseResponse(true);
                        ComplexFastFourierTransform cmplxFFT = new ComplexFastFourierTransform();
                        FrequencyDomain frequencyDomain = cmplxFFT.Transform(summedFilterData, manager.ActiveWorkBook().WindowedSyncFilters.Values.First().FilterLength);

                        var magPhaseList = ComplexFastFourierTransform.ToMagnitudePhaseList(frequencyDomain);

                        foreach (Tuple<double, double> coefficientMagPhase in magPhaseList)
                        {
                            fileWriter.WriteLine($"{coefficientMagPhase.Item1},{coefficientMagPhase.Item2}");
                        }
                    }
                } catch (Exception ex)
                {
                    Log.Warning(ex, ex.Message);
                    // TODO warn user
                }
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = $"{Properties.Resources.FILTER_IMPORT_EXPORT_FILE_EXTENSION} (*{Properties.Resources.FILTER_IMPORT_EXPORT_FILE_EXTENSION})|*{Properties.Resources.FILTER_IMPORT_EXPORT_FILE_EXTENSION}|{Properties.Resources.ALL_FILES} (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamReader fileReader = File.OpenText(openFileDialog.FileName))
                    {
                        fileReader.ReadLine(); // Skip the header row

                        List<Tuple<double, double>> magPhaseList = new List<Tuple<double, double>>();
                        while (!fileReader.EndOfStream)
                        {
                            string[] magPhaseData = fileReader.ReadLine().Split(',');
                            double magnitude = double.Parse(magPhaseData[0]);
                            double phase = double.Parse(magPhaseData[1]);

                            magPhaseList.Add(new Tuple<double, double>(magnitude, phase));
                        }
                        WorkBookManager manager = WorkBookManager.Manager();

                        SignalsAndTransforms.Models.CustomFilter customFilter = new SignalsAndTransforms.Models.CustomFilter(magPhaseList);
                        customFilter.Name = "Test";
                        customFilter.IsActive = true;

                        FilterViewModel context = DataContext as FilterViewModel;

                        context?.AddFilter(customFilter);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, ex.Message);   
                    // TODO warn user
                }
            }
        }
    }
}
