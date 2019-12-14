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
            IWindowedSyncFilter filter = new LowPassWindowedSync();
            filter.CutoffFrequencySamplingFrequencyPercentage = 0.2;
            filter.FilterLength = 64;

            List<double> impulseResponse = filter.ImpulseResponse();
            Assert.IsNotNull(impulseResponse);
            List<double> highFrequencyImpluseResponse = new List<double>(impulseResponse.Count);

            foreach (double value in impulseResponse)
            {
                highFrequencyImpluseResponse.Add(-value);
            }

            highFrequencyImpluseResponse[32] += 1;

            Assert.IsTrue(impulseResponse.Count == filter.FilterLength + 1);
        }
    }
}
