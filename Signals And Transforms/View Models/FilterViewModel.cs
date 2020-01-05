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

namespace SignalsAndTransforms.View_Models
{
    public class FilterViewModel : INotifyPropertyChanged
    {
        private WorkBookManager manager;

        public FilterViewModel()
        {
            manager = WorkBookManager.Manager();
            manager.PropertyChanged += ActiveWorkBookChangedHandler;
            LoadFilterData();
        }

        public ObservableCollection<FilterItemView> Filters { get; private set; }

        public IList<DataPoint> ImpulseResponsePoints { get; private set; }

        public IList<DataPoint> FrequencyResponsePoints { get; private set; }

        public IList<DataPoint> DecibelResponsePoints { get; private set; }

        public IList<DataPoint> StepResponsePoints { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ActiveWorkBookChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            LoadFilterData();
        }

        public void AddFilter(Filter newFilter)
        {
            WorkBook workBook = manager.ActiveWorkBook();
            workBook.Filters.Add(newFilter.Name, newFilter);
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

        private void LoadFilterData()
        {
            List<double> summedFilterData = manager.ActiveWorkBook().SummedFilterImpulseResponse(true);
            if (summedFilterData == null || summedFilterData.Count == 0)
            {
                return;
            }

            ImpulseResponsePoints = new List<DataPoint>(summedFilterData.Count);
            StepResponsePoints = new List<DataPoint>(summedFilterData.Count);

            Filters = new ObservableCollection<FilterItemView>();

            for (int idx = 0; idx < summedFilterData.Count; idx++)
            {
                ImpulseResponsePoints.Add(new DataPoint(idx, summedFilterData[idx]));
            }
            ComplexFastFourierTransform cmplxFFT = new ComplexFastFourierTransform();
            FrequencyDomain frequencyDomain = cmplxFFT.Transform(summedFilterData, manager.ActiveWorkBook().Filters.Values.First().FilterLength);

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

            foreach (var filter in manager.ActiveWorkBook().Filters.Values)
            {
                filter.PropertyChanged -= handleFilterUpdate;
                FilterItemView viewItem = new FilterItemView();
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
