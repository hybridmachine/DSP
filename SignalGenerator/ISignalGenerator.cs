using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalGenerator
{
    public interface ISignalGenerator
    {
        List<double> GetSignal(int sampleCount, int cycles);
    }
}
