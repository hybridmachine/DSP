﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SignalProcessor;
using SignalProcessor.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalProcessor.Interfaces;

namespace UnitTests
{
    [TestClass]
    public class WindowedSyncFilterTest
    {
        [TestMethod]
        public void LowPassWindowedSyncTest()
        {
            IWindowedSyncFilter filter = new WindowedSyncFilter();
            IWindowedSyncFilter highPass = new WindowedSyncFilter() { FilterType = FILTERTYPE.HIGHPASS };
            
            filter.CutoffFrequencySamplingFrequencyPercentage = 20;
            filter.FilterLength = 64;

            highPass.CutoffFrequencySamplingFrequencyPercentage = filter.CutoffFrequencySamplingFrequencyPercentage;
            highPass.FilterLength = filter.FilterLength;

            IList<double> impulseResponse = filter.ImpulseResponse(true);
            IList<double> secondImpulseResponseTest = filter.ImpulseResponse(true);
            IList<double> non_normalizedResponseTest = filter.ImpulseResponse(false);

            // Make sure back to back calls return the same data for the same parameters
            for (int idx = 0; idx < impulseResponse.Count; idx++)
            {
                Assert.IsTrue(impulseResponse[idx] == secondImpulseResponseTest[idx]);
            }

            IList<double> highImpulseResponse = highPass.ImpulseResponse(true);

            Assert.IsNotNull(impulseResponse);
            Assert.IsNotNull(highImpulseResponse);
            Assert.IsNotNull(non_normalizedResponseTest);

            Assert.IsTrue(impulseResponse.Count == filter.FilterLength + 1);
        }
    }
}
