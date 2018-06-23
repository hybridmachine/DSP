using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalGenerator.Generators
{
    public class WhiteNoise : ISignalGenerator
    {
        /// <summary>
        /// Get a Random Signal
        /// </summary>
        /// <param name="sampleCount">Number of samples to return</param>
        /// <param name="cycles">Unused for this signal generator</param>
        /// <returns></returns>
        public List<double> GetSignal(int sampleCount, int cycles)
        {
            List<double> samples = new List<double>(sampleCount);

            Random rand = new Random();
            for (int idx = 0; idx < sampleCount; idx++)
            {
                samples.Add(rand.NextDouble());
            }

            return samples;
        }
    }
}
