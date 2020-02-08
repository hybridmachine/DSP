using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using SignalProcessor;
using System.Numerics;

namespace SignalsAndTransforms.Models
{
    // A collection of signals, filters, and convolution kernels being actively worked on. Also contains the code
    // to save/load from disk. WorkBooks are managed by the WorkBookManager
    public class WorkBook : INotifyPropertyChanged
    {
        private Dictionary<string, Signal> m_signals;
        private Dictionary<string, WindowedSyncFilter> m_windowedSyncFilters;
        private Dictionary<string, CustomFilter> m_customFilters;

        public WorkBook()
        {
            // Default constructor used by Dapper, which loads the name property by mapping.
            m_signals = new Dictionary<string, Signal>();
            m_windowedSyncFilters = new Dictionary<string, WindowedSyncFilter>();
            m_customFilters = new Dictionary<string, CustomFilter>();
        }

        public WorkBook(String name)
        {
            Name = name;
            m_signals = new Dictionary<string, Signal>();
            m_windowedSyncFilters = new Dictionary<string, WindowedSyncFilter>();
        }
        public long Id { get; set; }
        public String Name { get; set; }
        public String FilePath { get; set; }
        public String Notes { get; set; }

        public Dictionary<string, Signal> Signals
        {
            get { return m_signals; }
        }

        public Dictionary<string, WindowedSyncFilter> WindowedSyncFilters
        {
            get { return m_windowedSyncFilters; }
        }

        public Dictionary<string, CustomFilter> CustomFilters
        {
            get { return m_customFilters; }
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

        /// <summary>
        /// Band pass
        /// </summary>
        /// <returns></returns>
        public List<double> ConvolvedFilterImpulseResponse()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Band reject
        /// </summary>
        /// <returns></returns>
        public List<double> SummedFilterImpulseResponse(bool normalize = true)
        {
            IList<double> summedImpulseResponse = null;
            foreach (var filter in WindowedSyncFilters.Values.Where(filt => filt.IsActive))
            {
                // For now we sum the filters
                // TODO add convolution as an option
                // We assume all filters have the same filter length
                // See https://www.dspguide.com/CH14.PDF#page=14&zoom=auto,-119,688 
                // Page 274 chapter 14 of "The Scientist and Engineer's Guide to Digital Signal Processing"
                if (null == summedImpulseResponse)
                {
                    summedImpulseResponse = filter.ImpulseResponse(normalize);
                }
                else
                {
                    IList<double> filterImpulseResponse = filter.ImpulseResponse(normalize);

                    // Ignore any filters that don't have the same filter length
                    if (filterImpulseResponse.Count == summedImpulseResponse.Count)
                    {
                        for (int idx = 0; idx < summedImpulseResponse.Count; idx++)
                        {
                            summedImpulseResponse[idx] += filterImpulseResponse[idx];
                        }
                    }
                }
            }

            foreach (var filter in CustomFilters.Values.Where(filt => filt.IsActive))
            {
                if (null == summedImpulseResponse)
                {
                    summedImpulseResponse = filter.ImpulseResponse(normalize);
                }
                else
                {
                    IList<double> filterImpulseResponse = filter.ImpulseResponse(normalize);

                    // Ignore any filters that don't have the same filter length
                    if (filterImpulseResponse.Count == summedImpulseResponse.Count)
                    {
                        for (int idx = 0; idx < summedImpulseResponse.Count; idx++)
                        {
                            summedImpulseResponse[idx] += filterImpulseResponse[idx];
                        }
                    }
                }
            }

            return (List<double>)summedImpulseResponse;
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

        /// <summary>
        /// Export the supplied frequency domain to the supplied outputStream
        /// Writes the frequency domain data as a CSV format of magnitude,phase for each entry
        /// Note this does not stamp the header on the data, callers should do that on the stream before calling this
        /// </summary>
        /// <param name="frequencyDomain">Frequency Domain to serialize</param>
        /// <param name="outputStream">Output stream to write data to</param>
        /// <returns></returns>
        public static async Task<bool> SerializeFrequencyDomain(FrequencyDomain frequencyDomain, StreamWriter outputStream)
        {
            bool success = true;

            foreach (Complex coefficient in frequencyDomain.FourierCoefficients)
            {
                await outputStream.WriteLineAsync($"{coefficient.Magnitude},{coefficient.Phase}");
            }
            return success;
        }
    }
}
