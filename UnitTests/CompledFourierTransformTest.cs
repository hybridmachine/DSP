using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalProcessor;
namespace UnitTests
{
    [TestClass]
    public class CompledFourierTransformTest
    {
        [TestMethod]
        public void TestComplexTransform()
        {
            FastFourierTransform fourierTransform = new FastFourierTransform();
            FrequencyDomain result;
            FrequencyDomain realFormatResult;
            List<double> recreatedSignal;
            List<double> recreatedSignalFromComplex;

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

            result = fourierTransform.ComplexTransform(signal);
            realFormatResult = fourierTransform.Transform(signal.GetRange(0,32));
            recreatedSignal = fourierTransform.Synthesize(realFormatResult);
            recreatedSignalFromComplex = fourierTransform.ComplexSynthesize(result.FourierCoefficients);

            Assert.IsNotNull(result);
            Assert.IsNotNull(realFormatResult);
            Assert.IsNotNull(recreatedSignal);
            Assert.IsNotNull(recreatedSignalFromComplex);
        }
    }
}
