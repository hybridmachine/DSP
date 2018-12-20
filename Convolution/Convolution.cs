using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSP
{
    public enum ConvolutionType
    {
        INPUTSIDE,
        OUTPUTSIDE
    }

    public class Convolution
    {
        // This algorithm is derived from Table 6-1 of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        // http://www.dspguide.com/ch6/3.htm
        public List<double> Convolve(List<double> kernel, List<double> input, ConvolutionType type)
        {
            if (type == ConvolutionType.INPUTSIDE)
            {
                return InputSideConvolution(kernel, input);
            }
            else
            {
                return OuputSideConvolution(kernel, input);
            }
        }

        // This algorithm is derived from Table Chapter 7 section on Correlation of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        // http://www.dspguide.com/ch7/3.htm
        public List<Double> Correlate(List<double> target, List<double> signal)
        {
            // Correlation is performed by flipping either the target or signal and then running convolution
            List<double> invertedTarget = new List<double>(target);
            invertedTarget.Reverse();
            return InputSideConvolution(invertedTarget, signal);
        }

        // This algorithm is derived from Table 6-1 of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        // http://www.dspguide.com/ch6/3.htm
        private List<double> InputSideConvolution(List<double> kernel, List<double> input)
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

        // This algorithm is derived from Table 6-2 of ISBN 0-9660176-3-3 "The Scientist and Engineer's Guide to Digital Signal Processing"
        // http://www.dspguide.com/ch6/4.htm
        private List<double> OuputSideConvolution(List<double> kernel, List<double> input)
        {
            List<double> output = new List<double>(kernel.Count + input.Count - 1);

            for ( int I = 0; I < output.Capacity; I++)
            {
                output.Add(0.0);
                for (int J = 0; J < kernel.Count; J++)
                {
                    if ((I - J) < 0) continue;
                    if ((I - J) >= input.Count) continue;
                    output[I] = output[I] + kernel[J] * input[I - J];
                }
            }

            return output;
        }
    }
}
