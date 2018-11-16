using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP
{
    public class Convolution
    {
        // This algorithm is derived from Table 6-1 of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        // http://www.dspguide.com/ch6/3.htm
        public List<double> Convolve(List<double> kernel, List<double> input)
        {
            List<double> output = new List<double>(kernel.Count + input.Count - 1);

            // Zero the output
            for (int idx = 0; idx < output.Capacity; idx++)
            {
                output.Add(0.0);
            }

            for (int I = 0; I < input.Count; I++)
            {
                for (int J = 0; J < kernel.Count; J++)
                {
                    output[I + J] = output[I + J] + input[I] * kernel[J];
                }
            }

            return output;
        }
    }
}
