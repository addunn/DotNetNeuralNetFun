using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML;
using CL;
using System.IO;

namespace MLPG
{
    public class Testing
    {

        public static TrainingItem[] LoadMNISTTrainingItems(string type = "training", int amount = -1)
        {

            string trainingItemsPath = Global.SharedPath + "my-raw-data-sets\\MNIST\\" + type + "\\";

            List<ML.TrainingItem> items = ML.Load.ClassifierTrainingSet(trainingItemsPath);

            items.Shuffle();

            Random rnd = new Random();

            TrainingItem[] result = items.ToArray();

            return result;
        }

        // creates 4 "project items" (neural networks) with different configurations....
        public static Project CreateTestProjectForMNIST()
        {
            Project result = new Project("MNIST", Solution.DataPath);

            TrainingItem[] trainingItems = LoadMNISTTrainingItems("training");
            TrainingItem[] testingItems = LoadMNISTTrainingItems("testing");

            result.trainingItems = trainingItems;

            result.testingItems = testingItems;

            List<ProjectItem> projectItems = new List<ProjectItem>();

            // increase this value if you're willing to wait longer for a trained network
            int amountOfProjectItems = 3;

            for (int n = 0; n < amountOfProjectItems; n++)
            {
                int[] hiddenLayers = new int[Rnd.GetRandomInteger(2, 4)];

                for (int h = 0; h < hiddenLayers.Length; h++)
                {
                    // first layer we want 75 nodes, all other layers between 35-55 nodes
                    hiddenLayers[h] = (h == 0 ? 75 : Rnd.GetRandomInteger(40, 60));
                }

                double weightStart = -1;
                double weightEnd = 1;

                IActivationFunction activationFunction = null;

                // what activation function do you want to use?
                activationFunction = new ActivationFunctions.Sigmoid();

                HyperParameters p = new HyperParameters();

                p.LearningRate = Rnd.GetRandomDouble(0.03, 0.07); // 0.05 is probably the ideal
                p.MaxBiasValue = 500;
                p.MaxWeightValue = 500;
                p.MinBiasValue = -500;
                p.MinWeightValue = -500;
                p.BatchSize = 200;
                p.PassThroughs = 4;
                p.DynamicDropoutLayerNodeCountDivisor = 300;
                p.RegularDropoutProbability = 0.09;

                p.MomentumAlpha = 0.9;
                p.AdaGradE = 0.01;
                p.RMSPropE = 0.01;
                p.AdamE = 0.01;
                p.RMSPropB = 0.9;
                p.AdamB = 0.9;
                p.AdamAlpha = 0.9;

                Features f = new Features();
                f.UseAdaGrad = true;
                f.UseRMSProp = false;
                f.UseMomentum = false;
                f.UseRegularDropOut = false;
                f.UseDynamicDropOut = false;
                f.UseAdam = true;

                ProjectItem projectItem = new ProjectItem(result.DataPath);

                projectItem.CurrentNetwork = ML.Testing.BuildUntrainedMNISTNetwork(hiddenLayers, weightStart, weightEnd, activationFunction, p, f);
                projectItem.Enabled = true;
                projectItems.Add(projectItem);

            }

            result.projectItems = projectItems;

            return result;

        }

        public static Project CreateTestProjectForXOR()
        {
            Project result = new Project("XOR", Solution.DataPath);

            List<TrainingItem> trainingItems = ML.Testing.BuildXORTrainingData(10000);
            List<TrainingItem> testingItems = ML.Testing.BuildXORTrainingData(2000);

            result.trainingItems = trainingItems.ToArray();

            result.testingItems = testingItems.ToArray();

            List<ProjectItem> projectItems = new List<ProjectItem>();



            int amountOfProjectItems = 15;

            for (int n = 0; n < amountOfProjectItems; n++)
            {
                int[] hiddenLayers = new int[Rnd.GetRandomInteger(4, 5)];

                for(int h = 0; h < hiddenLayers.Length; h++)
                {
                    hiddenLayers[h] = Rnd.GetRandomInteger(10, 40);
                }

                double weightStart = -1;
                double weightEnd = 1;


                IActivationFunction activationFunction = null;

                activationFunction = new ActivationFunctions.Sigmoid();

                HyperParameters p = new HyperParameters();
                p.LearningRate = Rnd.GetRandomDouble(0.00001, 0.9);
                p.MaxBiasValue = Rnd.GetRandomDouble(10, 500);
                p.MaxWeightValue = Rnd.GetRandomDouble(10, 500);
                p.MinBiasValue = Rnd.GetRandomDouble(-10, -500);
                p.MinWeightValue = Rnd.GetRandomDouble(-10, -500);
                p.BatchSize = 200;
                p.PassThroughs = 4;
                p.DynamicDropoutLayerNodeCountDivisor = 300;
                p.RegularDropoutProbability = 0.05;

                p.MomentumAlpha = 0.9;
                p.AdaGradE = 0.01;
                p.RMSPropE = 0.5;
                p.AdamE = 0.01;
                p.RMSPropB = 0.9;
                p.AdamB = 0.9;
                p.AdamAlpha = 0.9;


                Features f = new Features();
                f.UseAdaGrad = false;
                f.UseRMSProp = false;
                f.UseMomentum = false;
                f.UseRegularDropOut = Rnd.GetRandomInteger(1, 5) == 1;
                f.UseDynamicDropOut = false;
                f.UseAdam = true;

                ProjectItem projectItem = new ProjectItem(result.DataPath);

                projectItem.CurrentNetwork = ML.Testing.BuildUntrainedXORNetwork(hiddenLayers, weightStart, weightEnd, activationFunction, p, f);
                projectItem.Enabled = true;
                projectItems.Add(projectItem);

            }

            result.projectItems = projectItems;

            return result;

        }
    }
}
