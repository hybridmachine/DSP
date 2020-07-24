using GSF.Media;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SignalProcessor;
using SignalProcessor.Interfaces;
using SignalsAndTransforms.Commands;
using SignalsAndTransforms.Managers;
using SignalsAndTransforms.Models;
using SignalsAndTransforms.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SignalsAndTransforms.View_Models
{
    public class SignalGeneratorViewModel : INotifyPropertyChanged
    {
        private WorkBookManager workBookManager;

        public ObservableCollection<SignalItemView> SignalViews { get; private set; }

        public ObservableCollection<Signal> Signals { get; private set; }

        public ICommand SaveWave { get; private set; }
        public ICommand PlayWave { get; private set; }

        private WaveFile SignalWaveFile { get; set; }

        public bool HaveWaves
        {
            get
            {
                return (SignalWaveFile != null);
            }
        }

        public void PlayTheWave(object unused)
        {
            SignalWaveFile.Play();
        }

        public void SaveTheWave(object unused)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = $"{Properties.Resources.WAV_FILES} (*{Properties.Resources.WAV_FILE_EXTENSITON})|*{Properties.Resources.WAV_FILE_EXTENSITON}|{Properties.Resources.ALL_FILES} (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                SignalWaveFile.Save(saveFileDialog.FileName);
            }
        }

        public void AddSignal(Signal signal)
        {
            workBookManager.ActiveWorkBook().Signals.Add(signal.Name, signal);
            LoadSignalsFromActiveWorkBook();
        }

        public void DeleteSignal(Signal signal)
        {
            signal.PropertyChanged -= handleSignalUpdate;

            workBookManager.ActiveWorkBook().Signals.Remove(signal.Name);
            LoadSignalsFromActiveWorkBook();
        }

        private void LoadSignalsFromActiveWorkBook()
        {
            Signals.Clear();
            SignalViews.Clear();

            List<Signal> signals = new List<Signal>(workBookManager.ActiveWorkBook().Signals.Values);
            // No addrange for observeablecollection
            foreach (Signal signal in signals.Where(sig => sig.Type == SignalType.Source))
            {
                signal.PropertyChanged -= handleSignalUpdate;
                Signals.Add(signal);
                SignalViews.Add(new SignalItemView { DataContext = signal });
                signal.PropertyChanged += handleSignalUpdate;
            }
        }

        public IList<DataPoint> PlotPoints { get; private set; }
        public PlotModel FrequencyViewModel { get; private set; }

        public String Title { get; private set; }

        public SignalGeneratorViewModel()
        {
            Signals = new ObservableCollection<Signal>();
            SignalViews = new ObservableCollection<SignalItemView>();

            Title = Properties.Resources.SIGNAL_PLOT_TITLE;
            workBookManager = WorkBookManager.Manager();
            workBookManager.PropertyChanged += ActiveWorkBookChangedHandler;
            CreateCommands();
        }

        private void ActiveWorkBookChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            LoadSignalsFromActiveWorkBook();
            PlotSignals();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void PlotSignals()
        {
            Signal workbookSourceSignal = workBookManager.ActiveWorkBook().SumOfSources();

            SignalWaveFile = null;
            PlotPoints = new List<DataPoint>(512);
            FrequencyViewModel = null;

            // Return empty set, this clears the display when all signals are off
            if (workbookSourceSignal == null)
            {
                NotifyPropertyChanged(nameof(PlotPoints));
                NotifyPropertyChanged(nameof(FrequencyViewModel));
                return;
            }

            CreateWavFile(workbookSourceSignal);

            for (int idx = 0; idx < workbookSourceSignal.Samples.Count; idx++)
            {
                PlotPoints.Add(new DataPoint(idx, workbookSourceSignal.Samples[idx]));
            }


            //Dictionary<double,double> frequencyAmplitudes = SpectralAnalyzer.AveragedFrequency(workbookSourceSignal.Samples, 512, workbookSourceSignal.SamplingHZ);
            IDFT fftTransform = new RealFastFourierTransform();

            //FrequencyDomain frequencyDomain = new FrequencyDomain();
            //frequencyDomain.FrequencyAmplitudes = frequencyAmplitudes;
             

            FrequencyViewModel = new FrequencyHistogramViewModel(fftTransform.Transform(workbookSourceSignal.Samples, workbookSourceSignal.SamplingHZ));

            NotifyPropertyChanged(nameof(PlotPoints));
            NotifyPropertyChanged(nameof(FrequencyViewModel));
        }

        /// <summary>
        /// Load the summed signal into a wav file, can be saved or played
        /// </summary>
        /// <param name="workbookSourceSignal"></param>
        private void CreateWavFile(Signal workbookSourceSignal)
        {
            int sampleRate = (int)workbookSourceSignal.SamplingHZ;

            SignalWaveFile = new WaveFile(sampleRate, 16, 1);

            // Convert amplitudes to 16 bit PCM values
            // First find max value, set that to be 100%
            // Scale all other values against that max, convert each value to 16 bit int. Biased to 32767 for Y zero point
            List<double> signalValues = workbookSourceSignal.Samples;

            double peakValue = signalValues.Max();
            if (Math.Abs(signalValues.Min()) > peakValue)
            {
                peakValue = Math.Abs(signalValues.Min());
            }

            foreach (double value in signalValues)
            {
                double percentage = value / peakValue;
                double pcmVal = (percentage * 32767) + 32767; // Bias at midpoint of 16 bit range

                SignalWaveFile.AddSample(pcmVal);
            }
        }

        public void handleSignalUpdate(object sender, PropertyChangedEventArgs args)
        {
            CommandManager.InvalidateRequerySuggested();

            PlotSignals();
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region commands
        private void CreateCommands()
        {
            SaveWave = new RelayCommand(param => HaveWaves, SaveTheWave);
            PlayWave = new RelayCommand(param => HaveWaves, PlayTheWave);
        }

        #endregion
    }
}
