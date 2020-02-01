using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SignalProcessor.Interfaces;

namespace SignalProcessor.Filters
{

    public class WindowedSyncFilter : IWindowedSyncFilter
    {
        public double CutoffFrequencySamplingFrequencyPercentage { get ; set ; }
        public int FilterLength { get ; set ; }
        public FILTERTYPE FilterType { get; set; }

        public WindowedSyncFilter()
        {
            FilterType = FILTERTYPE.LOWPASS; // Lowpass by default
        }

        /// <summary>
        /// Implementation of the algorithm from P 290 Equation 16-4 of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        /// https://www.dspguide.com/CH16.PDF#page=5&zoom=auto,-310,23
        /// Note this filter kernel assumes samples are normalized by the caller
        /// </summary>
        /// <returns></returns>
        public virtual IList<double> ImpulseResponse(bool normalize = true)
        {
            List<double> impulseResponse;
            impulseResponse = new List<double>(FilterLength + 1);
            double normalizationFactor = 0.0;
            for (int idx = 0; idx <= FilterLength; idx++)
            {
                if (idx == (FilterLength / 2))
                {
                    impulseResponse.Add(2 * Math.PI * (CutoffFrequencySamplingFrequencyPercentage / 100.0));
                }
                else
                {
                    double sincNumerator = (Math.Sin(2 * Math.PI * (CutoffFrequencySamplingFrequencyPercentage / 100.0) * (idx - (FilterLength / 2))));
                    double sincDenominator = (idx - (FilterLength / 2));
                    double blackmanWindow = (0.42 - (0.5 * Math.Cos((2 * Math.PI * idx)/FilterLength)) + (0.08 * Math.Cos((4 * Math.PI * idx)/FilterLength)));
                    double value = (sincNumerator / sincDenominator) * blackmanWindow;
                    impulseResponse.Add(value);
                }

                normalizationFactor += impulseResponse[impulseResponse.Count - 1];
            }

            if (normalize)
            {
                // Normalize for unity gain at DC
                for (int idx = 0; idx < impulseResponse.Count; idx++)
                {
                    impulseResponse[idx] = impulseResponse[idx] / normalizationFactor;
                }
            }

            // Spectral inversion
            if (FilterType == FILTERTYPE.HIGHPASS)
            {
                // Spectral Inversion of the low pass filter
                for (int idx = 0; idx < impulseResponse.Count; idx++)
                {
                    impulseResponse[idx] = impulseResponse[idx] * -1;
                }

                impulseResponse[(impulseResponse.Count - 1) / 2] += 1;
            }

            return impulseResponse;
        }

        public IList<Complex> FrequencyResponse()
        {
            ComplexFastFourierTransform transform = new ComplexFastFourierTransform();
            List<Complex> coefficients = transform.Transform(new List<double>(ImpulseResponse()), FilterLength).FourierCoefficients;

            return coefficients;
        }
    }
}
