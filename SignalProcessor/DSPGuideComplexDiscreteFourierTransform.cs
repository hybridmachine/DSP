using SignalProcessor.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessor
{
    public class DSPGuideComplexDiscreteFourierTransform : IDFT
    {
        /// <summary>
        /// Discrete Fourier Transform Synthesis
        /// Derived from Chapter 31 of the "The Scientist and Engineer's Guide to Digital Signal Processing
        /// https://www.dspguide.com/CH31.PDF
        /// </summary>
        /// <param name="frequencyDomain"></param>
        /// <returns>The real portion of the time domain signal</returns>
        public List<double> Synthesize(FrequencyDomain frequencyDomain)
        {
            if (frequencyDomain == null)
                return null;
   
            // Note index variables and arrays named to coincide with equation on page Chapter 31, page 578, 
            // Discrete Fourier Transform (DFT) synthesis
            int N = frequencyDomain.FourierCoefficients.Count;
            List<double> realTimeDomain = new List<double>(N);
            List<Complex> X = frequencyDomain.FourierCoefficients;
            Complex j = new Complex(0, 1);
            Complex exponentPart = j * Math.PI * 2;
            object realTimeDomainLock = new object();

            for (int n = 0; n <= (N - 1); n++)
            {
                realTimeDomain.Add(0.0);
            }

            Parallel.For(0, (N - 1), (n) =>
                {
                    Complex xn = new Complex(0, 0);
                    for (int k = 0; k <= (N - 1); k++)
                    {
                        double knOverN = (double)(k * n) / (double)N;
                        xn += X[k] * Complex.Pow(Math.E, exponentPart * knOverN);
                    }

                    lock (realTimeDomainLock)
                    {
                        realTimeDomain[n] = xn.Real;
                    }
                });

            return realTimeDomain;
        }

        /// <summary>
        /// Discrete Fourier Transform Analysis
        /// Derived from Chapter 31 of the "The Scientist and Engineer's Guide to Digital Signal Processing
        /// https://www.dspguide.com/CH31.PDF
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="sampleRateHz"></param>
        /// <returns></returns>
        public FrequencyDomain Transform(List<double> signal, double sampleRateHz)
        {
            if (signal == null)
                return null;

            // Note index variables and arrays named to coincide with equation on page Chapter 31, page 578, 
            // Discrete Fourier Transform (DFT) analysis
            List<double> x = signal;
            List<Complex> X = new List<Complex>(x.Count);
            int N = x.Count;
            Complex j = new Complex(0, 1);
            Complex exponentPart = -1 * j * Math.PI * 2;
            FrequencyDomain frequencyDomain = null;
            object xLock = new object();

            for (int k = 0; k <= (N - 1); k++)
            {
                X.Add(new Complex(0, 0));
            }
            
            // k runs over period 0 to N - 1
            Parallel.For(0, (N - 1), (k) =>
                {
                    Complex Xk = new Complex(0, 0);
                    //X[k] = 1/N * (Sum from 0 to N - 1 of x[n] * e ^ (-j * 2 * Pi * k * n / N
                    for (int n = 0; n <= (N - 1); n++)
                    {
                        double knOverN = (double)(k * n) / (double)N;
                        Xk += x[n] * Complex.Pow(Math.E, (exponentPart * knOverN));
                    }

                    lock (xLock)
                    {
                        X[k] = Xk / N; // Normalize X[k] (1/N multiplier)
                    }
                });

            frequencyDomain = new FrequencyDomain();
            frequencyDomain.FourierCoefficients = X;
            frequencyDomain.SampleRateHz = sampleRateHz;
            LoadFrequencyAmplitudes(frequencyDomain);
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
            List<double> vector = new List<double>(numPoints + 5);

            double spacing = (end - start) / (double)(numPoints - 1);

            for (double value = start; value <= end; value += spacing)
            {
                vector.Add(value);
            }
            return vector;
        }
    }
}
