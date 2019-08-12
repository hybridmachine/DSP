using Discrete_Convolution.View_Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Discrete_Convolution
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadSignal_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as ConvolutionViewModel;
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    dataContext.SignalFile = fileDialog.FileName;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }

        }

        private void LoadFilterKernel_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = DataContext as ConvolutionViewModel;
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    dataContext.FilterFile = fileDialog.FileName;
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }
        }
    }
}
