using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using SignalProcessor.Interfaces;
using System.Collections.Concurrent;

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
            int numProcs = Environment.ProcessorCount;
            int concurrencyLevel = numProcs * 2;

            // Prepare the Fourier Transform
            Dictionary<int,double> fourTime = new Dictionary<int,double>(timeDomainLen);
            ConcurrentDictionary<int, Complex> csw = new ConcurrentDictionary<int, Complex>(concurrencyLevel, timeDomainLen);

            List<Complex> fCoefs = new List<Complex>(timeDomainLen);

            // Setup the fourier time array
            for (int idx = 0; idx < timeDomainLen; idx++)
            {
                fourTime.Add(idx,((double)idx / (double)timeDomainLen));
            }

            for (int fi = 1; fi <= timeDomainLen; fi++)
            {
                Parallel.ForEach(fourTime, (timeVal) =>
                {
                    Complex negOneI = new Complex(0, -1);
                    Complex euler = new Complex(Math.E, 0);

                    Complex value = Complex.Pow(euler, (negOneI * Math.PI * 2 * (fi - 1) * timeVal.Value));
                    csw[timeVal.Key]=value;
                });

                // Compute dot product between signal and complex sine wave
                Complex dotProduct = new Complex(0, 0);
                object dotProductLock = new object();

                Parallel.For(0, (timeDomainLen - 1), (idx) => {
                    Complex dotProductLocal = new Complex(0, 0);
                    
                    dotProductLocal += ((csw[idx] * timeDomain[idx]));

                    lock (dotProductLock)
                    {
                        dotProduct += dotProductLocal;
                    }
                }

                );

                fCoefs.Add(dotProduct);
                csw.Clear();
            }

            FrequencyDomain result = new FrequencyDomain();
            result.FourierCoefficients = fCoefs;
            result.SampleRateHz = sampleRateHz;
            return result;
        }

        /// <summary>
        /// Simple pass through , callers typically parse magnitude/phase CSVs and pass in a list of magnitude and phases which we
        /// then assign to a new frequencyDomain object and return. 
        /// </summary>
        /// <param name="magPhaseList"></param>
        /// <returns></returns>
        public static FrequencyDomain FromMagnitudePhaseList(List<Tuple<double, double>> magPhaseList)
        {
            if (null == magPhaseList)
                return null;

            FrequencyDomain frequencyDomain = new FrequencyDomain();

            foreach (Tuple<double, double> magPhase in magPhaseList)
            {
                frequencyDomain.FourierCoefficients.Add(Complex.FromPolarCoordinates(magPhase.Item1, magPhase.Item2));
            }
            return frequencyDomain;
        }

        public static List<Tuple<double, double>>ToMagnitudePhaseList(FrequencyDomain frequencyDomain)
        {
            List<Tuple<double, double>> magPhaseList = new List<Tuple<double, double>>();

            foreach (Complex coefficient in frequencyDomain.FourierCoefficients)
            {
                magPhaseList.Add(new Tuple<double, double>(coefficient.Magnitude, coefficient.Phase));
            }

            return magPhaseList;
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
