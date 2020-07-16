using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PediaStatDevice
{
    public class Range
    {
        public float LowerLimit
        { get; set; }

        public float UpperLimit
        {
            get;
            set;
        }

        public Range(float low, float high)
        {
            LowerLimit = low;
            UpperLimit = high;
        }
    }
}
