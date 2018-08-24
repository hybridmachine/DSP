using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalGenerator;
using SignalGenerator.Generators;
using SignalProcessor;
using System.IO;
using SampleGenerator;

namespace Discrete_Fourier_Transform
{
    class Program
    {
        static void Main(string[] args)
        {
            ISignalGenerator sinusoid = new Sinusoid();
            Sample sinusoidSamp = new Sample(1000, 1, 200, sinusoid);
            List<double> signal = sinusoidSamp.GetNextSamplesForTimeSlice(10);
            IDFT transform = new CorrelationFourierTransform();
            FrequencyDomain frequencyDomain = transform.Transform(signal);
            List<double> synthesis = transform.Synthesize(frequencyDomain);

            try
            {
                FileStream output = new FileStream("sinusoid.csv", FileMode.Create);

                if (output.CanWrite)
                {
                    StreamWriter writer = new StreamWriter(output);

                    // Write header row
                    writer.WriteLine("Index, Signal, Synthesis");

                    for (int idx = 0; idx < signal.Count; idx++)
                    {
                        double sampleVal = signal[idx];
                        double synthVal = synthesis[idx];
                        writer.WriteLine(idx + "," + sampleVal + "," + synthVal);
                        idx++;
                    }

                    writer.Close();
                }
                output.Close();
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
