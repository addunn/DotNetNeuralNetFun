using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    public class Normalizer
    {
        public static void ScaleArrayToRange(double[] vals, double sourceMin, double sourceMax, double targetMin, double targetMax)
        {
            for(int n = 0; n < vals.Length; n++)
            {
                vals[n] = ((targetMax - targetMin) * ((vals[n] - sourceMin) / (sourceMax - sourceMin))) + (targetMin);
            }
        }
        public static double ScaleDoubleToRange(double val, double sourceMin, double sourceMax, double targetMin, double targetMax)
        {
            return ((targetMax - targetMin) * ((val - sourceMin) / (sourceMax - sourceMin))) + targetMin;
        }
    }
}
