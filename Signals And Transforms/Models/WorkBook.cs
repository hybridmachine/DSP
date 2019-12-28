﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using SignalProcessor.Filters;

namespace SignalsAndTransforms.Models
{
    // A collection of signals, filters, and convolution kernels being actively worked on. Also contains the code
    // to save/load from disk. WorkBooks are managed by the WorkBookManager
    public class WorkBook : INotifyPropertyChanged
    {
        private Dictionary<string, Signal> m_signals;
        private Dictionary<string, WindowedSyncFilter> m_filters;

        public WorkBook()
        {
            // Default constructor used by Dapper, which loads the name property by mapping.
            m_signals = new Dictionary<string, Signal>();
            m_filters = new Dictionary<string, WindowedSyncFilter>();
        }

        public WorkBook(String name)
        {
            Name = name;
            m_signals = new Dictionary<string, Signal>();
            m_filters = new Dictionary<string, WindowedSyncFilter>();
        }
        public long Id { get; set; }
        public String Name { get; set; }
        public String FilePath { get; set; }
        public String Notes { get; set; }

        public Dictionary<string, Signal> Signals
        {
            get { return m_signals; }
        }

        public Dictionary<string, WindowedSyncFilter> Filters
        {
            get { return m_filters; }
        }

        public Signal ConvolutionKernel {
            get
            {
                Signal signal = null;
                if (m_signals.ContainsKey("ConvolutionKernel"))
                {
                    signal = m_signals["ConvolutionKernel"];
                }
                return signal;
            }

            set
            {
                m_signals["ConvolutionKernel"] = value;
            }
        }

        public Signal SumOfSources()
        {
            List<Signal> signals = new List<Signal>(Signals.Values.Where(sig => sig.IsActive));
            Signal baseSignal = signals.Where(sig => sig.Type == SignalType.Source).FirstOrDefault();
            if (baseSignal == null)
            {
                return null;
            }

            Signal workbookSourceSignal = new Signal();
            workbookSourceSignal.Name = "Source";
            workbookSourceSignal.SamplingHZ = baseSignal.SamplingHZ;
            workbookSourceSignal.SampleSeconds = baseSignal.SampleSeconds;
            workbookSourceSignal.Type = SignalType.Output;

            double sampleRate = baseSignal.SamplingHZ; // hz
            List<double> timePoints = new List<double>((int)(2 * sampleRate));

            for (double timePointVal = 0; timePointVal < 2.0; timePointVal += (1.0 / sampleRate))
            {
                timePoints.Add(timePointVal);
            }

            List<double> sumSignal = new List<double>(timePoints.Count);

            foreach (double timePointVal in timePoints)
            {
                double signalValue = 0.0;
                foreach (Signal signal in signals.Where(sig => sig.Type == SignalType.Source))
                {
                    signalValue += signal.Amplitude * (Math.Sin(2 * Math.PI * signal.SignalHZ * timePointVal));
                }
                sumSignal.Add(signalValue);
            }

            workbookSourceSignal.Samples.Clear();
            workbookSourceSignal.Samples.AddRange(sumSignal);

            return workbookSourceSignal;
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
