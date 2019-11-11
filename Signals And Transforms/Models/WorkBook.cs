using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SignalsAndTransforms.Models
{
    // A collection of signals, filters, and convolution kernels being actively worked on. Also contains the code
    // to save/load from disk. WorkBooks are managed by the WorkBookManager
    public class WorkBook
    {
        public WorkBook(String name)
        {
            Name = name;
            Notes = String.Empty; // Needed by the DAL
        }

        public String Name { get; set; }
        public String FilePath { get; set; }
        public String Notes { get; set; }

        public Signal SourceSignal { get; set; }
        public Signal OutputSignal { get; set; }
        public Signal ConvolutionKernel { get; set; }
    }
}
