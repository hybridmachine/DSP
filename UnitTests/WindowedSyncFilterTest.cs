﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SignalProcessor;
using SignalProcessor.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class WindowedSyncFilterTest
    {
        [TestMethod]
        public void LowPassWindowedSyncTest()
        {
            IWindowedSyncFilter filter = new LowPassWindowedSync();
            IWindowedSyncFilter highPass = new HighPassWindowedSync();

            filter.CutoffFrequencySamplingFrequencyPercentage = 0.2;
            filter.FilterLength = 64;

            highPass.CutoffFrequencySamplingFrequencyPercentage = filter.CutoffFrequencySamplingFrequencyPercentage;
            highPass.FilterLength = filter.FilterLength;

            List<double> impulseResponse = filter.ImpulseResponse();
            List<double> highImpulseResponse = highPass.ImpulseResponse();

            Assert.IsNotNull(impulseResponse);
            
            Assert.IsTrue(impulseResponse.Count == filter.FilterLength + 1);
        }
    }
}