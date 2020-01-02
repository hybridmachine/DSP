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

        private void LoadFilterData()
        {
            List<double> summedFilterData = manager.ActiveWorkBook().SummedFilterImpulseResponse();
            if (summedFilterData == null || summedFilterData.Count == 0)
            {
                return;
            }

            ImpulseResponsePoints = new List<DataPoint>(summedFilterData.Count);
            Filters = new ObservableCollection<FilterItemView>();

            for (int idx = 0; idx < summedFilterData.Count; idx++)
            {
                ImpulseResponsePoints.Add(new DataPoint(idx, summedFilterData[idx]));
            }
            ComplexFastFourierTransform cmplxFFT = new ComplexFastFourierTransform();
            FrequencyDomain frequencyDomain = cmplxFFT.Transform(summedFilterData, manager.ActiveWorkBook().Filters.Values.First().FilterLength);

            FrequencyResponsePoints = new List<DataPoint>(frequencyDomain.FrequencyAmplitudes.Count);

            List<double> values = new List<double>(frequencyDomain.FrequencyAmplitudes.Values);
            for (int idx = 0; idx < frequencyDomain.FrequencyAmplitudes.Count; idx++)
            {
                FrequencyResponsePoints.Add(new DataPoint(idx, values[idx]));
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
