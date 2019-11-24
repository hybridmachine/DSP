using SignalsAndTransforms.Managers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SignalsAndTransforms
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow window = new MainWindow();
            if (e.Args.Length == 1)
            {
                WorkBookManager.Manager().Load(e.Args[0], true);
            }
            window.Show();
        }
    }
}
