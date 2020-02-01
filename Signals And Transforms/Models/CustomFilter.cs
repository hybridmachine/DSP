﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalProcessor.Filters;

namespace SignalsAndTransforms.Models
{
    /// <summary>
    /// UI model for filter, the SignalProcessor filter types are delegates that do the actual work
    /// </summary>
    public class CustomFilter : SignalProcessor.Filters.CustomFilter, INotifyPropertyChanged
    {
        public long Id { get; set; }

        public CustomFilter(List<Tuple<double, double>> magPhaseList) : base(magPhaseList)
        {

        }

        private bool m_isActive;
        public bool IsActive
        {
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}