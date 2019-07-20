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
using System.Windows.Threading;
using Signals_And_Transforms.Models;
using OxyPlot.Axes;

namespace Signals_And_Transforms.View_Models
{
    public class HistogramViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        FrequencyDomain _frequencyDomain;
        
        public PlotModel MyModel { get; private set; }

        Sample _signalSample;

        public HistogramViewModel()
        {
            _signalSample = SampleData.SignalSample;
            GetNewModel();
            SampleData.NewSlice += HandleNewSlice;
        }

        private void GetNewModel()
        {
         
            _frequencyDomain = SampleData.FreqDomain;

            MyModel = new PlotModel { Title = "Frequency Histogram" };
            var linearAxis1 = new LinearAxis();
            linearAxis1.Position = AxisPosition.Bottom;
            MyModel.Axes.Add(linearAxis1);
            var linearAxis2 = new LinearAxis();
            MyModel.Axes.Add(linearAxis2);
            var linearBarSeries1 = new LinearBarSeries();
            linearBarSeries1.Title = "Hertz";

            for (int idx = 0; idx < _frequencyDomain.RealComponent.Count; idx++)
            {
                int hz = (int)Math.Floor((0.5) * ((float)idx / (float)(_frequencyDomain.RealComponent.Count - 1)) * SampleData.SignalSample.SampleRate);
                linearBarSeries1.Points.Add(new DataPoint(hz, _frequencyDomain.Magnitude(idx)));
            }
            
            MyModel.Series.Add(linearBarSeries1);

            MyModel.TitleFontSize = 12;
            NotifyPropertyChanged("MyModel");
        }

        public void HandleNewSlice(object sender, EventArgs e)
        {
            GetNewModel();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
