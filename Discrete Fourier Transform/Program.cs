using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalGenerator;
using System.IO;

namespace Discrete_Fourier_Transform
{
    class Program
    {
        static void Main(string[] args)
        {
            List<double> signals = Sinusoid.GetSignal(32, 2);

            FileStream output = new FileStream("sinusoid.csv", FileMode.Create);

            if (output.CanWrite)
            {
                StreamWriter writer = new StreamWriter(output);
                int idx = 0;
                foreach (double signal in signals)
                {
                    writer.WriteLine(idx + "," + signal);
                    idx++;
                }
                writer.Close();
            }
            output.Close();
        }
    }
}
