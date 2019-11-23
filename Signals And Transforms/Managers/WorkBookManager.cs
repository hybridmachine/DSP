using SignalsAndTransforms.DAL;
using SignalsAndTransforms.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.Managers
{
    // Manages the active workbook (only one active at a time)
    public sealed class WorkBookManager : INotifyPropertyChanged
    {
        private WorkBook activeWorkbook;
        
        // Only one instance in the app
        private WorkBookManager()
        {
        
        }

        private static WorkBookManager Instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public static WorkBookManager Manager()
        {
            if (Instance == null)
            {
                Instance = new WorkBookManager();
            }
            return Instance;
        }

        public WorkBook ActiveWorkBook()
        {
            if (activeWorkbook == null)
            {
                activeWorkbook = new WorkBook();
            }

            return activeWorkbook;
        }

        // Load a workbook from the specified path
        public WorkBook Load(string filePath, bool makeActive = false)
        {
            WorkBook loadedWorkbook = WorkBookDAL.Load(filePath);

            // Callers should ensure active workbook is saved before loading
            if (makeActive)
            {
                activeWorkbook = loadedWorkbook;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ActiveWorkBook"));
            }
            return loadedWorkbook;
        }

        public bool SaveWorkBook(WorkBook workBook)
        {
            WorkBookDAL.Create(workBook);
            WorkBookDAL.Update(workBook);
            return true;
        }

        public bool Update(WorkBook workBook)
        {
            WorkBookDAL.Update(workBook);
            return true;
        }
    }
}
