using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.View_Models
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string m_workbookTitle;

        public event PropertyChangedEventHandler PropertyChanged;

        public String WorkBookTitle
        {
            get
            {
                return m_workbookTitle;
            }
            set
            {
                m_workbookTitle = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WorkBookTitle)));
            }
        }
    }
}
