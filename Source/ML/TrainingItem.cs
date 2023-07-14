using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    [Serializable]
    public struct TrainingItem
    {
        public double[] Input;

        public double[] ExpectedOutput;

        // ExpectedOutputIndex is for classifiers only
        public Int16 ExpectedOutputIndex;

        public TrainingItem(double[] input, double[] expectedOutput, Int16 expectedOutputIndex = -1) : this()
        {
            ExpectedOutput = expectedOutput;
            Input = input;
            ExpectedOutputIndex = expectedOutputIndex;
        }
    }
}
