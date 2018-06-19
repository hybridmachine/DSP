using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using OxyPlot;
using OxyPlot.Series;
using SignalGenerator;
using SignalProcessor;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Signals_And_Transforms.View_Models
{
    public class GraphViewModel : INotifyPropertyChanged
    {
        List<double> signal;
        FrequencyDomain frequencyDomain;
        List<double> synthesis;
        double _signalScale;
        int _sampleCount;
        int _cycleCount;

        public event PropertyChangedEventHandler PropertyChanged;

        public GraphViewModel()
        {
            _sampleCount = 128;
            _cycleCount = 5;
            _signalScale = 1.0;

            GetNewModel();
        }

        public PlotModel MyModel { get; private set; }
        public double SignalScale
        {
            get
            {
                return _signalScale;
            }
            set
            {
                _signalScale = value;
                if (_signalScale == 0.0)
                {
                    _signalScale = double.MinValue;
                }

                NotifyPropertyChanged();
                GetNewModel();
            }
        }

        private void GetNewModel()
        {
            signal = Sinusoid.GetSignal(_sampleCount, _cycleCount);
            frequencyDomain = DFT.Transform(signal);
            synthesis = DFT.Synthesize(frequencyDomain);

            this.MyModel = new PlotModel { Title = "Signal And Synthesis" };
            this.MyModel.Series.Add(new FunctionSeries(getSynthesis, 0, synthesis.Count - 1, 1.0, "Synthesis"));
            this.MyModel.Series.Add(new FunctionSeries(getSignal, 0, signal.Count - 1, 1.0, "Signal"));

            NotifyPropertyChanged("MyModel");
        }
        public int SampleCount {
            get
            {
                return _sampleCount;
            }
            set
            {
                if (value > 0)
                {
                    _sampleCount = value;
                    NotifyPropertyChanged();
                    GetNewModel();
                }
            }
        }

        public int CycleCount
        {
            get
            {
                return _cycleCount;
            }
            set
            {
                if (value > 0)
                {
                    _cycleCount = value;
                    NotifyPropertyChanged();
                    GetNewModel();
                }
            }
        }
        private double getSynthesis(double index)
        {
            int idx = (int)index;
            if (idx >= synthesis.Count || idx < 0)
            {
                return 0.0;
            }

            return synthesis[idx];
        }

        private double getSignal(double index)
        {
            int idx = (int)index;
            if (idx >= signal.Count || idx < 0)
            {
                return 0.0;
            }

            return signal[idx] * SignalScale;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
