using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DSP;

namespace TestConvolution
{
    class Program
    {
        static void Main(string[] args)
        {
            Convolution convolver = new Convolution();
            List<double> kernel = new List<double>(100);
            List<double> input = new List<double>(100);

            if (args.Count() < 2)
            {
                Console.WriteLine("usage: TestConvolution.exe <kernel file> <input file>");
                return;
            }

            FileStream kernelFile = File.OpenRead(args[0]);
            FileStream inputFile = File.OpenRead(args[1]);

            StreamReader kernelStream = new StreamReader(kernelFile);
            StreamReader inputStream = new StreamReader(inputFile);

            int kernelIdx = 0;
            while(kernelStream.Peek() > 0)
            {
                string line = kernelStream.ReadLine().Trim();

                //Skip comments
                if (line[0] == '#')
                {
                    continue;
                }

                kernel.Add(Double.Parse(line));
                kernelIdx++;
            }

            int inputIdx = 0;
            while (inputStream.Peek() > 0)
            {
                string line = inputStream.ReadLine().Trim();

                //Skip comments
                if (line[0] == '#')
                {
                    continue;
                }

                input.Add(Double.Parse(line));
                inputIdx++;
            }

            ConvolutionType type = ConvolutionType.INPUTSIDE;
            List<double> output = convolver.Convolve(kernel, input, type);

            Console.WriteLine("# Convolution Type: " + type + " signal:");

            foreach (double val in output)
            {
                Console.WriteLine(val);
            }
        }
    }
}
