using System;
using System.Collections.Generic;
using System.Text;

namespace SignalProcessor
{
    public interface IWindowedSyncFilter
    {
        double CutoffFrequencySamplingFrequencyPercentage { get; set; }
        int FilterLength { get; set; }

        List<double> ImpulseResponse();
    }
}
