using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessor.Interfaces
{
    public enum PASSTYPE
    {
        NONE = 0,
        LOW,
        HIGH
    };

    public enum FILTERTYPE
    {
        CUSTOM = 0,
        WINDOWEDSYNC
    }

    /// <summary>
    /// Represents frequency domain scaling vector filters
    /// </summary>
    public interface IDFTFilter
    {
        IList<Complex> FrequencyResponse();

        IList<double> ImpulseResponse(bool normalize);
    }
}
