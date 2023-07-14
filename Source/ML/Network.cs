using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL;

namespace ML
{
    [Serializable]
    public class Network
    {
        [Serializable]
        public struct Statistics
        {
            public Network Net;

            public long TotalLayers;
            public long TotalNodes;
            public long TotalHiddenNodes;
            public long TotalWeights;

            public double TotalActivationValue;
            public double MaxActivationValue;
            public double MinActivationValue;
            public double AverageActivationValue;

            public double TotalWeightValue;
            public double MaxWeightValue;
            public double MinWeightValue;
            public double AverageWeightValue;

            public double TotalBiasValue;
            public double MaxBiasValue;
            public double MinBiasValue;
            public double AverageBiasValue;

            public Statistics(Network network) : this()
            {
                Net = network;

                MinWeightValue = Double.MaxValue;
                MaxWeightValue = Double.MinValue;

                MinActivationValue = Double.MaxValue;
                MaxActivationValue = Double.MinValue;

                MinBiasValue = Double.MaxValue;
                MaxBiasValue = Double.MinValue;
            }

            public void CalculateStats()
            {

                // List<Layer> layers = Net.Layers;



                // reset stats
                TotalLayers = 0;
                TotalNodes = 0;
                TotalHiddenNodes = 0;
                TotalWeights = 0;
                TotalActivationValue = 0;

                MinActivationValue = Double.MaxValue;
                MaxActivationValue = Double.MinValue;

                MinWeightValue = Double.MaxValue;
                MaxWeightValue = Double.MinValue;

                MinBiasValue = Double.MaxValue;
                MaxBiasValue = Double.MinValue;

                AverageActivationValue = 0;
                TotalWeightValue = 0;
                AverageWeightValue = 0;
                TotalBiasValue = 0;

                AverageBiasValue = 0;

                // calc stats
                for (int n = 0; n < Net.Layers.Length; n++)
                {
                    TotalLayers++;

                    for (int m = 0; m < Net.Layers[n].Nodes.Length; m++)
                    {

                        for (int w = 0; w < Net.Layers[n].Nodes[m].Weights.Length; w++)
                        {
                            TotalWeightValue += Net.Layers[n].Nodes[m].Weights[w];
                            if (Net.Layers[n].Nodes[m].Weights[w] > MaxWeightValue)
                            {
                                MaxWeightValue = Net.Layers[n].Nodes[m].Weights[w];
                            }
                            if (Net.Layers[n].Nodes[m].Weights[w] < MinWeightValue)
                            {
                                MinWeightValue = Net.Layers[n].Nodes[m].Weights[w];
                            }
                        }
                        TotalWeights += Net.Layers[n].Nodes[m].Weights.Length;
                        TotalNodes++;

                        if (n != Net.Layers.Length - 1)
                        {
                            TotalHiddenNodes++;
                        }

                        TotalActivationValue += Net.Layers[n].Nodes[m].ActivationValue;
                        TotalBiasValue += Net.Layers[n].Nodes[m].Bias;
                        if (Net.Layers[n].Nodes[m].ActivationValue > MaxActivationValue)
                        {
                            MaxActivationValue = Net.Layers[n].Nodes[m].ActivationValue;
                        }
                        if (Net.Layers[n].Nodes[m].ActivationValue < MinActivationValue)
                        {
                            MinActivationValue = Net.Layers[n].Nodes[m].ActivationValue;
                        }
                        if (Net.Layers[n].Nodes[m].Bias > MaxBiasValue)
                        {
                            MaxBiasValue = Net.Layers[n].Nodes[m].Bias;
                        }
                        if (Net.Layers[n].Nodes[m].Bias < MinBiasValue)
                        {
                            MinBiasValue = Net.Layers[n].Nodes[m].Bias;
                        }
                    }
                }

                AverageWeightValue = TotalWeightValue / TotalWeights;
                AverageBiasValue = TotalBiasValue / TotalNodes;
                AverageActivationValue = TotalActivationValue / TotalNodes;

            }
        }

        public Layer[] Layers;

        public Features Features;

        public IActivationFunction ActivationFunction;

        public HyperParameters Params;

        public double[] ActivatedInput;

        public List<string> OutputLabels;

        public Statistics Stats;

        public NetworkType Type = NetworkType.Classifier;
        public Network(
            int inputNodesCount,
            string[] outputLabels, 
            int[] hiddenLayers, 
            double weightStart, 
            double weightEnd, 
            IActivationFunction activationFunction, 
            HyperParameters hyperParameters, 
            Features features)
        {
            Stats = new Statistics(this);

            ActivationFunction = activationFunction;

            Params = hyperParameters;

            Features = features;

            // hidden layers plus output layer
            Layers = new Layer[hiddenLayers.Length + 1];

            ActivatedInput = new double[inputNodesCount];

            // create, sort and normalize the labels
            // sorting is done because of index and training data
            OutputLabels = outputLabels.OrderBy(s => s).ToList<string>().ConvertAll(s => s.ToLower().Trim());

            // keeps track of the node count from the prev layer
            int lastLayerNodeCount = inputNodesCount;

            int count = 0;
            // add all the hidden layers
            foreach (int n in hiddenLayers)
            {
                Layers[count] = new Layer(n, lastLayerNodeCount, weightStart, weightEnd);

                lastLayerNodeCount = n;

                count++;
            }

            // add the output layer
            Layers[Layers.Length - 1] = new Layer(outputLabels.Length, lastLayerNodeCount, weightStart, weightEnd);

        }


        public void RunInput(double[] activatedInput)
        {
            ActivatedInput = activatedInput;

            for (int n = 0; n < Layers.Length; n++)
            {
                for (int m = 0; m < Layers[n].Nodes.Length; m++)
                {

                    // don't do anything with the activation value if currentNode is disabled
                    if (Layers[n].Nodes[m].Enabled)
                    {
                        double nodeValue = 0;

                        for (int w = 0; w < Layers[n].Nodes[m].Weights.Length; w++)
                        {

                            bool enabled = (n == 0 ? true : Layers[n - 1].Nodes[w].Enabled);

                            if (enabled)
                            {
                                // get node act value that is connected to this weight
                                double prevNodeActivationValue = n == 0 ? ActivatedInput[w] : Layers[n - 1].Nodes[w].ActivationValue;

                                // multiple the weight with the value
                                nodeValue += prevNodeActivationValue * Layers[n].Nodes[m].Weights[w];
                            }
                        }

                        // add the bias
                        nodeValue += Layers[n].Nodes[m].Bias;

                        Layers[n].Nodes[m].Value = nodeValue;

                        double computedValue = ActivationFunction.Run(nodeValue);

                        Layers[n].Nodes[m].ActivationValue = computedValue;
                    }
                }
            }
        }
    }
}
