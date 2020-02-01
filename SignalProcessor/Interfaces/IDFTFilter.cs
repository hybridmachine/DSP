using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessor.Interfaces
{
    public enum FILTERTYPE
    {
        LOWPASS,
        HIGHPASS,
        CUSTOM
    }

    /// <summary>
    /// Represents frequency domain scaling vector filters
    /// </summary>
    public interface IDFTFilter
    {
        FILTERTYPE FilterType { get; }

        IList<Complex> FrequencyResponse();

        IList<double> ImpulseResponse(bool normalize);
    }
}
