using DSP;
using OxyPlot;
using SignalProcessor;
using SignalsAndTransforms.Managers;
using SignalsAndTransforms.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace SignalsAndTransforms.View_Models
{
    public class ConvolutionViewModel : INotifyPropertyChanged
    {
        private WorkBookManager manager;
        private Convolution convolver; 

        public ConvolutionViewModel()
        {
            manager = WorkBookManager.Manager();
            manager.PropertyChanged += ActiveWorkBookChangedHandler;
            convolver = new Convolution();
        }

        private void ActiveWorkBookChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            PlotData();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IList<DataPoint> SignalPlotPoints { get; private set; }

        public IList<DataPoint> ConvolutionPlotPoints { get; private set; }

        public IList<DataPoint> ResultPlotPoints { get; private set; }

        public FrequencyHistogramViewModel ResultFrequencyHistogram { get; private set; }

        public void PlotData()
        {
            List<double> summedFilterData = manager.ActiveWorkBook().CombinedFilterImpulseResponse();

            if (null == summedFilterData)
                return;

            Signal workbookSourceSignal = manager.ActiveWorkBook().SumOfSources();
            if (workbookSourceSignal == null)
            {
                return;
            }

            SignalPlotPoints = new List<DataPoint>(workbookSourceSignal.Samples.Count);

            for (int idx = 0; idx < workbookSourceSignal.Samples.Count; idx++)
            {
                SignalPlotPoints.Add(new DataPoint(idx, workbookSourceSignal.Samples[idx]));
            }

            ConvolutionPlotPoints = new List<DataPoint>(summedFilterData.Count);

            for (int idx = 0; idx < summedFilterData.Count; idx++)
            {
                ConvolutionPlotPoints.Add(new DataPoint(idx, summedFilterData[idx]));
            }
            
            List<double> convolutionResult = convolver.Convolve(summedFilterData, workbookSourceSignal.Samples, ConvolutionType.INPUTSIDE);
            ResultPlotPoints = new List<DataPoint>(convolutionResult.Count);
            for (int idx = 0; idx < convolutionResult.Count; idx++)
            {
                ResultPlotPoints.Add(new DataPoint(idx, convolutionResult[idx]));
            }

            ComplexFastFourierTransform cmplxFFT = new ComplexFastFourierTransform();
            FrequencyDomain frequencyDomain = cmplxFFT.Transform(convolutionResult, workbookSourceSignal.SamplingHZ);
            ResultFrequencyHistogram = new FrequencyHistogramViewModel(frequencyDomain);
            
            NotifyPropertyChanged(nameof(SignalPlotPoints));
            NotifyPropertyChanged(nameof(ConvolutionPlotPoints));
            NotifyPropertyChanged(nameof(ResultPlotPoints));
            NotifyPropertyChanged(nameof(ResultFrequencyHistogram));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
