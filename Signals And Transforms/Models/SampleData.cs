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

                        Sample sinusoidSamp = new Sample(8000, 1, 100, sinusoid);
                        Sample sinusoidSamp2 = new Sample(8000, 1, 2000, sinusoid);
                        Sample whiteNoise = new Sample(16000, 1, 1000, random);
                        Sample squareWave = new Sample(8000, 1, 400, square);
                        _signalSample = squareWave; //sinusoidSamp.SumWithSample(sinusoidSamp2);//.SumWithSample(whiteNoise);
                    }

                    _signalSample.GetNextSamplesForTimeSlice(20); // Prime the pump
                    return _signalSample;
                }
            }
        }

        public static void SetFilter(PASSTYPE _filterType)
        {
            if (_filterType == PASSTYPE.NONE)
            {
                _filter = null;
            }
            else
            {
                _filter = new SimplePassFilter(0.15, _filterType);
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

        public static List<double> GetNextSamplesForTimeSlice(int millis)
        {
            List<double> sliceSignal = SignalSample.GetNextSamplesForTimeSlice(20);

            NewSlice?.Invoke(null, null);
            return sliceSignal;
        }

        public static event EventHandler NewSlice;
    }
}
