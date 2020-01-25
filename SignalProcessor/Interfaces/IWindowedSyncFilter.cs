using System;
using System.Collections.Generic;
using System.Text;

namespace SignalProcessor.Interfaces
{
    public interface IWindowedSyncFilter : IDFTFilter
    {
        double CutoffFrequencySamplingFrequencyPercentage { get; set; }
        int FilterLength { get; set; }
    }
}
