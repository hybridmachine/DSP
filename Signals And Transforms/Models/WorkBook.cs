using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace SignalsAndTransforms.Models
{
    // A collection of signals, filters, and convolution kernels being actively worked on. Also contains the code
    // to save/load from disk. WorkBooks are managed by the WorkBookManager
    public class WorkBook : INotifyPropertyChanged
    {
        private Dictionary<string, Signal> m_signals;

        public WorkBook()
        {
            // Default constructor used by Dapper, which loads the name property by mapping.
            m_signals = new Dictionary<string, Signal>();
        }

        public WorkBook(String name)
        {
            Name = name;
            m_signals = new Dictionary<string, Signal>();
        }
        public long Id { get; set; }
        public String Name { get; set; }
        public String FilePath { get; set; }
        public String Notes { get; set; }

        public Dictionary<string, Signal> Signals
        {
            get { return m_signals; }
        }

        public Signal SourceSignal
        {
            get
            {
                Signal signal = null;
                if (m_signals.ContainsKey("Source"))
                {
                    signal = m_signals["Source"];
                }
                return signal;
            }

            set 
            {
                m_signals["Source"] = value;
            }
        }
        public Signal OutputSignal {
            get
            {
                Signal signal = null;
                if (m_signals.ContainsKey("Output"))
                {
                    signal = m_signals["Output"];
                }
                return signal;
            }

            set
            {
                m_signals["Output"] = value;
            }
        }
        public Signal ConvolutionKernel {
            get
            {
                Signal signal = null;
                if (m_signals.ContainsKey("ConvolutionKernel"))
                {
                    signal = m_signals["ConvolutionKernel"];
                }
                return signal;
            }

            set
            {
                m_signals["ConvolutionKernel"] = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
