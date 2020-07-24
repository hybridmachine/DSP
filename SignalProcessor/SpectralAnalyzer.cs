using SignalProcessor.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalProcessor
{
    public class SpectralAnalyzer
    {
        /// <summary>
        /// Calculate the average frequency response of a signal using an FFT of len dftLen
        /// This will pad the input signal to an even multiple of dftLen then take the FFT over
        /// that sample and then average the magnitudes (ignoring phase) of this signal, the magnitudes
        /// are returned in a list from 0 to dftLen - 1
        /// </summary>
        /// <param name="sample"></param>
        /// <param name="dftLen"></param>
        /// <param name="sampleHZ"></param>
        /// <returns></returns>
        public static Dictionary<double,double> AveragedFrequency(List<double> sample, int dftLen, double sampleHZ)
        {
            Dictionary<double,double> averagedFrequencyRespose = new Dictionary<double, double>();

            int remainder = 0;
            Math.DivRem(sample.Count, dftLen, out remainder);

            if (remainder > 0)
            {
                int pad = dftLen - remainder;
                for (int idx = 0; idx < pad; idx++)
                {
                    sample.Add(0.0);
                }
            }

            Math.DivRem(sample.Count, dftLen, out remainder);
            if (remainder > 0)
            {
                throw new Exception("Unable to pad sample");
            }

            int iterationCount = 0;
            for (int idx = 0; idx < sample.Count; idx += dftLen)
            {
                List<double> analyzeBlock = HammingWindowedSignal(sample.GetRange(idx, dftLen));

                IDFT dft = new DSPGuideComplexDiscreteFourierTransform();

                FrequencyDomain result = dft.Transform(analyzeBlock, sampleHZ);
                foreach (double key in result.FrequencyAmplitudes.Keys)
                {
                    if (!averagedFrequencyRespose.ContainsKey(key))
                    {
                        averagedFrequencyRespose.Add(key, 0.0);
                    }
                    
                    averagedFrequencyRespose[key] += result.FrequencyAmplitudes[key];
                }
 
                iterationCount++;
            }

            List<double> keys = new List<double>(averagedFrequencyRespose.Keys);
            foreach (double key in keys)
            {
                averagedFrequencyRespose[key] /= iterationCount;
            }

            return averagedFrequencyRespose;
        }

        public static List<double> HammingWindowedSignal(List<double> signal)
        {
            if (signal == null)
                return null;

            int M = signal.Count;
            double twoPi = Math.PI * 2;

            List<double> hammingWindowedSignal = new List<double>(M + 1);

            // Derived from Chapter 16 equation 16.1
            // w[i] = 0.54 - 0.46cos(2*Pi * i/M)
            // i running from 0 to M. Here we are limiting idx to 0 -> M - 1, not sure if this is the right approach
            // should I include the last item in the window?
            for (int idx = 0; idx < M; idx++)
            {
                hammingWindowedSignal.Add(signal[idx] * (0.54 - (0.46 * Math.Cos((twoPi * idx) / M))));
            }

            return hammingWindowedSignal;
        }
    }   
}
