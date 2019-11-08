using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SignalProcessor;
using Signals_And_Transforms.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals_And_Transforms.View_Models
{
    public class SignalGeneratorViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Signal> Signals { get; private set; }
        public IList<DataPoint> PlotPoints { get; private set; }
        public IList<DataPoint> FrequencyHistogram { get; private set; }

        public String Title { get; private set; }

        public SignalGeneratorViewModel()
        {
            Signals = new ObservableCollection<Signal>();
            Title = Properties.Resources.SIGNAL_PLOT_TITLE;       
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void PlotSignals()
        {
            if (Signals.Count == 0)
            {
                return;
            }

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

            // Test data for now
            for (int idx=0; idx<signal.Count;idx++)
            {
                PlotPoints.Add(new DataPoint(idx, signal[idx]));
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
            PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, args);
        }
    }
}
