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
            LoadTestData();
        }

        private void ActiveWorkBookChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            PlotData();
        }

        private void LoadTestData()
        {
            string filterKernelPath = Path.Combine(Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["datadir"]), @"Filter Kernels\High Pass.csv");
            // For now load test convolution data, todo load from disk
            Signal convolutionKernel = new Signal();
            convolutionKernel.Name = "ConvolutionKernel";
            convolutionKernel.SampleSeconds = 1;
            convolutionKernel.SamplingHZ = 32;
            convolutionKernel.Type = SignalType.ConvolutionKernel;

            try
            { 
                // Low pass filter kernel
                using (StreamReader file = new StreamReader(filterKernelPath))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        double value;
                        if (double.TryParse(line, out value))
                        {
                            convolutionKernel.Samples.Add(value);
                        }
                    }
                }
            } catch (Exception ex)
            {
                // TODO log the exception, perhaps notify in the UI
            }

            manager.ActiveWorkBook().ConvolutionKernel = convolutionKernel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IList<DataPoint> SignalPlotPoints { get; private set; }

        public IList<DataPoint> ConvolutionPlotPoints { get; private set; }

        public IList<DataPoint> ResultPlotPoints { get; private set; }

        public FrequencyHistogramViewModel ResultFrequencyHistogram { get; private set; }


        public void PlotData()
        {
            
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
