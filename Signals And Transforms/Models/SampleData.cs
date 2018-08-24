using SampleGenerator;
using SignalGenerator;
using SignalGenerator.Generators;
using SignalProcessor;
using SignalProcessor.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals_And_Transforms.Models
{
    public class SampleData
    {
        private static Sample _signalSample;
        private static IDFTFilter _filter;

        private static Object _getLock = new Object();
        public static Sample SignalSample
        {
            get
            {
                lock (_getLock)
                {
                    if (_signalSample == null)
                    {
                        ISignalGenerator sinusoid = new Sinusoid();
                        ISignalGenerator random = new WhiteNoise();
                        ISignalGenerator square = new SquareIshWave();

                        Sample sinusoidSamp = new Sample(8000, 1, 500, sinusoid);
                        Sample sinusoidSamp2 = new Sample(16000, 1, 7000, sinusoid);
                        Sample whiteNoise = new Sample(16000, 1, 1000, random);
                        Sample squareWave = new Sample(8000, 1, 400, square);
                        _signalSample = sinusoidSamp;//.SumWithSample(whiteNoise);
                    }

                    _signalSample.Get50Padded64ChannelSamples(); // Prime the pump
                    return _signalSample;
                }
            }
        }

        public static void SetFilter(PASSTYPE _filterType, double clipPercent)
        {
            if (_filterType == PASSTYPE.NONE)
            {
                _filter = null;
            }
            else
            {
                _filter = new SimplePassFilter(clipPercent, _filterType);
            }
        }

        public static FrequencyDomain FreqDomain
        {
            get
            {
                FrequencyDomain fDomain = _signalSample.GetFrequencyDomainForSlice;
                fDomain.ApplyFilter(_filter);

                return fDomain;

            }
        }

        /// <summary>
        /// Get the next 50 samples, the Frequency Domain is padded to 64 in its calculation
        /// </summary>
        /// <returns></returns>
        public static List<double> Get50Padded64ChannelSamples()
        {
            NewSlice?.Invoke(null, null);
            return SignalSample.Get50Padded64ChannelSamples();
        }

        public static List<double> GetNextSamplesForTimeSlice(int millis)
        {
            NewSlice?.Invoke(null, null);
            return SignalSample.GetNextSamplesForTimeSlice(20);
        }

        public static event EventHandler NewSlice;
    }
}
