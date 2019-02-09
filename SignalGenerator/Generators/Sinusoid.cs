using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalGenerator.Generators
{
    public class Sinusoid : ISignalGenerator
    {
        // Generate a simple sinusoid
        public List<double> GetSignal(int sampleCount, int cycles)
        {
            List<double> samples = new List<double>(sampleCount);
            double theta;
            double amplitude;

            for (int sample = 0; sample < sampleCount; sample++)
            {
                theta = cycles * ((double)sample / (double)(sampleCount - 1)) * 2 * Math.PI;
                amplitude = Math.Sin(theta);
                Console.Out.WriteLine("Sample " + sample + " radian " + theta + " amplitude " + amplitude);
                samples.Add(amplitude);
            }
            return samples;
        }
    }
}
