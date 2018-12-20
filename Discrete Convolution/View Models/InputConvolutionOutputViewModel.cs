using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Discrete_Convolution.View_Models
{
    public class InputConvolutionOutputViewModel : INotifyPropertyChanged
    {
        public PlotModel InputModel { get; private set; }
        public PlotModel ConvolutionModel { get; private set; }
        public PlotModel OutputModel { get; private set; }

        public InputConvolutionOutputViewModel()
        {
            LoadInitialInputModel();
            LoadInitialConvolutionModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void LoadInitialInputModel()
        {
            InputModel = new PlotModel { Title = "Input Signal" };
            var linearBarSeries1 = new ScatterSeries { MarkerType = MarkerType.Circle };
            linearBarSeries1.Title = "Signal";

            
            linearBarSeries1.Points.Add(new ScatterPoint(1, 1));
            linearBarSeries1.Points.Add(new ScatterPoint(2, 2));
            linearBarSeries1.Points.Add(new ScatterPoint(3, 3));
            linearBarSeries1.Points.Add(new ScatterPoint(4, 4));

            InputModel.Series.Add(linearBarSeries1);

            NotifyPropertyChanged("InputModel");
        }

        private void LoadInitialConvolutionModel()
        {
            ConvolutionModel = new PlotModel { Title = "Input Signal" };
            var linearBarSeries1 = new ScatterSeries { MarkerType = MarkerType.Circle };
            linearBarSeries1.Title = "Signal";


            linearBarSeries1.Points.Add(new ScatterPoint(1, 1));
            linearBarSeries1.Points.Add(new ScatterPoint(2, 2));
            linearBarSeries1.Points.Add(new ScatterPoint(3, 3));
            linearBarSeries1.Points.Add(new ScatterPoint(4, 4));

            ConvolutionModel.Series.Add(linearBarSeries1);

            NotifyPropertyChanged("ConvolutionModel");
        }

        private void LoadInitialOutputModel()
        {

        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
