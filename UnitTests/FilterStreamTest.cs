using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSF.Media;

namespace UnitTests
{
    [TestClass]
    public class FilterStreamTest
    {
        /// <summary>
        /// Load a WAV file, read its data segment and using FFT convolution to filter the stream with a filter kernel
        /// </summary>
        [TestMethod]
        public void TestFFTConvolutionWavStream()
        {
            WaveDataReader wavReader = WaveDataReader.FromFile("ToneWithHighFrequency.wav");
            while(wavReader.WaveStream.Position < wavReader.WaveStream.Length)
            {

            }
        }

        /// <summary>
        /// Utility method to generate wav files
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sampleRate"></param>
        /// <param name="bitsPerSample"></param>
        /// <param name="samples"></param>
        public static bool CreateWavFile(string filePath, int sampleRate, short bitsPerSample, List<double>samples)
        {
            try
            {
                WaveFile waveFile = new WaveFile(sampleRate, bitsPerSample, 1);
                double[] sampleArray = new double[samples.Count];
                samples.CopyTo(sampleArray);

                waveFile.AddSamples(sampleArray);
                waveFile.Save(filePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
