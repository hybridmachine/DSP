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
            LoadFilterImpulseResponse();
        }

        private void ActiveWorkBookChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            LoadFilterImpulseResponse();
            PlotData();
        }

        private void LoadFilterImpulseResponse()
        {
            if (manager.ActiveWorkBook().Filters.Count == 0)
            {
                return;
            }
            // For now load test convolution data, todo load from disk
            Signal convolutionKernel = new Signal();
            convolutionKernel.Name = "ConvolutionKernel";
            convolutionKernel.SampleSeconds = 1;
            convolutionKernel.Type = SignalType.ConvolutionKernel;

            List<double> summedImpulseResponse = null;
            foreach(var filter in manager.ActiveWorkBook().Filters.Values)
            {
                // For now we sum the filters
                // TODO add convolution as an option
                // We assume all filters have the same filter length
                // See https://www.dspguide.com/CH14.PDF#page=14&zoom=auto,-119,688 
                // Page 274 chapter 14 of "The Scientist and Engineer's Guide to Digital Signal Processing"
                if (null == summedImpulseResponse)
                {
                    summedImpulseResponse = filter.ImpulseResponse(); 
                }
                else
                {
                    List<double> filterImpulseResponse = filter.ImpulseResponse();

                    // Ignore any filters that don't have the same filter length
                    if (filterImpulseResponse.Count == summedImpulseResponse.Count)
                    {
                        for (int idx = 0; idx < summedImpulseResponse.Count; idx++)
                        {
                            summedImpulseResponse[idx] += filterImpulseResponse[idx];
                        }
                    }
                }
            }

            convolutionKernel.Samples = summedImpulseResponse;
            convolutionKernel.SamplingHZ = convolutionKernel.Samples.Count - 1;
            manager.ActiveWorkBook().ConvolutionKernel = convolutionKernel;

            /*
             * Code to read from disk, was used in testing, may be useful later so saving in a comment
            string filterKernelPath = Path.Combine(Environment.ExpandEnvironmentVariables(ConfigurationManager.AppSettings["datadir"]), @"Filter Kernels\Low Pass.csv");
             
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

                convolutionKernel.SamplingHZ = convolutionKernel.Samples.Count - 1;

            }
            catch (Exception ex)
            {
                // TODO log the exception, perhaps notify in the UI
            }

            */

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
