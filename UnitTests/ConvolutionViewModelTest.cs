using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Discrete_Convolution.View_Models;

namespace UnitTests
{
    [TestClass]
    public class ConvolutionViewModelTest
    {
        [TestMethod]
        [DeploymentItem(@"TestFiles\filterFile.csv")]
        [DeploymentItem(@"TestFiles\signalFile.csv")]
        public void TestConvolution()
        {
            ConvolutionViewModel testModel = new ConvolutionViewModel();

            testModel.FilterFile = @"filterFile.csv";
            testModel.SignalFile = @"signalFile.csv";

            bool convolveResult = testModel.Convolve().Result;
            Assert.IsTrue(convolveResult);
        }
    }
}
