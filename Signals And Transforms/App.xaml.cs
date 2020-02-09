using SignalsAndTransforms.Managers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Serilog;
using System.Windows.Threading;
using SignalsAndTransforms;
using System.IO;

namespace SignalsAndTransforms
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string logParentPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string logPath = Path.Combine(logParentPath, ConfigurationManager.AppSettings["relativeLogPath"]);
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
               .CreateLogger();

            DispatcherUnhandledException += Application_DispatcherUnhandledException;

            MainWindow window = new MainWindow();

            if (e.Args.Length == 1)
            {
                WorkBookManager.Manager().Load(e.Args[0], true);
            }
            window.Show();
        }

        void Application_Exit(object sender, ExitEventArgs e)
        {
            Log.Information($"Signals And Transforms shutting down with exit code {e.ApplicationExitCode}");
        }

        void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, e.Exception.Message);

            MessageBoxResult result = MessageBox.Show($"{SignalsAndTransforms.Properties.Resources.FATAL_ERROR_MESSAGE} {e.Exception.Message}",
                                          SignalsAndTransforms.Properties.Resources.FATAL_ERROR_MESSAGE_TITLE,
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Error);

            e.Handled = false; // Let the app terminate
        }
    }
}
