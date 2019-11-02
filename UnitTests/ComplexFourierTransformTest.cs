using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalProcessor;
using System.IO;

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
    }
}
