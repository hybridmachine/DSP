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

        public WorkBook(String name)
        {
            Name = name;
            Notes = String.Empty; // Needed by the DAL
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
                return m_signals["Source"];
            }

            set 
            {
                m_signals["Source"] = value;
            }
        }
        public Signal OutputSignal {
            get
            {
                return m_signals["Output"];
            }

            set
            {
                m_signals["Output"] = value;
            }
        }
        public Signal ConvolutionKernel {
            get
            {
                return m_signals["ConvolutionKernel"];
            }

            set
            {
                m_signals["ConvolutionKernel"] = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
