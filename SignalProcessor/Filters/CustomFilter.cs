using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SignalProcessor.Filters
{
    /// <summary>
    /// Class for custom filters with arbitrary magnitude/phase array for frequency repsonse.
    /// </summary>
    public class CustomFilter
    {
        public FrequencyDomain FrequencyResponse { get; set; }

        /// <summary>
        /// Create a CustomFilter from a list of magnitudes and phases
        /// </summary>
        /// <param name="magPhaseList"></param>
        public CustomFilter(List<Tuple<double, double>> magPhaseList)
        {
            FrequencyResponse = new FrequencyDomain();
            foreach (Tuple<double, double> magPhase in magPhaseList)
            {
                Complex coefficient = Complex.FromPolarCoordinates(magPhase.Item1, magPhase.Item2);
                FrequencyResponse.FourierCoefficients.Add(coefficient);
            }
        }

        public List<double> ImpulseResponse()
        {
            ComplexFastFourierTransform transform = new ComplexFastFourierTransform();
            return transform.Synthesize(FrequencyResponse);
        }
    }
}
