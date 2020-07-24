using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using SignalProcessor.Interfaces;

namespace SignalProcessor.Filters
{
    /// <summary>
    /// Class for custom filters with arbitrary magnitude/phase array for frequency repsonse.
    /// </summary>
    public class CustomFilter : IDFTFilter
    {
        public FrequencyDomain FreqDomain { get; set; }

        public FILTERTYPE FilterType {
            get
            {
                return FILTERTYPE.CUSTOM;
            }
        }

        /// <summary>
        /// Create a CustomFilter from a list of magnitudes and phases
        /// </summary>
        /// <param name="magPhaseList"></param>
        public CustomFilter(List<Tuple<double, double>> magPhaseList)
        {
            FreqDomain = new FrequencyDomain();
            foreach (Tuple<double, double> magPhase in magPhaseList)
            {
                Complex coefficient = Complex.FromPolarCoordinates(magPhase.Item1, magPhase.Item2);
                FreqDomain.FourierCoefficients.Add(coefficient);
            }
        }
        
        public IList<Complex> FrequencyResponse()
        {
            return FreqDomain.FourierCoefficients;
        }

        public IList<double> ImpulseResponse(bool normalize = false)
        {
            ComplexCorrelationFourierTransform transform = new ComplexCorrelationFourierTransform();
            return transform.Synthesize(FreqDomain);
        }
    }
}
