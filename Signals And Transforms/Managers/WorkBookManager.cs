using SignalsAndTransforms.DAL;
using SignalsAndTransforms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.Managers
{
    // Manages the active workbook (only one active at a time)
    public sealed class WorkBookManager
    {
        private WorkBook activeWorkbook;
        
        // Only one instance in the app
        private WorkBookManager()
        {
        
        }

        private static WorkBookManager Instance;
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
        public WorkBook Load(string filePath)
        {
            return WorkBookDAL.Load(filePath);
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
