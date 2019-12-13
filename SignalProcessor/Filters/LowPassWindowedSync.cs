using System;
using System.Collections.Generic;
using System.Text;

namespace SignalProcessor.Filters
{
    public class LowPassWindowedSync : IWindowedSyncFilter
    {
        public double CutoffFrequencySamplingFrequencyPercentage { get ; set ; }
        public int FilterLength { get ; set ; }

        private List<double> m_impulseResponse;
        /// <summary>
        /// Implementation of the algorithm from P 290 Equation 16-4 of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        /// https://www.dspguide.com/CH16.PDF#page=5&zoom=auto,-310,23
        /// Note this filter kernel assumes samples are normalized by the caller
        /// </summary>
        /// <returns></returns>
        public List<double> ImpulseResponse()
        {
            if (m_impulseResponse != null)
            {
                return m_impulseResponse;
            }

            m_impulseResponse = new List<double>(FilterLength + 1);
            for (int idx = 0; idx <= FilterLength; idx++)
            {
                if (idx == (FilterLength / 2))
                {
                    m_impulseResponse.Add(2 * Math.PI * CutoffFrequencySamplingFrequencyPercentage);
                }
                else
                {
                    double sincNumerator = (Math.Sin(2 * Math.PI * CutoffFrequencySamplingFrequencyPercentage * (idx - (FilterLength / 2))));
                    double sincDenominator = (idx - (FilterLength / 2));
                    double blackmanWindow = (0.42 - (0.5 * Math.Cos((2 * Math.PI * idx)/FilterLength)) + (0.08 * Math.Cos((4 * Math.PI * idx)/FilterLength)));
                    double value = (sincNumerator / sincDenominator) * blackmanWindow;
                    m_impulseResponse.Add(value);
                }
            }

            return m_impulseResponse;
        }
    }
}
