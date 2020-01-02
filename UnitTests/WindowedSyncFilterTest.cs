using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            IWindowedSyncFilter filter = new WindowedSyncFilter();
            IWindowedSyncFilter highPass = new WindowedSyncFilter() { FilterType = FilterType.HIGHPASS };
            
            filter.CutoffFrequencySamplingFrequencyPercentage = 20;
            filter.FilterLength = 64;

            highPass.CutoffFrequencySamplingFrequencyPercentage = filter.CutoffFrequencySamplingFrequencyPercentage;
            highPass.FilterLength = filter.FilterLength;

            List<double> impulseResponse = filter.ImpulseResponse();
            List<double> secondImpulseResponseTest = filter.ImpulseResponse();

            // Make sure back to back calls return the same data for the same parameters
            for (int idx = 0; idx < impulseResponse.Count; idx++)
            {
                Assert.IsTrue(impulseResponse[idx] == secondImpulseResponseTest[idx]);
            }

            List<double> highImpulseResponse = highPass.ImpulseResponse();

            Assert.IsNotNull(impulseResponse);
            Assert.IsNotNull(highImpulseResponse);

            Assert.IsTrue(impulseResponse.Count == filter.FilterLength + 1);
        }
    }
}
