using Microsoft.VisualStudio.TestTools.UnitTesting;
using SignalProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class SpectralAnalyzerTest
    {
        [TestMethod]
        public void TestSpectralAnalysis()
        {
            int sampleRate = 1000; // hz
            List<double> timePoints = new List<double>(2 * sampleRate);

            for (double timePointVal = 0; timePointVal < 2.0; timePointVal += (1.0 / sampleRate))
            {
                timePoints.Add(timePointVal);
            }

            List<double> signal = new List<double>(timePoints.Count);

            foreach (double timePointVal in timePoints)
            {
                double signalValue = (2.5 * Math.Sin(2 * Math.PI * 5 * timePointVal)) + (1.5 * Math.Sin(2 * Math.PI * 300 * timePointVal));
                signal.Add(signalValue);
            }

            List<double> spectrum = SpectralAnalyzer.AveragedFrequency(signal, 512, sampleRate);
            Assert.IsNotNull(spectrum);
        }
    }
}
