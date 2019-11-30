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
                var binningOptions = new BinningOptions(BinningOutlierMode.CountOutliers, BinningIntervalType.InclusiveLowerBound, BinningExtremeValueMode.ExcludeExtremeValues);
                var binBreaks = HistogramHelpers.CreateUniformBins(0, frequencyDomain.SampleRateHz / 2, (int)Math.Ceiling(frequencyDomain.SampleRateHz / 2));
                chs.Items.AddRange(HistogramHelpers.Collect(frequencyDomain.FrequencyAmplitudes.Values, binBreaks, binningOptions));
                Series.Add(chs);
            }
        }
    }
}
