using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalProcessor;
using System.IO;
using System.Numerics;

namespace UnitTests
{
    [TestClass]
    public class ComplexFourierTransformTest
    {
        public const double MaxSignalDifference = 0.0001;
        public const double MaxAmplitudeDifference = 0.01;

        [TestMethod]
        public void TestComplexTransform()
        {
            IDFT complexFourierTransform = new ComplexFastFourierTransform();
            
            FrequencyDomain result;
            List<double> recreatedSignal;

            int sampleRate = 1000; // hz
            List<double> timePoints = new List<double>(2 * sampleRate);

            for (double timePointVal = 0; timePointVal < 2.0; timePointVal += (1.0 / sampleRate))
            {
                timePoints.Add(timePointVal);
            }

            List<double> signal = new List<double>(timePoints.Count);

            foreach (double timePointVal in timePoints)
            {
                double signalValue = (2.5 * Math.Sin(2 * Math.PI * 4 * timePointVal)) + (1.5 * Math.Sin(2 * Math.PI * 6.5 * timePointVal));
                signal.Add(signalValue);
            }

            result = complexFourierTransform.Transform(signal, sampleRate);
            List<Tuple<double, double>> magPhaseList = ComplexFastFourierTransform.ToMagnitudePhaseList(result);
            FrequencyDomain fromMagPhaseList = ComplexFastFourierTransform.FromMagnitudePhaseList(magPhaseList);

            for (int idx = 0; idx < result.FourierCoefficients.Count; idx++)
            {
                double absDifference = Complex.Abs(result.FourierCoefficients[idx] - fromMagPhaseList.FourierCoefficients[idx]);
                // Check that they are close, rounding error occurs, they wont be equal
                Assert.IsTrue(absDifference < 1.0E-10);
            }

            // This file can be viewed in Excel for plotting of hz and amplitudes
            StreamWriter amplitudeFile = new StreamWriter("frequencyAmplitudes.csv");
            if (amplitudeFile != null)
            {
                amplitudeFile.WriteLine("HZ, Amplitude");
                foreach (var frequencyAmplitude in result.FrequencyAmplitudes)
                {
                    amplitudeFile.WriteLine($"{frequencyAmplitude.Key}, {frequencyAmplitude.Value}");      
                }
            }
            amplitudeFile.Close();
            recreatedSignal = complexFourierTransform.Synthesize(result);

            Assert.IsNotNull(result);
            Assert.IsNotNull(recreatedSignal);

            double amplitude40 = result.FrequencyAmplitudes[4.0];
            double amplitude65 = result.FrequencyAmplitudes[6.5];

            Assert.IsTrue(Math.Abs(amplitude40 - 2.5) <= MaxAmplitudeDifference);
            Assert.IsTrue(Math.Abs(amplitude65 - 1.5) <= MaxAmplitudeDifference);

            for (int idx = 0; idx < recreatedSignal.Count; idx++)
            {
                Assert.IsTrue((Math.Abs(recreatedSignal[idx] - signal[idx]) <= MaxSignalDifference));
            }
        }

        [TestMethod]
        public void TestLoadFromFrequencyAmplitudes()
        {
            // low pass amplitude array
            double[] amplitudes = { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                                    1.0, 1.0, 1.0, 1.0, 1.0, 1.0,
                                    0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                    0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                    0.0, 0.0, 0.0, 0.0, 0.0, 0.0,
                                    0.0, 0.0};

            List<double> frequencyDomain = new List<double>(amplitudes);
            //FrequencyDomain frequencyDomain = ComplexFastFourierTransform.LoadFourierCoefficients(new List<double>(amplitudes));

            ComplexFastFourierTransform synth = new ComplexFastFourierTransform();
            FrequencyDomain timeDomain = synth.Transform(frequencyDomain, 2 * frequencyDomain.Count());

            foreach (var value in timeDomain.FrequencyAmplitudes)
            {
                Console.WriteLine(value);
            }
        }
    }
}
