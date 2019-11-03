using Signals_And_Transforms.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals_And_Transforms.Models
{
    public enum SignalType
    {
        Sinusoid,
        SquareWave,
        TriangleWave,
        Random
    };

    public class Signal
    {
        public SignalType TypeOfSignal;    
        public string Name;
        public double SamplingHZ;
        public double SignalHZ;
        public double SampleSeconds; // How long the signal is sampled for
        private List<Double> m_samples;
        public List<double> Samples
        {
            get
            {
                if (m_samples == null)
                {
                    m_samples = new List<double>((int)Math.Ceiling(SampleSeconds * SamplingHZ));
                }

                return m_samples;
            }
        }

        public override string ToString()
        {
            return String.Format(Resources.SAMPLE_DETAILS, Name, TypeOfSignal, SignalHZ, SamplingHZ, SampleSeconds);
        }
    }
}
