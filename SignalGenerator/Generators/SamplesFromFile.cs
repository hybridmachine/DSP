using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SignalGenerator.Generators
{
    public class SamplesFromFile : ISignalGenerator
    {
        private String _dataFilePath;

        /// <summary>
        /// Source data from a file. The format is
        /// # comment lines are ignored (must start with a #) and comments must be on their own line
        /// Amplitude (float value)
        /// ....
        /// EOF
        /// 
        /// One line per reading, carriage return line feed or simply line feed (Windows or UNIX style)
        /// </summary>
        /// <param name="sourceFilePath"></param>
        public SamplesFromFile(string sourceFilePath)
        {
            // Throw whatever exception we get here, 
            // Caller should try/catch when instantiating.
            _dataFilePath = sourceFilePath;
        }

        /// <summary>
        /// Read the signals from the file, sampleCount is max here, if we hit EOF, you only get samples up to EOF
        /// Cycle count is how many times you want the sample repeated, 1 is just whats in the file, more than that and the sampled signal is sent back repeated
        /// </summary>
        /// <param name="sampleCount"></param>
        /// <param name="cycles"></param>
        /// <returns></returns>
        public List<double> GetSignal(int sampleCount, int cycles)
        {
            TextReader dataFile = new StreamReader(_dataFilePath);
            List<double> signals = new List<double>();
            string line;
            while (((line = dataFile.ReadLine()) != null) && (sampleCount > 0))
            {
                // Ignore comment lines
                if (line.Contains('#')) continue;

                // Throw the exception if it occurs.
                signals.Add(Double.Parse(line));
                sampleCount--;
            }

            while (cycles > 1)
            {
                signals.AddRange(signals);
                cycles--;
            }

            dataFile.Close();
            
            return signals;
        }
    }
}
