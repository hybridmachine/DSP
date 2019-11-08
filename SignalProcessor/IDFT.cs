using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessor
{
    /// <summary>
    /// Interface for DFT implementations (fast and correlation)
    /// </summary>
    public interface IDFT
    {
        FrequencyDomain Transform(List<double> signal, double sampleRateHz);
        List<double> Synthesize(FrequencyDomain frequencyDomain);
    }
}
