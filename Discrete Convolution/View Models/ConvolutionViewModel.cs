using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace Discrete_Convolution.View_Models
{
    // View model for main window, loads signals and filters, applies convolutions, outputs results
    public class ConvolutionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

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
        private FileStream _signalFile;
        public String SignalFile {
            get
            {
                return _signalFile?.Name;
            }

            set
            {
                try
                {
                    if (null != _signalFile)
                    {
                        _signalFile.Close();
                    }
                    _signalFile = new FileStream(value, FileMode.Open);
                    OnPropertyChanged(nameof(SignalFile));
                } catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }
        }

        private FileStream _filterFile;
        public String FilterFile {
            get
            {
                return _filterFile?.Name;
            }

            set
            {
                try
                {
                    if (null != _filterFile)
                    {
                        _filterFile.Close();
                    }
                    _filterFile = new FileStream(value, FileMode.Open);
                    OnPropertyChanged(nameof(FilterFile));
                } catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
            }
        }
    }
}
