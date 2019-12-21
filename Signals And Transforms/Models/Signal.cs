using SignalsAndTransforms.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.Models
{
    public enum SignalType
    {
        Source,
        ConvolutionKernel,
        Output
    };

    public class Signal : INotifyPropertyChanged
    {
        public long Id { get; set; }
        private bool m_isActive;
        public bool IsActive { 
            get
            {
                return m_isActive;
            }
            set
            {
                m_isActive = value;
                NotifyPropertyChanged(nameof(IsActive));
            }
        }

        private SignalType m_Type;
        public SignalType Type {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
                NotifyPropertyChanged(nameof(Type));
            }
        }

        private string m_Name;
        public string Name {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        private double m_samplingHz;
        public double SamplingHZ {
            get
            {
                return m_samplingHz;
            }
            set
            {
                m_samplingHz = value;
                NotifyPropertyChanged(nameof(SamplingHZ));
            }
        }

        private double m_signalHz;
        public double SignalHZ {
            get
            {
                return m_signalHz;
            }
            set
            {
                m_signalHz = value;
                NotifyPropertyChanged(nameof(SignalHZ));
            }
        }

        private double m_sampleSeconds;
        public double SampleSeconds {
            get
            {
                return m_sampleSeconds;
            }
            set
            {
                m_sampleSeconds = value;
                NotifyPropertyChanged(nameof(SampleSeconds));
            }
        } // How long the signal is sampled for

        private double m_amplitude;
        public double Amplitude {
            get
            {
                return m_amplitude;
            }
            set
            {
                m_amplitude = value;
                NotifyPropertyChanged(nameof(Amplitude));
            }
        } 

        private List<Double> m_samples;

        public event PropertyChangedEventHandler PropertyChanged;

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

            set
            {
                m_samples = value; // Used by dapper when reading from DB
            }
        }

        public override string ToString()
        {
            return String.Format(Resources.SAMPLE_DETAILS, Name, Type, SignalHZ, SamplingHZ, SampleSeconds);
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
