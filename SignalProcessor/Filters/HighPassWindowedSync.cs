using System;
using System.Collections.Generic;
using System.Text;

namespace SignalProcessor.Filters
{
    public class HighPassWindowedSync : LowPassWindowedSync
    {
        /// <summary>
        /// Implementation of the algorithm from P 290 Equation 16-4 of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        /// https://www.dspguide.com/CH16.PDF#page=5&zoom=auto,-310,23
        /// Note this filter kernel assumes samples are normalized by the caller
        /// </summary>
        /// <returns></returns>
        public override List<double> ImpulseResponse()
        {
            base.ImpulseResponse();

            // Spectral Inversion of the low pass filter
            for (int idx = 0; idx < m_impulseResponse.Count; idx++)
            {
                m_impulseResponse[idx] = m_impulseResponse[idx] * -1;
            }

            m_impulseResponse[(m_impulseResponse.Count - 1)/2] += 1;

            return m_impulseResponse;
        }
    }
}
