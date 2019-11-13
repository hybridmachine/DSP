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

namespace SignalsAndTransforms.View_Models
{
    public class ConvolutionViewModel : INotifyPropertyChanged
    {
        private WorkBookManager manager;
        private Convolution convolver; 

        public ConvolutionViewModel()
        {
            manager = WorkBookManager.Manager();
            convolver = new Convolution();
            LoadTestData();
        }

        private void LoadTestData()
        {
            // For now load test convolution data, todo load from disk
            Signal convolutionKernel = new Signal();
            convolutionKernel.Name = "ConvolutonKernel";
            convolutionKernel.SampleSeconds = 1;
            convolutionKernel.SamplingHZ = 32;
            convolutionKernel.TypeOfSignal = SignalType.Sinusoid;
            // Low pass filter kernel
            convolutionKernel.Samples.AddRange(new double[] { 0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
-0.01,
-0.03,
-0.02,
0.02,
0.18,
0.28,
0.3,
0.28,
0.18,
0.02,
-0.02,
-0.03,
-0.01,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0,
0 });

            manager.ActiveWorkBook().ConvolutionKernel = convolutionKernel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IList<DataPoint> SignalPlotPoints { get; private set; }

        public IList<DataPoint> ConvolutionPlotPoints { get; private set; }

        public IList<DataPoint> ResultPlotPoints { get; private set; }

        public IList<DataPoint> ResultFrequencyHistogram { get; private set; }


        public void PlotData()
        {
            if (null == manager.ActiveWorkBook().SourceSignal)
            {
                return;
            }

            Signal workbookSourceSignal = manager.ActiveWorkBook().SourceSignal;
            SignalPlotPoints = new List<DataPoint>(workbookSourceSignal.Samples.Count);

            for (int idx = 0; idx < workbookSourceSignal.Samples.Count; idx++)
            {
                SignalPlotPoints.Add(new DataPoint(idx, workbookSourceSignal.Samples[idx]));
            }


            Signal convolutionKernel = manager.ActiveWorkBook().ConvolutionKernel;
            ConvolutionPlotPoints = new List<DataPoint>(convolutionKernel.Samples.Count);

            for (int idx = 0; idx < convolutionKernel.Samples.Count; idx++)
            {
                ConvolutionPlotPoints.Add(new DataPoint(idx, convolutionKernel.Samples[idx]));
            }
            
            List<double> convolutionResult = convolver.Convolve(convolutionKernel.Samples, workbookSourceSignal.Samples, ConvolutionType.INPUTSIDE);
            ResultPlotPoints = new List<DataPoint>(convolutionResult.Count);
            for (int idx = 0; idx < convolutionResult.Count; idx++)
            {
                ResultPlotPoints.Add(new DataPoint(idx, convolutionResult[idx]));
            }

            ComplexFastFourierTransform cmplxFFT = new ComplexFastFourierTransform();
            FrequencyDomain frequencyDomain = cmplxFFT.Transform(convolutionResult, workbookSourceSignal.SamplingHZ);
            ResultFrequencyHistogram = new List<DataPoint>(frequencyDomain.FrequencyAmplitudes.Count);
            foreach (var freq in frequencyDomain.FrequencyAmplitudes)
            {
                ResultFrequencyHistogram.Add(new DataPoint(freq.Key, freq.Value));
            }

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
