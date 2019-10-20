using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SignalProcessor
{
    public class RealFastFourierTransform : IDFT
    {

        private FrequencyDomain Transform(FrequencyDomain frequencyDomain)
        {
            int n = frequencyDomain.RealComponent.Count;
            int nm1 = n - 1;
            int nd2 = n / 2;
            int m = (int)(Math.Log(n, 2));
            int j = nd2;

            #region bitreversal
            int k;
            for (int idx = 1; idx <= (n - 2); idx++)
            {
                if (idx <= j)
                {
                    double tr = frequencyDomain.RealComponent[j];
                    double ti = frequencyDomain.ImaginaryComponent[j];
                    frequencyDomain.RealComponent[j] = frequencyDomain.RealComponent[idx];
                    frequencyDomain.ImaginaryComponent[j] = frequencyDomain.ImaginaryComponent[idx];
                    frequencyDomain.RealComponent[idx] = tr;
                    frequencyDomain.ImaginaryComponent[idx] = ti;
                }
                k = nd2;
                while (!(k>j))
                {
                    j = j - k;

                    if (k >= 2)
                    {
                        k = k / 2;
                    }
                }
                j = j + k;
            }
            #endregion

            #region stageLoop
            for (int L = 1; L <= m; L++)
            {
                int le = (int)Math.Pow(2, L);
                int le2 = le / 2;
                double ur = 1;
                double ui = 0;

                double sr = Math.Cos(Math.PI / le2);
                double si = -1 * Math.Sin(Math.PI / le2);
                double tr;
                double ti;

                for (j = 1; j <= le2; j++)
                {
                    int jm1 = j - 1;
                    for (int idx = jm1; idx < nm1; idx += le)
                    {
                        int ip = (idx + le2) - 1;
                        tr = frequencyDomain.RealComponent[ip] * ur - frequencyDomain.ImaginaryComponent[ip] * ui;
                        ti = frequencyDomain.RealComponent[ip] * ui + frequencyDomain.ImaginaryComponent[ip] * ur;
                        frequencyDomain.RealComponent[ip] = frequencyDomain.RealComponent[idx] - tr;
                        frequencyDomain.ImaginaryComponent[ip] = frequencyDomain.ImaginaryComponent[idx] - ti;
                        frequencyDomain.RealComponent[idx] = frequencyDomain.RealComponent[idx] + tr;
                        frequencyDomain.ImaginaryComponent[idx] = frequencyDomain.ImaginaryComponent[idx] + ti;
                    }
                    tr = ur;
                    ur = tr * sr - ui * si;
                    ui = tr * si + ui * sr;
                }
            }
            #endregion

            return frequencyDomain;
        }
        /// <summary>
        /// This algorithm is derived from Chapter 12 (table 12-4) of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        /// </summary>
        /// <param name="timeDomain"></param>
        /// <returns></returns>
        public FrequencyDomain Transform(List<double> timeDomain)
        {
            FrequencyDomain fftResult = new FrequencyDomain(timeDomain.Count, this);
            // The FFT operates in place, store the timeDomain samples in the real component (leave the imaginary compoenent zeroed)
            fftResult.RealComponent.Clear();
            fftResult.RealComponent.AddRange(timeDomain);
            fftResult.ImaginaryComponent.Clear();
            fftResult.ImaginaryComponent.Capacity = timeDomain.Count;
            for (int idx = 0; idx < timeDomain.Count; idx++)
            {
                fftResult.ImaginaryComponent.Add(0.0);
            }

            return Transform(fftResult);
        }

        /// <summary>
        /// This algorithm is derived from Table 12-5 of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        /// Inverse Fourier Transform
        /// </summary>
        /// <param name="frequencyDomain"></param>
        /// <returns></returns>
        public List<double> Synthesize(FrequencyDomain frequencyDomain)
        {
            for (int k = 0; k < frequencyDomain.frequencyDomainLen; k++)
            {
                frequencyDomain.ImaginaryComponent[k] *= -1;
            }

            // Transformation happens in place
            Transform(frequencyDomain);

            // Chainge the sign of the imaginary component
            for (int i = 0; i < frequencyDomain.frequencyDomainLen; i++)
            {

                frequencyDomain.RealComponent[i] /= frequencyDomain.frequencyDomainLen;
                frequencyDomain.ImaginaryComponent[i] = -1 * frequencyDomain.ImaginaryComponent[i] / frequencyDomain.frequencyDomainLen;
            }

            return frequencyDomain.RealComponent;
        }
    }
}
