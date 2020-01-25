using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SignalProcessor.Filters;

namespace UnitTests
{
    [TestClass]
    public class CustomFilterTest
    {
        [TestMethod]
        [DeploymentItem(@"TestFiles\lowPassMagPhase.csv")]
        public void LoadCustomFilterFromMagPhaseTest()
        {
            List<Tuple<double, double>> magPhaseList = new List<Tuple<double, double>>();
            using (StreamReader fileReader = File.OpenText(@"lowPassMagPhase.csv"))
            {
                fileReader.ReadLine(); // Skip the header row

                while (!fileReader.EndOfStream)
                {
                    string[] magPhaseData = fileReader.ReadLine().Split(',');
                    double magnitude = double.Parse(magPhaseData[0]);
                    double phase = double.Parse(magPhaseData[1]);

                    magPhaseList.Add(new Tuple<double, double>(magnitude, phase));

                }    
            }

            CustomFilter customFilter = new CustomFilter(magPhaseList);

            IList<double> impulseResponse = customFilter.ImpulseResponse();

            Assert.IsNotNull(impulseResponse);
        }
    }
}
