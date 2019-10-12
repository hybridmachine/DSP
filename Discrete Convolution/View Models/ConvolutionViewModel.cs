using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using DSP;

namespace Discrete_Convolution.View_Models
{
    // View model for main window, loads signals and filters, applies convolutions, outputs results
    public class ConvolutionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ConvolutionViewModel()
        {
            SignalData = new List<double>();
            SignalData.Add(1.0);
            SignalData.Add(2.0);
            SignalData.Add(3.0);
            SignalData.Add(4.0);
            SignalData.Add(5.0);
        }

        public string ChartName
        {
            get
            {
                return "Test";
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

        /// <summary>
        /// After convolution, the output from that convolution
        /// </summary>
        public List<Double> ConvolutionOutput { get; private set; }

        /// <summary>
        /// User visible error messages, such as when loading a file fails
        /// </summary>
        private String _errorMessage;
        public String ErrorMessage {
            get
            {
                return _errorMessage;
            }
            private set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        private string _signalFilePath;
        private List<Double> _signalData;
        public List<Double> SignalData
        {
            get
            {
                return _signalData;
            }
            private set
            {
                _signalData = value;
                OnPropertyChanged(nameof(SignalData));
            }
        }


        /// <summary>
        /// Convenience for getting 1-n independant values for charting purposes
        /// </summary>
        public List<Double> SignalIndependantData
        {
            get
            {
                if (null == _signalData)
                    return new List<Double>(); // Send back an empty list

                List<Double> independantValues = new List<Double>(_signalData.Count());

                int idx = 1; 
                foreach (double unused in _signalData)
                {
                    independantValues.Add(idx);
                    idx++;
                }
                return independantValues;
            }
        }

        public String SignalFile
        {
            get
            {
                return _signalFilePath;
            }

            set
            {
                try
                {
                    _signalFilePath = value;
                    string[] fileData = File.ReadAllLines(_signalFilePath);
                    SignalData = new List<Double>(fileData.Count());

                    foreach (string fileLine in fileData)
                    {
                        SignalData.Add(Double.Parse(fileLine));
                    }

                    OnPropertyChanged(nameof(SignalData));
                    OnPropertyChanged(nameof(SignalIndependantData));
                    OnPropertyChanged(nameof(SignalFile));
                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }
        }

        private string _filterFilePath;
        public List<Double> FilterData { get; private set; }

        public String FilterFile {
            get
            {
                return _filterFilePath;
            }

            set
            {
                try
                {
                    _filterFilePath = value;
                    string[] fileData = File.ReadAllLines(_filterFilePath);
                    FilterData = new List<Double>(fileData.Count());

                    foreach (string fileLine in fileData)
                    {
                        FilterData.Add(Double.Parse(fileLine));
                    }
                    
                    OnPropertyChanged(nameof(FilterFile));
                } catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }
        }

        /// <summary>
        /// Read the data and kernel information from the files and perform the input side convolution
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Convolve()
        {
            if (FilterData == null || SignalData == null)
            {
                return false;
            }
            
            // May be CPU intensive so run on a background thread
            return await Task<bool>.Run(() => {
                Convolution convolver = new Convolution();
                ConvolutionOutput = convolver.Convolve(FilterData, SignalData, ConvolutionType.INPUTSIDE);
                return true;
            });
        }
    }
}
