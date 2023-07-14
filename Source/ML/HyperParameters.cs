using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    [Serializable]
    public class HyperParameters
    {
        public double LearningRate;
        public double MaxWeightValue;
        public double MinWeightValue;
        public double MaxBiasValue;
        public double MinBiasValue;

        // if dynamic dropout is enabled, the probabiltiy a node will be dropped in a layer is: layer node count / DynamicDropoutLayerNodeCountDivisor
        public double DynamicDropoutLayerNodeCountDivisor;
        public double RegularDropoutProbability;

        // (usually 0.9)
        public double MomentumAlpha;

        // (very small to prevent divide by 0)
        public double AdaGradE;
        public double RMSPropE;
        public double AdamE;

        // (usually 0.9) (the forgetting factor)
        public double RMSPropB;

        // (usually 0.9) (the forgetting factor)
        public double AdamB;

        // (usually 0.9)
        public double AdamAlpha;

        // BATCH SIZE AND PASS THROUGHS COULD MAKE IT SEEM THAT A NETWORK IS PERFORMING BETTER THAN REALITY

        public int BatchSize;

        // number of times to run the same batch through
        public int PassThroughs;

        public HyperParameters()
        {

        }

        public double ComputeAggressiveness()
        {
            double result = 0;
            // this could be revisted... 
            // the multiply is how much i think a nn changes when changing these values
            result += LearningRate * 10;
            result += MaxWeightValue;
            result += -MinWeightValue;
            result += MaxBiasValue;
            result += -MinBiasValue;

            return result;
        }
    }
}
