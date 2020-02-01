using SignalProcessor.Filters;
using SignalProcessor.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.Models
{
    /// <summary>
    /// Simple subclass of WindowedSyncFilter, just adds the IsActive flag used by the UI in workbooks
    /// </summary>
    public class WindowedSyncFilter : SignalProcessor.Filters.WindowedSyncFilter, INotifyPropertyChanged
    {
        public long Id { get; set; }

        private bool m_isActive;
        public bool IsActive { 
            get
            {
                return m_isActive;
            }
            set
            {
                m_isActive = value;
                NotifyPropertyChanged(nameof(IsActive));
            }
        }

        private string m_Name;
        public new string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        public new double CutoffFrequencySamplingFrequencyPercentage
        {
            get
            {
                return base.CutoffFrequencySamplingFrequencyPercentage;
            }
            set
            {
                base.CutoffFrequencySamplingFrequencyPercentage = value;
                NotifyPropertyChanged(nameof(CutoffFrequencySamplingFrequencyPercentage));
            }
        }

        public new int FilterLength
        {
            get
            {
                return base.FilterLength;
            }
            set
            {
                base.FilterLength = value;
                NotifyPropertyChanged(nameof(FilterLength));
            }
        }

        public new FILTERTYPE FilterType { 
            get
            {
                return base.FilterType;
            }
            set
            {
                base.FilterType = value;
                NotifyPropertyChanged(nameof(FilterType));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
