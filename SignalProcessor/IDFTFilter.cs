using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessor
{
    public enum PASSTYPE
    {
        NONE = 0,
        LOW,
        HIGH
    };

    /// <summary>
    /// Represents frequency domain scaling vector filters
    /// </summary>
    public interface IDFTFilter
    {
        /// <summary>
        /// Take the scaling vector and apply the low/band/high pass filter
        /// Assumes frequency channels are low to high 0 -> Count in vector
        /// </summary>
        /// <param name="scalingVector"></param>
        void ScaleFrequencies(List<double> scalingVector);
    }
}
