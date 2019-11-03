using Signals_And_Transforms.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals_And_Transforms.View_Models
{
    public class SignalGeneratorViewModel
    {
        public ObservableCollection<Signal> Signals { get; private set; }

        public SignalGeneratorViewModel()
        {
            Signals = new ObservableCollection<Signal>();
        }
    }
}
