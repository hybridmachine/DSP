﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalProcessor;
namespace UnitTests
{
    [TestClass]
    public class ComplexFourierTransformTest
    {
        public const double MaxDifference = 0.0001;

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

            result = complexFourierTransform.Transform(signal);
            recreatedSignal = complexFourierTransform.Synthesize(result);

            Assert.IsNotNull(result);
            Assert.IsNotNull(recreatedSignal);

            for (int idx = 0; idx < recreatedSignal.Count; idx++)
            {
                Assert.IsTrue((Math.Abs(recreatedSignal[idx] - signal[idx]) < MaxDifference));
            }
        }
    }
}
