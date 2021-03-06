﻿using DSP;
using OxyPlot;
using SignalProcessor;
using SignalProcessor.Interfaces;
using SignalsAndTransforms.Interfaces;
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
using System.Windows.Input;

namespace SignalsAndTransforms.View_Models
{
    public class FilterViewModel : INotifyPropertyChanged
    {
        private WorkBookManager manager;

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
                return WorkBookManager.Manager().ActiveWorkBook().SumModeActive;
            }
            set
            {
                WorkBookManager.Manager().ActiveWorkBook().SumModeActive = value;
                LoadFilterData();
            }
        }

        /// <summary>
        /// Convolve the filters
        /// </summary>
        public bool ConvolveModeActive {
            get
            {
                return WorkBookManager.Manager().ActiveWorkBook().ConvolveModeActive;
            }
            set
            {
                WorkBookManager.Manager().ActiveWorkBook().ConvolveModeActive = value;
                LoadFilterData();
            }
        }

        /// <summary>
        /// Delete the specified filters from the workbook then reload filter data (refreshing the display)
        /// </summary>
        /// <param name="deleteItems">List of filters to delete</param>
        public void DeleteFilters(List<IFilterIdentifier> deleteItems)
        {
            foreach (IFilterIdentifier deleteMe in deleteItems)
            {
                WorkBookManager.Manager().ActiveWorkBook().DeleteFilter(deleteMe);
            }

            LoadFilterData();
        }

        private void LoadFilterData()
        {
            List<double> summedFilterData = manager.ActiveWorkBook().CombinedFilterImpulseResponse(true);

            // Return an empty set
            if (summedFilterData == null || summedFilterData.Count == 0)
            {
                ImpulseResponsePoints = new List<DataPoint>();
                StepResponsePoints = new List<DataPoint>();
                FrequencyResponsePoints = new List<DataPoint>();
                DecibelResponsePoints = new List<DataPoint>();
                Filters = new ObservableCollection<UserControl>();

                NotifyPropertyChanged(nameof(ImpulseResponsePoints));
                NotifyPropertyChanged(nameof(FrequencyResponsePoints));
                NotifyPropertyChanged(nameof(DecibelResponsePoints));
                NotifyPropertyChanged(nameof(StepResponsePoints));
                NotifyPropertyChanged(nameof(SumModeActive));
                NotifyPropertyChanged(nameof(ConvolveModeActive));
                NotifyPropertyChanged(nameof(Filters));

                return;
            }

            ImpulseResponsePoints = new List<DataPoint>(summedFilterData.Count);
            StepResponsePoints = new List<DataPoint>(summedFilterData.Count);

            Filters = new ObservableCollection<UserControl>();

            for (int idx = 0; idx < summedFilterData.Count; idx++)
            {
                ImpulseResponsePoints.Add(new DataPoint(idx, summedFilterData[idx]));
            }
            //IDFT cmplxFFT = new ComplexFastFourierTransform();
            IDFT cmplxFFT = new DSPGuideComplexDiscreteFourierTransform();

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
