using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.View_Models
{
    public class AboutViewModel
    {
        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string ApplicationInfo
        {
            get
            {
                return $"{Properties.Resources.APPLICATION_NAME} {Properties.Resources.COPYRIGHT_LABEL} {Properties.Resources.COPYRIGHT} {Properties.Resources.AUTHOR}";
            }
        }
    }
}
