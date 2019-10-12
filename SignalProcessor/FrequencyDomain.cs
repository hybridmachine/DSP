using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessor
{
    public class FrequencyDomain
    {
        public List<double> RealComponent;
        public List<double> ScalingFactor; // When filtering, the scaling factor for each frequency component in real domain
        public List<double> ImaginaryComponent;
        public int frequencyDomainLen;

        /// <summary>
        /// This constructor is used by the JSON to POCO system in the web service, which then sets the publicly available
        /// properties directly
        /// </summary>
        public FrequencyDomain()
        {
           
        }
        public FrequencyDomain(int timeDomainLen, IDFT dft)
        {
            frequencyDomainLen = 0;
            if (dft is CorrelationFourierTransform)
            {
                frequencyDomainLen = (timeDomainLen / 2) + 1;
            }
            else if (dft is FastFourierTransform)
            {
                frequencyDomainLen = timeDomainLen;
            }

            RealComponent = new List<double>(frequencyDomainLen);
            ScalingFactor = new List<double>(frequencyDomainLen);
            ImaginaryComponent = new List<double>(frequencyDomainLen);

            for (int K = 0; K < frequencyDomainLen; K++)
            {
                RealComponent.Add(0.0);
                ImaginaryComponent.Add(0.0);
                ScalingFactor.Add(1.0);
            }
        }

        // Copy constructor
        public FrequencyDomain(FrequencyDomain rh)
        {
            this.RealComponent = new List<double>(rh.RealComponent);
            this.ScalingFactor = new List<double>(rh.ScalingFactor);
            this.ImaginaryComponent = new List<double>(rh.ImaginaryComponent);
            this.frequencyDomainLen = rh.frequencyDomainLen;
        }

        /// <summary>
        /// Users can call this and alter the filter values (low to high, 0 -> Count)
        /// </summary>
        /// <returns></returns>
        public void ApplyFilter(IDFTFilter filter)
        {
            if (null == filter)
            {
                for (int idx = 0; idx < ScalingFactor.Count; idx++)
                {
                    ScalingFactor[idx] = 1.0;
                }
            }
            else
            {
                filter.ScaleFrequencies(ScalingFactor);
            }
        }

        public double ScaledRealComponent(int K)
        {
            if (K >= RealComponent.Count)
                return 0.0;

            return (RealComponent[K] * ScalingFactor[K]);
        }

        public double ScaledImaginaryComponent(int K)
        {
            if (K >= ImaginaryComponent.Count)
                return 0.0;

            return (ImaginaryComponent[K] * ScalingFactor[K]);
        }


        // See page 162, Equation 8-6 and 8-7 in "The Scientist and Engineer's Guide to Digital Signal Processing
        #region polarcoords
        public double Magnitude(int k)
        {
            return Math.Sqrt(Math.Pow(ScaledRealComponent(k), 2) + Math.Pow(ScaledImaginaryComponent(k), 2));
        }

        public double Phase(int k)
        {
            if (ScaledRealComponent(k) == 0)
                return Math.PI / 2.0;

            return Math.Atan(ScaledImaginaryComponent(k) / ScaledRealComponent(k));
        }
        #endregion
    }
}
