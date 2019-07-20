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
using SignalProcessor.Filters;
using System.Windows.Threading;
using Signals_And_Transforms.Models;
using System.Windows.Input;

namespace Signals_And_Transforms.View_Models
{
    public class GraphViewModel : INotifyPropertyChanged
    {
        private DispatcherTimer _dispatchTimer;
        List<double> _signal;
        FrequencyDomain _frequencyDomain;
        List<double> _synthesis;
        double _signalScale;
        int _sampleCount;
        int _cycleCount;
        private string _filterType;
        int _framesPerSecond = 0;
        private int _filterClipPercentage;

        public event PropertyChangedEventHandler PropertyChanged;

        public GraphViewModel()
        {
            _sampleCount = 128;
            _cycleCount = 5;
            _signalScale = 1.0;
            _filterType = "None";
            _dispatchTimer = new DispatcherTimer();
            _signal = new List<double>(1000);
            _synthesis = new List<double>(1000);

            FramesPerSecond = 10;
            GetNewModel();

            _dispatchTimer.Tick += DispatchTimer_Tick;
            _dispatchTimer.Start();
        }

        private ICommand _incrementSynthesisOffset;
        public ICommand IncrementOffsetCmd
        {
            get
            {
                if (_incrementSynthesisOffset == null)
                {
                    _incrementSynthesisOffset = new RelayCommand(
                        p => this.CanIncrement,
                        p => this.IncrementOffset());
                }
                return _incrementSynthesisOffset;
            }
        }

        private ICommand _decrementSynthesisOffset;
        public ICommand DecrementOffsetCommand
        {
            get
            {
                if (_decrementSynthesisOffset == null)
                {
                    _decrementSynthesisOffset = new RelayCommand(
                        p => this.CanDecrement,
                        p => this.DecrementOffset());
                }
                return _decrementSynthesisOffset;
            }
        }
        bool CanIncrement
        {
            get
            {
                return true;
            }
        }
        bool CanDecrement
        {
            get
            { 
                return true;
            }
        }

        void IncrementOffset()
        {
            SynthesisOffset += 0.2;
            NotifyPropertyChanged();
        }

        void DecrementOffset()
        {
            SynthesisOffset -= 0.2;
            NotifyPropertyChanged();
        }

        private void DispatchTimer_Tick(object sender, EventArgs e)
        {
            GetNewModel();
        }

        public PlotModel MyModel { get; private set; }
        public double SynthesisOffset
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

        public int FramesPerSecond
        {
            get
            {
                return _framesPerSecond;
            }
            set
            {
                _framesPerSecond = value;
                if (_framesPerSecond <= 0)
                {
                    _dispatchTimer.Stop();
                }
                else
                { 
                    _dispatchTimer.Interval = new TimeSpan(0, 0, 0, 0, (1000 / _framesPerSecond));
                    if (!_dispatchTimer.IsEnabled)
                    {
                        _dispatchTimer.Start();
                    }
                }        
                NotifyPropertyChanged();
            }
        }

        private void GetNewModel()
        {

            switch (_filterType)
            {
                case "Low":
                    SampleData.SetFilter(PASSTYPE.LOW, _filterClipPercentage/100.0);
                    break;
                case "High":
                    SampleData.SetFilter(PASSTYPE.HIGH, _filterClipPercentage/100.0);
                    break;
                default:
                    SampleData.SetFilter(PASSTYPE.NONE, _filterClipPercentage/100.0);
                    break;
            }

            _synthesis.Clear();
            _signal.Clear();
            //IDFT transform = new CorrelationFourierTransform();
            IDFT transform = new FastFourierTransform();


            int sampleCount = 64;
            int padCount = 0;
            do
            {
                _signal.AddRange(SampleData.GetPaddedChannelSample(sampleCount, padCount));
                _frequencyDomain = SampleData.FreqDomain;

                _synthesis.AddRange(transform.Synthesize(_frequencyDomain).Take((sampleCount - padCount)));
            } while (_signal.Count < SampleData.SignalSample.SampleCount);

                this.MyModel = new PlotModel { Title = "Signal And Synthesis" };
            this.MyModel.Series.Add(new FunctionSeries(getSignal, 0, _signal.Count - 1, 1.0, "Signal"));
            this.MyModel.Series.Add(new FunctionSeries(getSynthesis, 0, _signal.Count - 1, 1.0, "Synthesis")); // Clip synthesis to signal sample count

            this.MyModel.TitleFontSize = 12;

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

        public int FilterClipPercentage
        {
            get
            {
                return _filterClipPercentage;
            }
            set
            {
                _filterClipPercentage = value > 0 ? value : 0;
                if (_filterClipPercentage > 100)
                    _filterClipPercentage = 100;

                NotifyPropertyChanged();
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

            return (_synthesis[idx] - SynthesisOffset);
        }

        private double getSignal(double index)
        {
            int idx = (int)index;
            if (idx >= _signal.Count || idx < 0)
            {
                return 0.0;
            }

            return _signal[idx];
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
