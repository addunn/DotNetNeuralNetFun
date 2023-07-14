using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL;

namespace ML
{
    [Serializable]
    public struct Node
    {
        public double Bias;
        public double ActivationValue;
        public double Value;
        // public double[] Weights;
        public double[] Weights;
        public bool Enabled;

        // various storage for different optimizing techniques (probably don't need this many vars, should optimize this)
        public double[] WeightChangeLastValue;
        public double[] WeightAdamLastM;
        // special g is used in AdaGrad, RMSProp and Adam
        public double[] WeightSpecialG;

        public double BiasChangeLastValue;
        public double BiasAdamLastM;
        // special g is used in AdaGrad, RMSProp and Adam
        public double BiasSpecialG;
        
        /*
            MOMENTUM OF WEIGHT CHANGE:
	        - OLD: w = w + -η(der) 
	        - MOMENTUM: w = w + -η(der) + -αη(der[t-1])
	        - α = 0.9 (hyper param)
	        - need to store the der in the weight.
         */
        public Node(int weightCount, double weightStart, double weightEnd, int index) : this()
        {
            // bias starts at 0?
            Bias = 0;
            Weights = new double[weightCount];
            WeightChangeLastValue = new double[weightCount];
            WeightAdamLastM = new double[weightCount];
            WeightSpecialG = new double[weightCount];

            Enabled = true;

            for (int n = 0; n < weightCount; n++)
            {
                // initialize as random weight between weightStart and weightEnd
                double w = Rnd.GetRandomDouble(weightStart, weightEnd);
                Weights[n] = w;
            }
        }
    }
}
