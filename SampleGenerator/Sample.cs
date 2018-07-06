/***
 * Generate synthetic samples, add/subtract samples
 * 
 * */
using SignalGenerator;
using SignalProcessor;
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
        protected FrequencyDomain _frequencyDomain;
        protected List<double> _samples;
        protected List<double> _sliceSignal; // The signal data for a slice
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

        public FrequencyDomain GetFrequencyDomainForSlice
        {
            get
            {
                return _frequencyDomain;
            }
        }

        public List<Double> GetSignalSlice
        {
            get
            {
                return _sliceSignal;
            }
        }
        
        // 64 channel DFT done on 50 samples + 14 zeros padded right
        public List<double> Get50Padded64ChannelSamples()
        {
            int numSamplesToGet = 32;
            int curIDX = _sampleIDX;
            if (curIDX + numSamplesToGet >= _samples.Count)
            {
                numSamplesToGet = _samples.Count - curIDX - 1;
                if (numSamplesToGet <= 0)
                {
                    // Roll over to the beginning
                    _sampleIDX = 0;
                    return Get50Padded64ChannelSamples();
                }
            }
            _sampleIDX += numSamplesToGet;
            _sliceSignal = _samples.GetRange(curIDX, numSamplesToGet);

            List<double> paddedSignal = new List<double>(_sliceSignal);
            // Pad
            int padLen  = (64 - numSamplesToGet);
            for (int cnt = 0; cnt < padLen; cnt++)
                paddedSignal.Add(0.0);

            _frequencyDomain = DFT.Transform(paddedSignal);
            return _sliceSignal;
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
            _sliceSignal = _samples.GetRange(curIDX, numSamplesToGet);
            _frequencyDomain = DFT.Transform(_sliceSignal);
            return _sliceSignal;
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
