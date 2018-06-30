using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalGenerator.Generators
{
    public class SquareIshWave : ISignalGenerator
    {
        // Generate a close enough square wave
        public List<double> GetSignal(int sampleCount, int cycles)
        {
            List<double> samples = new List<double>(sampleCount);
            double theta;

            for (int sample = 0; sample < sampleCount; sample++)
            {
                theta = cycles * ((double)sample / (double)(sampleCount - 1)) * 2 * Math.PI;
                Console.Out.WriteLine("Sample " + sample + " radian " + theta);
                double value = 4.0 * Math.Sin(theta);
                if (value > 0.80)
                    value = 0.80;
                if (value < -0.80)
                    value = -0.80;

                samples.Add(value);
            }
           
            return samples;
        }
    }
}
