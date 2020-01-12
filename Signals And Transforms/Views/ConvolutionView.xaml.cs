using OxyPlot.Wpf;
using SignalsAndTransforms.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SignalsAndTransforms.Views
{
    /// <summary>
    /// Interaction logic for ConvolutionView.xaml
    /// </summary>
    public partial class ConvolutionView : UserControl
    {
        public ConvolutionView()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool visibility = (bool)e.NewValue;
            if (visibility == true)
            {
                ConvolutionViewModel model = DataContext as ConvolutionViewModel;

                model?.PlotData();
            }
        }

        private void SignalPlotMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_ClickHandler(SignalPlotLineSeries, sender, e);
        }

        private void ConvolutionPlotMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_ClickHandler(ConvolutionPlotLineSeries, sender, e);
        }

        private void ResultPlotMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem_ClickHandler(ResultPlotLineSeries, sender, e); 
        }

        private void MenuItem_ClickHandler(LineSeries lineSeries, object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.MenuItem menuItem = sender as System.Windows.Controls.MenuItem;

            if (menuItem != null)
            {
                if (menuItem.Header.ToString() == Properties.Resources.CTXT_PLOT_MARKER_TYPE_NONE)
                {
                    lineSeries.MarkerType = OxyPlot.MarkerType.None;
                }
                else if (menuItem.Header.ToString() == Properties.Resources.CTXT_PLOT_MARKER_TYPE_SQUARE)
                {
                    lineSeries.MarkerType = OxyPlot.MarkerType.Square;
                }
                else if (menuItem.Header.ToString() == Properties.Resources.CTXT_PLOT_MARKER_TYPE_TRIANGLE)
                {
                    lineSeries.MarkerType = OxyPlot.MarkerType.Triangle;
                }
                
            }
        }
    }
}
