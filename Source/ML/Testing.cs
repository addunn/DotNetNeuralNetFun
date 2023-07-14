using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL;

namespace ML
{
    public class Testing
    {

        public static Network BuildUntrainedXORNetwork(int[] hiddenLayers, double weightStart, double weightEnd, IActivationFunction activationFunction, HyperParameters hyperParameters, Features features)
        {
            return new Network(5, new string[] { "0", "1" }, hiddenLayers, weightStart, weightEnd, activationFunction, hyperParameters, features);
        }
        
        public static Network BuildUntrainedMNISTNetwork(int[] hiddenLayers, double weightStart, double weightEnd, IActivationFunction activationFunction, HyperParameters hyperParameters, Features features)
        {
            return new Network(28 * 28, new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }, hiddenLayers, weightStart, weightEnd, activationFunction, hyperParameters, features);
        }

        public static List<TrainingItem> BuildXORTrainingData(int amount = 10000)
        {
            List<TrainingItem> trainingItems = new List<TrainingItem>();


            for (int n = 0; n < amount; n++)
            {
                double[] input = new double[2000];
                double[] expectedOutput = new double[2];

                Int16 expectedOutputIndex = -1;
                int rnd = (n % 4) + 1;

                input[0] = Rnd.GetRandomInteger(0, 255);
                input[2] = Rnd.GetRandomInteger(0, 255);

                for (int m = 4; m < input.Length; m++)
                {
                    input[m] = Rnd.GetRandomInteger(0, 255);
                }

                if (rnd == 1)
                {
                    expectedOutputIndex = 0;

                    input[1] = Rnd.GetRandomInteger(128, 255);
                    input[3] = Rnd.GetRandomInteger(128, 255);
                    expectedOutput[0] = 1;
                    expectedOutput[1] = 0;
                }
                else if (rnd == 2)
                {
                    expectedOutputIndex = 0;

                    input[1] = Rnd.GetRandomInteger(0, 127);
                    input[3] = Rnd.GetRandomInteger(0, 127);
                    expectedOutput[0] = 1;
                    expectedOutput[1] = 0;
                }
                else if (rnd == 3)
                {
                    expectedOutputIndex = 1;
                    input[1] = Rnd.GetRandomInteger(128, 255);
                    input[3] = Rnd.GetRandomInteger(0, 127);
                    expectedOutput[0] = 0;
                    expectedOutput[1] = 1;
                }
                else if (rnd == 4)
                {
                    expectedOutputIndex = 1;
                    input[1] = Rnd.GetRandomInteger(0, 127);
                    input[3] = Rnd.GetRandomInteger(128, 255);
                    expectedOutput[0] = 0;
                    expectedOutput[1] = 1;
                }

                // scales the input to 0-1... while keeping the spread the same
                Normalizer.ScaleArrayToRange(input, 0, 255, 0, 1);
                
                TrainingItem trainingItem = new TrainingItem(input, expectedOutput, expectedOutputIndex);
                trainingItems.Add(trainingItem);
            }

            trainingItems.Shuffle();

            return trainingItems;
        }
    }
}
