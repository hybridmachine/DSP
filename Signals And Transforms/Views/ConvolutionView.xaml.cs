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
    }
}
