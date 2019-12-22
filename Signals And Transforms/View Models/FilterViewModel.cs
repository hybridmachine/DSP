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
    public class FilterViewModel : INotifyPropertyChanged
    {
        private WorkBookManager manager;

        public FilterViewModel()
        {
            manager = WorkBookManager.Manager();
            manager.PropertyChanged += ActiveWorkBookChangedHandler;
            LoadConvolutionFilterData();
        }

        public IList<DataPoint> ImpulseResponsePoints { get; private set; }

        public IList<DataPoint> FrequencyResponsePoints { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ActiveWorkBookChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            LoadConvolutionFilterData();
        }

        private void LoadConvolutionFilterData()
        {
            Signal convolutionKernel = manager.ActiveWorkBook().ConvolutionKernel;
            ImpulseResponsePoints = new List<DataPoint>(convolutionKernel.Samples.Count);

            for (int idx = 0; idx < convolutionKernel.Samples.Count; idx++)
            {
                ImpulseResponsePoints.Add(new DataPoint(idx, convolutionKernel.Samples[idx]));
            }
            ComplexFastFourierTransform cmplxFFT = new ComplexFastFourierTransform();
            FrequencyDomain frequencyDomain = cmplxFFT.Transform(convolutionKernel.Samples, convolutionKernel.SamplingHZ);

            FrequencyResponsePoints = new List<DataPoint>(frequencyDomain.FrequencyAmplitudes.Count);

            List<double> values = new List<double>(frequencyDomain.FrequencyAmplitudes.Values);
            for (int idx = 0; idx < frequencyDomain.FrequencyAmplitudes.Count; idx++)
            {
                FrequencyResponsePoints.Add(new DataPoint(idx, values[idx]));
            }

            NotifyPropertyChanged(nameof(ImpulseResponsePoints));
            NotifyPropertyChanged(nameof(FrequencyResponsePoints));
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
