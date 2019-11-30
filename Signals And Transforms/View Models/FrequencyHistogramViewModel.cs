using SignalProcessor;
using System;
using OxyPlot;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace SignalsAndTransforms.View_Models
{
    public class FrequencyHistogramViewModel : PlotModel
    {
        public FrequencyHistogramViewModel(FrequencyDomain frequencyDomain)
        {
            Title = Properties.Resources.PLOT_FREQUENCY_HISTOGRAM_TITLE;
            Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = Properties.Resources.PLOT_FREQUENCY_HISTOGRAM_LEFT_AXIS });
            Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = Properties.Resources.PLOT_FREQUENCY_HISTOGRAM_BOTTOM_AXIS });
            
            if (frequencyDomain != null)
            {
                HistogramSeries chs = new HistogramSeries();
                chs.Items.AddRange(GetFrequencyBins(frequencyDomain));
                Series.Add(chs);
            }
        }

        private IList<HistogramItem> GetFrequencyBins(FrequencyDomain frequencyDomain)
        {
            int binCount = (int)Math.Ceiling(frequencyDomain.SampleRateHz / 2);
            List<HistogramItem> histogramItems = new List<HistogramItem>(binCount);

            foreach (var amplitude in frequencyDomain.FrequencyAmplitudes)
            {
                HistogramItem item = new HistogramItem(amplitude.Key - 0.25, amplitude.Key + 0.25, amplitude.Value, 1);
                histogramItems.Add(item);
            }

            return histogramItems;
        }
    }
}
