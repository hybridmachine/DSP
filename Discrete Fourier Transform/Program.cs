using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalGenerator;
using SignalGenerator.Generators;
using SignalProcessor;
using System.IO;

namespace Discrete_Fourier_Transform
{
    class Program
    {
        static void Main(string[] args)
        {
            // ISignalGenerator sinusoid = new Sinusoid();
            // Sample sinusoidSamp = new Sample(1000, 1, 200, sinusoid);
            // List<double> signal = sinusoidSamp.GetNextSamplesForTimeSlice(64);
            // IDFT transform = new FastFourierTransform();
            // FrequencyDomain frequencyDomain = transform.Transform(signal);
            // List<double> synthesis = transform.Synthesize(frequencyDomain);
            
            try
            {
                StreamReader input = new StreamReader(args[0]);
                List<double> signal = new List<double>(64);

                while(input.Peek() > 0)
                {
                    double value;
                    if (Double.TryParse(input.ReadLine(), out value))
                    {
                        signal.Add(value);
                    }
                }
                IDFT transform = new RealFastFourierTransform();
                FrequencyDomain frequencyDomain = transform.Transform(signal);
                //List<double> synthesis = transform.Synthesize(frequencyDomain);
                FileStream output = new FileStream("FourierTransformResults.csv", FileMode.Create);

                if (output.CanWrite)
                {
                    StreamWriter writer = new StreamWriter(output);

                    // Write header row
                    writer.WriteLine("Index, Signal, Synthesis, Real, Imaginary");

                    for (int idx = 0; idx < signal.Count; idx++)
                    {
                        double sampleVal = signal[idx];
                        double synthVal = 0.0f;
                        writer.WriteLine(idx + "," + sampleVal + "," + synthVal + "," + frequencyDomain.RealComponent[idx] + "," + frequencyDomain.ImaginaryComponent[idx]);
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
