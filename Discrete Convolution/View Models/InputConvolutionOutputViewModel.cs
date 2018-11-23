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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void LoadInitialInputModel()
        {
            InputModel = new PlotModel { Title = "Input Signal" };
            var linearAxis1 = new LinearAxis();
            linearAxis1.Position = AxisPosition.Bottom;
            InputModel.Axes.Add(linearAxis1);
            var linearAxis2 = new LinearAxis();
            InputModel.Axes.Add(linearAxis2);
            var linearBarSeries1 = new ScatterSeries();
            linearBarSeries1.Title = "Signal";

            InputModel.Series.Add(linearBarSeries1);

            NotifyPropertyChanged("InputModel");
        }

        private void LoadInitialConvolutionModel()
        {

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
