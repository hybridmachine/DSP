/***
 * Generate synthetic samples, add/subtract samples
 * 
 * */
using SignalGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleGenerator
{
    public class Sample
    {
        protected int _sampleRate;
        protected double _seconds;
        protected double _hertz;
        protected List<double> _samples;
        int _sampleIDX;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleRate"> Number of samples per second</param>
        /// <param name="seconds">Length of signal</param>
        /// <param name="hertz">Cycles per second</param>
        /// <param name="sigGen">Signal Generator</param>
        public Sample(int sampleRate, double seconds, double hertz, ISignalGenerator sigGen)
        {
            _sampleRate = sampleRate;
            _seconds = seconds;
            _hertz = hertz;

            int sampleCount = (int)Math.Ceiling(_sampleRate * _seconds);
            int cycleCount = (int)Math.Ceiling(_seconds * _hertz);

            if (sigGen != null)
            { 
                _samples = sigGen.GetSignal(sampleCount, cycleCount);
            }
            else
            {
                _samples = new List<double>(cycleCount); // Create empty sample list
            }
            _sampleIDX = 0;
        }

        public List<double>GetNextSamplesForTimeSlice(double milliseconds)
        {
            int numSamplesToGet = (int)Math.Floor((milliseconds / 1000) * _sampleRate);
            int curIDX = _sampleIDX;
            if (curIDX + numSamplesToGet >= _samples.Count)
            {
                numSamplesToGet = _samples.Count - curIDX - 1;
                if (numSamplesToGet <= 0)
                { 
                    // Roll over to the beginning
                    _sampleIDX = 0;
                    return GetNextSamplesForTimeSlice(milliseconds);
                }
            }
            _sampleIDX += numSamplesToGet;
            return _samples.GetRange(curIDX, numSamplesToGet);
        }

        public void ResetSampleIndex()
        {
            _sampleIDX = 0;
        }

        public Sample SumWithSample(Sample sampleToAdd)
        {
            if (_sampleRate != sampleToAdd._sampleRate || _seconds != sampleToAdd._seconds)
            {
                // In the future we can normalize but for now expect matching signal geometries to add
                return null;
            }

            Sample sum = new Sample(_sampleRate, _seconds, _hertz, null);

            sum._samples.Clear();
            for (int idx = 0; idx < _samples.Count; idx++)
            {
                sum._samples.Add(this._samples[idx] + sampleToAdd._samples[idx]);
            }

            return sum;
        }
    }
}
