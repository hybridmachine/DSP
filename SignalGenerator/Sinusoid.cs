using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalGenerator
{
    public class Sinusoid
    {
        // Generate a simple sinusoid
        public static List<double> GetSignal(int sampleCount, int cycles)
        {
            List<double> samples = new List<double>(sampleCount);
            double theta;

            for (int sample = 0; sample < sampleCount; sample++)
            {
                theta = cycles * ((double)sample / (double)(sampleCount - 1)) * 2 * Math.PI;
                Console.Out.WriteLine("Sample " + sample + " radian " + theta);
                samples.Add(Math.Sin(theta));
            }
            return samples;
        }
    }
}
