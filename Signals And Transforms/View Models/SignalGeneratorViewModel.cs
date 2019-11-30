using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SignalProcessor;
using SignalsAndTransforms.Managers;
using SignalsAndTransforms.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.View_Models
{
    public class SignalGeneratorViewModel : INotifyPropertyChanged
    {
        private WorkBookManager workBookManager;

        public ObservableCollection<Signal> Signals { get; private set; }

        public void AddSignal(Signal signal)
        {
            workBookManager.ActiveWorkBook().Signals.Add(signal.Name, signal);
            LoadSignalsFromActiveWorkBook();
        }

        public void DeleteSignal(Signal signal)
        {
            workBookManager.ActiveWorkBook().Signals.Remove(signal.Name);
            LoadSignalsFromActiveWorkBook();
        }

        private void LoadSignalsFromActiveWorkBook()
        {
            Signals.Clear();
            List<Signal> signals = new List<Signal>(workBookManager.ActiveWorkBook().Signals.Values);
            // No addrange for observeablecollection
            foreach (Signal signal in signals)
            {
                Signals.Add(signal);
            }
        }

        public IList<DataPoint> PlotPoints { get; private set; }
        public PlotModel FrequencyViewModel { get; private set; }

        public String Title { get; private set; }

        public SignalGeneratorViewModel()
        {
            Signals = new ObservableCollection<Signal>();
            Title = Properties.Resources.SIGNAL_PLOT_TITLE;
            workBookManager = WorkBookManager.Manager();
            workBookManager.PropertyChanged += ActiveWorkBookChangedHandler;
        }

        private void ActiveWorkBookChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            LoadSignalsFromActiveWorkBook();
            PlotSignals();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void PlotSignals()
        {
            PlotPoints = new List<DataPoint>(512);
            
            Signal workbookSourceSignal = workBookManager.ActiveWorkBook().SumOfSources();
            if (workbookSourceSignal == null)
            {
                return;
            }

            // Test data for now
            for (int idx=0; idx<workbookSourceSignal.Samples.Count;idx++)
            {
                PlotPoints.Add(new DataPoint(idx, workbookSourceSignal.Samples[idx]));
            }

            ComplexFastFourierTransform cmplxFFT = new ComplexFastFourierTransform();
            FrequencyDomain frequencyDomain = cmplxFFT.Transform(workbookSourceSignal.Samples, workbookSourceSignal.SamplingHZ);

            
            FrequencyViewModel = new FrequencyHistogramViewModel(frequencyDomain);

            NotifyPropertyChanged(nameof(PlotPoints));
            NotifyPropertyChanged(nameof(FrequencyViewModel));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
