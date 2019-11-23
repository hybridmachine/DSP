using SignalsAndTransforms.Managers;
using SignalsAndTransforms.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SignalsAndTransforms.Models;
using Microsoft.Win32;

namespace SignalsAndTransforms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SignalSetup.DataContext = new SignalGeneratorViewModel();
            ConvolutionView.DataContext = new ConvolutionViewModel();
            this.DataContext = new MainWindowViewModel();
        }

        private void TextBox_KeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tBox = (TextBox)sender;
                DependencyProperty prop = TextBox.TextProperty;

                BindingExpression binding = BindingOperations.GetBindingExpression(tBox, prop);
                if (binding != null) { binding.UpdateSource(); }
            }
        }

        private void HistogramView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_Up(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_Down(object sender, RoutedEventArgs e)
        {

        }

        private void BTN_Convolve_Click(object sender, RoutedEventArgs e)
        {
            SignalSetup.Visibility = Visibility.Collapsed;
            SignalSetup.IsEnabled = false;

            ConvolutionView.Visibility = Visibility.Visible;
            SignalSetup.IsEnabled = true;
        }

        private void BTN_FFT_Click(object sender, RoutedEventArgs e)
        {
            SignalSetup.Visibility = Visibility.Visible;
            SignalSetup.IsEnabled = true;

            ConvolutionView.Visibility = Visibility.Collapsed;
            ConvolutionView.IsEnabled = false;
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            WorkBook activeWorkBook = WorkBookManager.Manager().ActiveWorkBook();

            if (activeWorkBook.FilePath != null)
            {
                WorkBookManager.Manager().Update(WorkBookManager.Manager().ActiveWorkBook());
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = $"{Properties.Resources.DATABASE_FILES} (*{Properties.Resources.WORKBOOK_FILE_EXTENSION})|*{Properties.Resources.WORKBOOK_FILE_EXTENSION}|{Properties.Resources.ALL_FILES} (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    activeWorkBook.Name = saveFileDialog.SafeFileName;
                    activeWorkBook.FilePath = saveFileDialog.FileName;
                    WorkBookManager.Manager().SaveWorkBook(activeWorkBook);
                    SetActiveWorkbookTitle();
                }
            }
        }

        private void SetActiveWorkbookTitle()
        {
            if (this.DataContext != null)
            {
                MainWindowViewModel model = this.DataContext as MainWindowViewModel;
                model.WorkBookTitle = WorkBookManager.Manager().ActiveWorkBook().Name;
            }
        }

        private void MenuItemLoad_Click(object sender, RoutedEventArgs e)
        {
            // TODO alert if current workbook has unsaved changes
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = $"{Properties.Resources.DATABASE_FILES} (*{Properties.Resources.WORKBOOK_FILE_EXTENSION})|*{Properties.Resources.WORKBOOK_FILE_EXTENSION}|{Properties.Resources.ALL_FILES} (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                // Make active
                WorkBookManager.Manager().Load(openFileDialog.FileName, true);
                SetActiveWorkbookTitle();
            }            
        }

        /// <summary>
        /// Handle the user action of dropping a workbook file onto the app to open. If it 
        /// isn't a workbook file (ending in the WORKBOOK_FILE_EXTENSION) then it is ignored
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string file = files[0];
                // Make active
                if (file.EndsWith(Properties.Resources.WORKBOOK_FILE_EXTENSION))
                {
                    WorkBookManager.Manager().Load(file, true);
                    SetActiveWorkbookTitle();
                }
            }
        }
    }
}
