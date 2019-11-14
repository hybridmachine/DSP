using SignalsAndTransforms.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.Models
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
        public long Id { get; set; }
        public SignalType TypeOfSignal;    
        public string Name { get; set; }
        public double SamplingHZ { get; set; }
        public double SignalHZ { get; set; }
        public double SampleSeconds { get; set; } // How long the signal is sampled for
        public double Amplitude { get; set; } 
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
