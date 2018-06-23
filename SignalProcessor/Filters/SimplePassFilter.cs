using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalProcessor.Filters
{
    public class SimplePassFilter : IDFTFilter
    {
        private double _passPercent;
        private PASSTYPE _passType;

        public SimplePassFilter(double passPercent, PASSTYPE type)
        {
            _passPercent = passPercent;
            _passType = type;
        }
        public void ScaleFrequencies(List<double> scalingVector)
        {
            int clipIDX = (int)Math.Floor((double)scalingVector.Count * _passPercent);

            for (int idx = 0; idx < scalingVector.Count; idx++)
            {
                if (_passType == PASSTYPE.LOW)
                {
                    if (idx <= clipIDX)
                    {
                        scalingVector[idx] = 1.0;
                    }
                    else
                    {
                        scalingVector[idx] = 0.0;
                    }
                }
                else if (_passType == PASSTYPE.HIGH)
                {
                    if (idx >= clipIDX)
                    {
                        scalingVector[idx] = 1.0;
                    }
                    else
                    {
                        scalingVector[idx] = 0.0;
                    }
                }
            }
        }
    }
}
