using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using SignalProcessor;
using System.Numerics;
using SignalsAndTransforms.Interfaces;

namespace SignalsAndTransforms.Models
{
    // A collection of signals, filters, and convolution kernels being actively worked on. Also contains the code
    // to save/load from disk. WorkBooks are managed by the WorkBookManager
    public class WorkBook : INotifyPropertyChanged
    {
        private const string FILTERCOMBINEMODE = "FilterCombineMode";

        public WorkBook()
        {
            // Default constructor used by Dapper, which loads the name property by mapping.
            Signals = new Dictionary<string, Signal>();
            WindowedSyncFilters = new Dictionary<string, WindowedSyncFilter>();
            CustomFilters = new Dictionary<string, CustomFilter>();
            Settings = new Dictionary<string, string>();
        }

        public WorkBook(String name) : this()
        {
            Name = name;
        }
        public long Id { get; set; }
        public String Name { get; set; }
        public String FilePath { get; set; }
        public String Notes { get; set; }

        public Dictionary<string, Signal> Signals { get; }

        public Dictionary<string, WindowedSyncFilter> WindowedSyncFilters { get; }

        public Dictionary<string, CustomFilter> CustomFilters { get; }

        public Dictionary<string, string> Settings { get; }

        public Signal ConvolutionKernel {
            get
            {
                Signal signal = null;
                if (Signals.ContainsKey("ConvolutionKernel"))
                {
                    signal = Signals["ConvolutionKernel"];
                }
                return signal;
            }

            set
            {
                Signals["ConvolutionKernel"] = value;
            }
        }

        public List<double> CombinedFilterImpulseResponse(bool normalize = true)
        {
            if (true == SumModeActive)
            {
                return SummedFilterImpulseResponse(normalize);
            }
            else
            {
                return ConvolvedFilterImpulseResponse(normalize);
            }
        }

        /// <summary>
        /// Remove the specified filter
        /// </summary>
        /// <param name="deleteMe"></param>
        public void DeleteFilter(IFilterIdentifier deleteMe)
        {
            if (deleteMe is CustomFilter)
            {
                CustomFilters.Remove(deleteMe.Name);
            }
            else if (deleteMe is WindowedSyncFilter)
            {
                WindowedSyncFilters.Remove(deleteMe.Name);
            }
        }

        /// <summary>
        /// Band pass
        /// </summary>
        /// <returns></returns>
        public List<double> ConvolvedFilterImpulseResponse(bool normalize = true)
        {
            IList<double> convolvedImpulseResponse = null;
            foreach (var filter in WindowedSyncFilters.Values.Where(filt => filt.IsActive))
            {
                if (null == convolvedImpulseResponse)
                {
                    convolvedImpulseResponse = filter.ImpulseResponse(normalize);
                }
                else
                {
                    DSP.Convolution convolver = new DSP.Convolution();
                    IList<double> convolvedTemp = convolver.Convolve(new List<double>(convolvedImpulseResponse), new List<double>(filter.ImpulseResponse(normalize)), DSP.ConvolutionType.INPUTSIDE);

                    convolvedImpulseResponse = convolvedTemp;
                }
            }

            if (null == convolvedImpulseResponse) return null;

            return new List<double>(convolvedImpulseResponse);
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
        #region settings
        /// <summary>
        /// Sum the filters
        /// </summary>
        public bool SumModeActive
        {
            get
            {
                if (Settings.ContainsKey(FILTERCOMBINEMODE))
                {
                    if (Settings[FILTERCOMBINEMODE].Trim().ToUpperInvariant() == nameof(SumModeActive).ToUpperInvariant())
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
                if (value == true)
                {
                    Settings[FILTERCOMBINEMODE] = nameof(SumModeActive).ToUpperInvariant();
                }
                // If false, we assume that the radio button true value for ConvolveModeActive will set the setting
            }
        }

        /// <summary>
        /// Convolve the filters
        /// </summary>
        public bool ConvolveModeActive
        {
            get
            {
                if (Settings.ContainsKey(FILTERCOMBINEMODE))
                {
                    if (Settings[FILTERCOMBINEMODE].Trim().ToUpperInvariant() == nameof(ConvolveModeActive).ToUpperInvariant())
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
                if (value == true)
                {
                    Settings[FILTERCOMBINEMODE] = nameof(ConvolveModeActive).ToUpperInvariant();
                }
                // If false, we assume that the radio button true value for SumModeActive will set the setting
            }
        }
        #endregion
    }
}
