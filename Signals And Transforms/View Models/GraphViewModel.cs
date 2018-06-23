using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using SignalGenerator;
using SignalProcessor;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SampleGenerator;
using SignalGenerator.Generators;
using SignalProcessor.Filters;

namespace Signals_And_Transforms.View_Models
{
    public class GraphViewModel : INotifyPropertyChanged
    {
        List<double> _signal;
        FrequencyDomain _frequencyDomain;
        List<double> _synthesis;
        double _signalScale;
        int _sampleCount;
        int _cycleCount;
        private string _filterType;

        public event PropertyChangedEventHandler PropertyChanged;

        public GraphViewModel()
        {
            _sampleCount = 128;
            _cycleCount = 5;
            _signalScale = 1.0;
            _filterType = "None";
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
            ISignalGenerator sinusoid = new Sinusoid();
            ISignalGenerator random = new WhiteNoise();

            Sample sinusoidSamp = new Sample(8000, 1, 100, sinusoid);
            Sample sinusoidSamp2 = new Sample(8000, 1, 1000, sinusoid);
            Sample whiteNoise = new Sample(8000, 1, 1000, random);
            Sample summedSignal = sinusoidSamp.SumWithSample(whiteNoise);
            IDFTFilter filter = null;

            switch (_filterType)
            {
                case "Low":
                    filter = new SimplePassFilter(0.10, PASSTYPE.LOW);
                    break;
                case "High":
                    filter = new SimplePassFilter(0.10, PASSTYPE.HIGH);
                    break;
                default:
                    break;
            }

            _signal = summedSignal.GetNextSamplesForTimeSlice(100);
            _frequencyDomain = DFT.Transform(_signal);

            if (filter != null)
            {
                _frequencyDomain.ApplyFilter(filter);
            }

            _synthesis = DFT.Synthesize(_frequencyDomain);

            //ISignalGenerator sinusoid = new Sinusoid();
            //signal = sinusoid.GetSignal(_sampleCount, _cycleCount);
            //frequencyDomain = DFT.Transform(signal);
            //synthesis = DFT.Synthesize(frequencyDomain);

            this.MyModel = new PlotModel { Title = "Signal And Synthesis" };
            this.MyModel.Series.Add(new FunctionSeries(getSynthesis, 0, _synthesis.Count - 1, 1.0, "Synthesis"));
            this.MyModel.Series.Add(new FunctionSeries(getSignal, 0, _signal.Count - 1, 1.0, "Signal"));

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

        public String FilterType
        {
            get
            {
                return _filterType;
            }
            set
            {
                _filterType = value;
                GetNewModel();
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
            if (idx >= _synthesis.Count || idx < 0)
            {
                return 0.0;
            }

            return _synthesis[idx];
        }

        private double getSignal(double index)
        {
            int idx = (int)index;
            if (idx >= _signal.Count || idx < 0)
            {
                return 0.0;
            }

            return _signal[idx] * SignalScale;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
