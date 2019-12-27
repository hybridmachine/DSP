using System;
using System.Collections.Generic;
using System.Text;

namespace SignalProcessor.Filters
{
    public enum FilterType
    {
        LOWPASS,
        HIGHPASS
    }

    public class WindowedSyncFilter : IWindowedSyncFilter
    {
        /// <summary>
        /// Convenience parameter so users can name their filters for sorting, searching, etc
        /// </summary>
        public string Name { get; set; }
        public double CutoffFrequencySamplingFrequencyPercentage { get ; set ; }
        public int FilterLength { get ; set ; }
        public FilterType FilterType { get; set; }

        public WindowedSyncFilter()
        {
            FilterType = FilterType.LOWPASS; // Lowpass by default
        }

        protected List<double> m_impulseResponse;
        /// <summary>
        /// Implementation of the algorithm from P 290 Equation 16-4 of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        /// https://www.dspguide.com/CH16.PDF#page=5&zoom=auto,-310,23
        /// Note this filter kernel assumes samples are normalized by the caller
        /// </summary>
        /// <returns></returns>
        public virtual List<double> ImpulseResponse()
        {
            if (m_impulseResponse != null)
            {
                return m_impulseResponse;
            }

            m_impulseResponse = new List<double>(FilterLength + 1);
            double normalizationFactor = 0.0;
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

                normalizationFactor += m_impulseResponse[m_impulseResponse.Count - 1];
            }

            // Normalize for unity gain at DC
            for (int idx = 0; idx < m_impulseResponse.Count; idx++)
            {
                m_impulseResponse[idx] = m_impulseResponse[idx] / normalizationFactor;
            }

            // Spectral inversion
            if (FilterType == FilterType.HIGHPASS)
            {
                // Spectral Inversion of the low pass filter
                for (int idx = 0; idx < m_impulseResponse.Count; idx++)
                {
                    m_impulseResponse[idx] = m_impulseResponse[idx] * -1;
                }

                m_impulseResponse[(m_impulseResponse.Count - 1) / 2] += 1;
            }

            return m_impulseResponse;
        }
    }
}
