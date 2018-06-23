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
        public List<double> RealScalingFactor; // When filtering, the scaling factor for each frequency component in real domain
        public List<double> ImaginaryComponent;

        public FrequencyDomain(int timeDomainLen)
        {
            int freqDomainLen = (timeDomainLen / 2) + 1;
            RealComponent = new List<double>(freqDomainLen);
            RealScalingFactor = new List<double>(freqDomainLen);
            ImaginaryComponent = new List<double>(freqDomainLen);

            for (int K = 0; K < freqDomainLen; K++)
            {
                RealComponent.Add(0.0);
                ImaginaryComponent.Add(0.0);
                RealScalingFactor.Add((double)K/freqDomainLen); // For now test high pass filter 
            }
        }

        public double ScaledRealComponent(int K)
        {
            if (K >= RealComponent.Count)
                return 0.0;

            return (RealComponent[K] * RealScalingFactor[K]);
        }

        public double ScaledImaginaryComponent(int K)
        {
            if (K >= ImaginaryComponent.Count)
                return 0.0;

            return (ImaginaryComponent[K] * RealScalingFactor[K]);
        }
    }

    public class DFT
    {
        /// <summary>
        /// This algorithm is derived from P 160 (Table 8-2) of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        /// </summary>
        /// <param name="timeDomain"></param>
        /// <returns></returns>
        public static FrequencyDomain Transform(List<double> timeDomain)
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
