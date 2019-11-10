using OxyPlot;
using SignalsAndTransforms.Managers;
using SignalsAndTransforms.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.View_Models
{
    public class ConvolutionViewModel : INotifyPropertyChanged
    {
        private WorkBookManager manager;

        public ConvolutionViewModel()
        {
            manager = WorkBookManager.Manager();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public IList<DataPoint> SignalPlotPoints { get; private set; }

        public IList<DataPoint> ConvolutionPlotPoints { get; private set; }

        public IList<DataPoint> ResultPlotPoints { get; private set; }

        public void PlotData()
        {
            SignalPlotPoints = new List<DataPoint>();
            Signal workbookSourceSignal = manager.ActiveWorkBook().SourceSignal;
            SignalPlotPoints = new List<DataPoint>(workbookSourceSignal.Samples.Count);

            for (int idx = 0; idx < workbookSourceSignal.Samples.Count; idx++)
            {
                SignalPlotPoints.Add(new DataPoint(idx, workbookSourceSignal.Samples[idx]));
            }

            NotifyPropertyChanged(nameof(SignalPlotPoints));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
