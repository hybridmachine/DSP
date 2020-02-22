using DSP;
using OxyPlot;
using SignalProcessor;
using SignalsAndTransforms.Managers;
using SignalsAndTransforms.Models;
using SignalsAndTransforms.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SignalsAndTransforms.View_Models
{
    public class FilterViewModel : INotifyPropertyChanged
    {
        private WorkBookManager manager;

        private const string FILTERCOMBINEMODE = "FilterCombineMode";

        public FilterViewModel()
        {
            manager = WorkBookManager.Manager();
            manager.PropertyChanged += ActiveWorkBookChangedHandler;
            SumModeActive = true;
            ConvolveModeActive = !SumModeActive;

            LoadFilterData();
        }

        public ObservableCollection<UserControl> Filters { get; private set; }

        public IList<DataPoint> ImpulseResponsePoints { get; private set; }

        public IList<DataPoint> FrequencyResponsePoints { get; private set; }

        public IList<DataPoint> DecibelResponsePoints { get; private set; }

        public IList<DataPoint> StepResponsePoints { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ActiveWorkBookChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            LoadFilterData();
        }

        public void AddFilter(WindowedSyncFilter newFilter)
        {
            WorkBook workBook = manager.ActiveWorkBook();
            workBook.WindowedSyncFilters.Add(newFilter.Name, newFilter);
            LoadFilterData(); // Property changed event handlers will be connected in this function
        }

        public void AddFilter(CustomFilter newFilter)
        {
            WorkBook workBook = manager.ActiveWorkBook();
            workBook.CustomFilters.Add(newFilter.Name, newFilter);
            LoadFilterData(); // Property changed event handlers will be connected in this function
        }

        private List<double> GetStepData(int len)
        {
            List<double> stepData = new List<double>();
            int pointsOn = len / 2; // point at which signal switches from 0 to 1

            for (int idx = 0; idx < len; idx++)
            {
                stepData.Add(idx >= (pointsOn - 1) ? 1.0 : 0.0);
            }
            return stepData;
        }

        /// <summary>
        /// Sum the filters
        /// </summary>
        public bool SumModeActive { 
            get
            {
                Dictionary<string, string> settings = WorkBookManager.Manager().ActiveWorkBook().Settings;
                if (settings.ContainsKey(FILTERCOMBINEMODE))
                {
                    if (settings[FILTERCOMBINEMODE].Trim().ToUpperInvariant() == nameof(SumModeActive).ToUpperInvariant())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            set
            {
                Dictionary<string, string> settings = WorkBookManager.Manager().ActiveWorkBook().Settings;
                if (value == true)
                {
                    settings[FILTERCOMBINEMODE] = nameof(SumModeActive).ToUpperInvariant();
                }
                // If false, we assume that the radio button true value for ConvolveModeActive will set the setting
            }
        }

        /// <summary>
        /// Convolve the filters
        /// </summary>
        public bool ConvolveModeActive {
            get
            {
                Dictionary<string, string> settings = WorkBookManager.Manager().ActiveWorkBook().Settings;
                if (settings.ContainsKey(FILTERCOMBINEMODE))
                {
                    if (settings[FILTERCOMBINEMODE].Trim().ToUpperInvariant() == nameof(ConvolveModeActive).ToUpperInvariant())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            set
            {
                Dictionary<string, string> settings = WorkBookManager.Manager().ActiveWorkBook().Settings;
                if (value == true)
                {
                    settings[FILTERCOMBINEMODE] = nameof(ConvolveModeActive).ToUpperInvariant();
                }
                // If false, we assume that the radio button true value for SumModeActive will set the setting
            }
        }

        private void LoadFilterData()
        {
            List<double> summedFilterData = manager.ActiveWorkBook().SummedFilterImpulseResponse(true);

            // Return an empty set
            if (summedFilterData == null || summedFilterData.Count == 0)
            {
                ImpulseResponsePoints = new List<DataPoint>();
                StepResponsePoints = new List<DataPoint>();
                FrequencyResponsePoints = new List<DataPoint>();
                DecibelResponsePoints = new List<DataPoint>();

                NotifyPropertyChanged(nameof(ImpulseResponsePoints));
                NotifyPropertyChanged(nameof(FrequencyResponsePoints));
                NotifyPropertyChanged(nameof(DecibelResponsePoints));
                NotifyPropertyChanged(nameof(StepResponsePoints));
                NotifyPropertyChanged(nameof(SumModeActive));
                NotifyPropertyChanged(nameof(ConvolveModeActive));

                return;
            }

            ImpulseResponsePoints = new List<DataPoint>(summedFilterData.Count);
            StepResponsePoints = new List<DataPoint>(summedFilterData.Count);

            Filters = new ObservableCollection<UserControl>();

            for (int idx = 0; idx < summedFilterData.Count; idx++)
            {
                ImpulseResponsePoints.Add(new DataPoint(idx, summedFilterData[idx]));
            }
            ComplexFastFourierTransform cmplxFFT = new ComplexFastFourierTransform();
            int filterLength = 0;

            if (manager.ActiveWorkBook().WindowedSyncFilters.Count > 0)
            {
                filterLength = manager.ActiveWorkBook().WindowedSyncFilters.Values.First().FilterLength;
            }
            else if (manager.ActiveWorkBook().CustomFilters.Count > 0)
            {
                filterLength = manager.ActiveWorkBook().CustomFilters.Values.First().FilterLength;
            } 
            else
            {
                return; // Count not find any filters to set length with
            }

            FrequencyDomain frequencyDomain = cmplxFFT.Transform(summedFilterData, filterLength);

            Convolution convolver = new Convolution();
            List<double> stepResponse = convolver.Convolve(summedFilterData, GetStepData(summedFilterData.Count + 16), ConvolutionType.INPUTSIDE);
            FrequencyResponsePoints = new List<DataPoint>(frequencyDomain.FrequencyAmplitudes.Count);
            DecibelResponsePoints = new List<DataPoint>(FrequencyResponsePoints.Count);

            // Load the frequency response graph data
            // Only scan the first half of the coefficients (up to the Nyquist frequency)
            int coefficientMax = (frequencyDomain.FourierCoefficients.Count / 2);
            for (int idx = 0; idx < coefficientMax; idx++)
            {
                double coeffLen = Complex.Abs(frequencyDomain.FourierCoefficients[idx]);
                double cuttoffFrequencyPercent = (((double)idx + 1.0) / summedFilterData.Count);
                FrequencyResponsePoints.Add(new DataPoint(cuttoffFrequencyPercent, coeffLen));
                DecibelResponsePoints.Add(new DataPoint(cuttoffFrequencyPercent, 20 * Math.Log10(coeffLen)));
            }

            int startingOffset = (summedFilterData.Count / 2);
            int endingOffset = startingOffset + summedFilterData.Count;
            int graphX = 0;
            // Load the step response graph data
            for (int idx = (startingOffset - 1); idx < endingOffset; idx++)
            {
                StepResponsePoints.Add(new DataPoint(graphX, stepResponse[idx]));
                graphX++;
            }

            foreach (var filter in manager.ActiveWorkBook().WindowedSyncFilters.Values)
            {
                filter.PropertyChanged -= handleFilterUpdate;
                WindowedSyncFilterItemView viewItem = new WindowedSyncFilterItemView();
                viewItem.DataContext = filter;
                Filters.Add(viewItem);
                filter.PropertyChanged += handleFilterUpdate; // Remove/re-add to avoid leaks in event handlers
            }

            foreach (var filter in manager.ActiveWorkBook().CustomFilters.Values)
            {
                filter.PropertyChanged -= handleFilterUpdate;
                CustomFilterViewItem viewItem = new CustomFilterViewItem();
                viewItem.DataContext = filter;
                Filters.Add(viewItem);
                filter.PropertyChanged += handleFilterUpdate; // Remove/re-add to avoid leaks in event handlers
            }

            NotifyPropertyChanged(nameof(ImpulseResponsePoints));
            NotifyPropertyChanged(nameof(FrequencyResponsePoints));
            NotifyPropertyChanged(nameof(DecibelResponsePoints));
            NotifyPropertyChanged(nameof(StepResponsePoints));
            NotifyPropertyChanged(nameof(Filters));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void handleFilterUpdate(object sender, PropertyChangedEventArgs args)
        {
            LoadFilterData();
        }
    }
}
