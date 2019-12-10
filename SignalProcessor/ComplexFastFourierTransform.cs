using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace SignalProcessor
{
   
    public class ComplexFastFourierTransform : IDFT
    {
        /// <summary>
        /// Adapted from Udemy course "Master the Fourier transform and its applications" by Mike X Cohen
        /// </summary>
        /// <param name="freqDomain">Time Frequency Domain data</param>
        /// <returns></returns>
        public List<double> Synthesize(FrequencyDomain freqDomain)
        {
            List<Complex> fCoefs = freqDomain.FourierCoefficients;
            int freqDomainLen = fCoefs.Count;
            List<double> timeDomain = new List<double>(freqDomainLen);
            List<Complex> csw = new List<Complex>(freqDomainLen);
            List<Complex> reconstructedSignal = new List<Complex>(freqDomainLen);

            List<double> fourTime = new List<double>(freqDomainLen);

            // Setup the fourier time array
            for (int idx = 0; idx < freqDomainLen; idx++)
            {
                fourTime.Add((double)idx / (double)freqDomainLen);
                reconstructedSignal.Add(new Complex(0, 0));
            }

            for (int fi = 1; fi <= freqDomainLen; fi++)
            {
                Complex posOneI = new Complex(0, 1);
                Complex euler = new Complex(Math.E, 0);

                foreach (double timeVal in fourTime)
                {
                    Complex value = fCoefs[fi - 1] * Complex.Pow(euler, (posOneI * Math.PI * 2 * (fi - 1) * timeVal));
                    csw.Add(value);
                }

                for (int idx = 0; idx < csw.Count; idx++)
                {
                    reconstructedSignal[idx] += csw[idx];
                }

                csw.Clear();
            }

            for (int idx = 0; idx < reconstructedSignal.Count; idx++)
            {
                reconstructedSignal[idx] /= freqDomainLen;
            }

            foreach(Complex sigVal in reconstructedSignal)
            {
                timeDomain.Add(sigVal.Real);
            }

            return timeDomain;
        }

        /// <summary>
        /// Adapted from Udemy course "Master the Fourier transform and its applications" by Mike X Cohen
        /// </summary>
        /// <param name="timeDomain">Time series data</param>
        /// <returns></returns>
        public FrequencyDomain Transform(List<double> timeDomain, double sampleRateHz)
        {
            FrequencyDomain result = FindFourierCoeffecients(timeDomain, sampleRateHz);
            LoadFrequencyAmplitudes(result);
            return result;
        }

        private static FrequencyDomain FindFourierCoeffecients(List<double> timeDomain, double sampleRateHz)
        {
            int timeDomainLen = timeDomain.Count;
            // Prepare the Fourier Transform
            List<double> fourTime = new List<double>(timeDomainLen);
            List<Complex> fCoefs = new List<Complex>(timeDomainLen);

            List<Complex> csw = new List<Complex>(timeDomainLen);

            // Setup the fourier time array
            for (int idx = 0; idx < timeDomainLen; idx++)
            {
                fourTime.Add((double)idx / (double)timeDomainLen);
            }

            for (int fi = 1; fi <= timeDomainLen; fi++)
            {
                // Compute complex sine wave
                foreach (double timeVal in fourTime)
                {
                    Complex negOneI = new Complex(0, -1);
                    Complex euler = new Complex(Math.E, 0);

                    Complex value = Complex.Pow(euler, (negOneI * Math.PI * 2 * (fi - 1) * timeVal));
                    csw.Add(value);
                }

                // Compute dot product between signal and complex sine wave
                Complex dotProduct = new Complex(0, 0);
                for (int idx = 0; idx < timeDomainLen; idx++)
                {
                    dotProduct += ((csw[idx] * timeDomain[idx]));
                }

                fCoefs.Add(dotProduct);
                csw.Clear();
            }

            FrequencyDomain result = new FrequencyDomain();
            result.FourierCoefficients = fCoefs;
            result.SampleRateHz = sampleRateHz;
            return result;
        }

        private static FrequencyDomain LoadFourierCoefficients(List<double> frequencyAmplitudes)
        {
            FrequencyDomain frequencyDomain = new FrequencyDomain();

            foreach (double amplitude in frequencyAmplitudes)
            {
                double absValue = amplitude / 2 * frequencyAmplitudes.Count;
                // For now calculate at a 45 degree phase, not sure how to handle phase in this case
                Complex coefficient = new Complex(Math.Sqrt(absValue) / Math.Sqrt(2), Math.Sqrt(absValue) / Math.Sqrt(2));
                frequencyDomain.FourierCoefficients.Add(coefficient);
            }
            return frequencyDomain;
        }

        private static void LoadFrequencyAmplitudes(FrequencyDomain frequencyDomain)
        {
            double sampleRate = frequencyDomain.SampleRateHz;
            int points = frequencyDomain.FourierCoefficients.Count;

            List<double> frequencyVector = linspace(0, sampleRate / 2, (int)Math.Floor((double)((points / 2) + 1)));
            List<double> amplitudesVector = new List<double>(points);
            foreach (Complex coefficient in frequencyDomain.FourierCoefficients)
            {
                double amplitude = 2 * (Complex.Abs(coefficient) / points);
                amplitudesVector.Add(amplitude);
            }

            for (int idx = 0; idx < frequencyVector.Count; idx++)
            {
                frequencyDomain.FrequencyAmplitudes.Add(frequencyVector[idx], amplitudesVector[idx]);
            }
        }

        private static List<double> linspace(double start, double end, int numPoints)
        {
            List<double> vector = new List<double>(numPoints+5);

            double spacing = (end - start) / (double)(numPoints - 1);

            for (double value = start; value <= end; value += spacing)
            {
                vector.Add(value);
            }
            return vector;
        }
    }
}
