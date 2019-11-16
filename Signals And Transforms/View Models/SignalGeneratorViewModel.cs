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
        public IList<DataPoint> PlotPoints { get; private set; }
        public IList<DataPoint> FrequencyHistogram { get; private set; }

        public String Title { get; private set; }

        public SignalGeneratorViewModel()
        {
            Signals = new ObservableCollection<Signal>();
            Title = Properties.Resources.SIGNAL_PLOT_TITLE;
            workBookManager = WorkBookManager.Manager();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void PlotSignals()
        {
            if (Signals.Count == 0)
            {
                return;
            }

            Signal workbookSourceSignal = new Signal();
            workbookSourceSignal.Name = "Source";
            workbookSourceSignal.SamplingHZ = Signals[0].SamplingHZ;
            workbookSourceSignal.SampleSeconds = Signals[0].SampleSeconds;
            workbookSourceSignal.Type = SignalType.Output;

            PlotPoints = new List<DataPoint>();
            FrequencyHistogram = new List<DataPoint>();

            double sampleRate = Signals[0].SamplingHZ; // hz
            List<double> timePoints = new List<double>((int)(2 * sampleRate));

            for (double timePointVal = 0; timePointVal < 2.0; timePointVal += (1.0 / sampleRate))
            {
                timePoints.Add(timePointVal);
            }

            List<double> signal = new List<double>(timePoints.Count);

            foreach (double timePointVal in timePoints)
            {
                double signalValue = 0.0;

                foreach (Signal sig in Signals)
                {
                    signalValue += sig.Amplitude * (Math.Sin(2 * Math.PI * sig.SignalHZ * timePointVal));
                }
                signal.Add(signalValue);
            }

            workbookSourceSignal.Samples.AddRange(signal);
            workBookManager.ActiveWorkBook().SourceSignal = workbookSourceSignal;
            
            // Test data for now
            for (int idx=0; idx<workbookSourceSignal.Samples.Count;idx++)
            {
                PlotPoints.Add(new DataPoint(idx, workbookSourceSignal.Samples[idx]));
            }

            ComplexFastFourierTransform cmplxFFT = new ComplexFastFourierTransform();
            FrequencyDomain frequencyDomain = cmplxFFT.Transform(signal, sampleRate);

            foreach (var freq in frequencyDomain.FrequencyAmplitudes)
            {
                FrequencyHistogram.Add(new DataPoint(freq.Key, freq.Value));
            }

            NotifyPropertyChanged(nameof(PlotPoints));
            NotifyPropertyChanged(nameof(FrequencyHistogram));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
