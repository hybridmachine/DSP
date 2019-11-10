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
            return activeWorkbook;
        }

        public WorkBook CreateWorkBook(String name)
        {
            WorkBook newWorkBook = new WorkBook(name);

            if (null == activeWorkbook)
            {
                activeWorkbook = newWorkBook;
            }

            return newWorkBook;
        }
    }
}
