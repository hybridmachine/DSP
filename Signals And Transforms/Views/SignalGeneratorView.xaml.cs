﻿using Signals_And_Transforms.Models;
using Signals_And_Transforms.View_Models;
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

namespace Signals_And_Transforms.Views
{
    /// <summary>
    /// Interaction logic for SignalGenerator.xaml
    /// </summary>
    public partial class SignalGeneratorView : UserControl
    {
        public SignalGeneratorView()
        {
            InitializeComponent();
        }

        private void AddSignal_Click(object sender, RoutedEventArgs e)
        {            
            SignalGeneratorViewModel model = DataContext as SignalGeneratorViewModel;

            if (model != null)
            {
                try
                {
                    Signal newSignal = new Signal();
                    newSignal.Name = SignalName.Text;
                    newSignal.SamplingHZ = double.Parse(SamplingHz.Text);
                    newSignal.SignalHZ = double.Parse(SignalHz.Text);
                    newSignal.Amplitude = double.Parse(SignalAmplitude.Text);
                    newSignal.SampleSeconds = 2.0;
                    newSignal.TypeOfSignal = SignalType.Sinusoid;
                    model.Signals.Add(newSignal);
                }
                catch (Exception ex)
                {
                    // Todo show to user
                }
                ClearValues();

                model.PlotSignals();
            }
        }

        private void ClearValues()
        {
            SignalName.Text = String.Empty;
            SignalHz.Text = String.Empty;
            SignalAmplitude.Text = "1.0";
            SignalName.Focus();
        }

        private void SignalsList_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                SignalGeneratorViewModel model = DataContext as SignalGeneratorViewModel;
                if (model == null)
                {
                    return;
                }

                var selectedItems = SignalsList.SelectedItems;
                List<Signal> copyOfSelectedItems = new List<Signal>(selectedItems.Count);

                foreach (Signal item in selectedItems)
                {
                    copyOfSelectedItems.Add(item);
                }

                foreach (Signal item in copyOfSelectedItems)
                {
                    model.Signals.Remove(item);
                }

                model.PlotSignals();
            }
        }
    }
}
