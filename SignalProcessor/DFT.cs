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

        public FrequencyDomain(int timeDomainLen)
        {
            int freqDomainLen = (timeDomainLen / 2) + 1;
            RealComponent = new List<double>(freqDomainLen);
            ScalingFactor = new List<double>(freqDomainLen);
            ImaginaryComponent = new List<double>(freqDomainLen);

            for (int K = 0; K < freqDomainLen; K++)
            {
                RealComponent.Add(0.0);
                ImaginaryComponent.Add(0.0);
                ScalingFactor.Add(1.0);
            }
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

    public class DFT
    {
        /// <summary>
        /// This algorithm is derived from Chapter 12 (table 12-4) of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        /// </summary>
        /// <param name="timeDomain"></param>
        /// <returns></returns>
        public static FrequencyDomain FastFourierTransform(List<double> timeDomain)
        {
            FrequencyDomain fftResult = new FrequencyDomain(timeDomain.Count);
            // The FFT operates in place, store the timeDomain samples in the real component (leave the imaginary compoenent zeroed)
            fftResult.RealComponent.Clear();
            fftResult.RealComponent.AddRange(timeDomain);

            int n = timeDomain.Count;
            int nm1 = n - 1;
            int nd2 = n / 2;
            int m = (int)(Math.Log(n) / Math.Log(2));
            int j = nd2;

            #region bitreversal
            int k;
            for (int idx = 1; idx < (n - 2); idx++)
            {
                if (j < idx)
                { 
                    double tr = fftResult.RealComponent[j];
                    double ti = fftResult.ImaginaryComponent[j];
                    fftResult.RealComponent[j] = fftResult.RealComponent[idx];
                    fftResult.ImaginaryComponent[j] = fftResult.ImaginaryComponent[idx];
                    fftResult.RealComponent[idx] = tr;
                    fftResult.ImaginaryComponent[idx] = ti;
                }
                k = nd2;
                while (j <= k)
                {
                    j = j + k;
                    k=k/2;
                }
                j = j + k;
            }
            #endregion

            #region stageLoop
            for (int L = 1; L <= m; L++)
            {
                int le = (int)2 ^ L;
                int le2 = le / 2;
                int ur = 1;
                int ui = 0;

                double sr = Math.Cos(Math.PI / le2);
                double si = -1 * Math.Sin(Math.PI / le2);

                // TODO, complete from table 12-4 http://www.dspguide.com/ch12/3.htm

            }
            #endregion

            return fftResult;
        }

        /// <summary>
        /// This algorithm is derived from P 160 (Table 8-2) of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        /// </summary>
        /// <param name="timeDomain"></param>
        /// <returns></returns>
        public static FrequencyDomain CorrelationTransform(List<double> timeDomain)
        {
            int timeDomainLen = timeDomain.Count;
            FrequencyDomain frequencyDomain = new FrequencyDomain(timeDomainLen);

            int freqDomainLen = frequencyDomain.RealComponent.Count;

            for (int K = 0; K < freqDomainLen; K++)
            {
                for (int I = 0; I < timeDomainLen; I++)
                {
                    frequencyDomain.RealComponent[K] += timeDomain[I] * Math.Cos(2 * Math.PI * K * I / timeDomainLen);
                    frequencyDomain.ImaginaryComponent[K] -= timeDomain[I] * Math.Sin(2 * Math.PI * K * I / timeDomainLen);
                }
            }
            return frequencyDomain;
        }

        /// <summary>
        /// This algorithm is derived from P 154 (Table 8-1) of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        /// </summary>
        /// <param name="frequencyDomain"></param>
        /// <returns></returns>
        public static List<double> Synthesize(FrequencyDomain frequencyDomain)
        {
            int freqDomainLen = frequencyDomain.RealComponent.Count;
            int timeDomainLen = freqDomainLen * 2 - 1;
            List<double> timeDomain = new List<double>(timeDomainLen);

            List<double> cosineAmplitudes = new List<double>(freqDomainLen);
            List<double> sineAmplitudes = new List<double>(freqDomainLen);

            for (int K = 0; K < freqDomainLen; K++)
            {
                cosineAmplitudes.Add(frequencyDomain.ScaledRealComponent(K) / (timeDomainLen / 2));
                sineAmplitudes.Add(-1 * frequencyDomain.ScaledImaginaryComponent(K) / (timeDomainLen / 2));
            }
            
            // Fixup the endpoints
            cosineAmplitudes[0] = cosineAmplitudes[0] / 2;
            cosineAmplitudes[freqDomainLen - 1] = cosineAmplitudes[freqDomainLen - 1] / 2;

            for (int I = 0; I < timeDomainLen; I++)
            {
                timeDomain.Add(0.0);
            }

            for (int K = 0; K < freqDomainLen; K++)
            {
                for (int I = 0; I < timeDomainLen; I++)
                {
                    timeDomain[I] += cosineAmplitudes[K] * Math.Cos(2 * Math.PI * K * I / timeDomainLen);
                    timeDomain[I] += sineAmplitudes[K] * Math.Sin(2 * Math.PI * K * I / timeDomainLen);
                }
            }

            return timeDomain;
        }
    }
}
