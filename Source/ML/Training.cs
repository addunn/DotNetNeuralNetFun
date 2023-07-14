using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL;


namespace ML
{
    public class Training
    {
        public Network Net;

        public TrainingItem[] TrainingItems;

        public TrainingItem[] TestingItems;

        public TrainInfo CurrentTrainInfo;


        [Serializable]
        public class TrainInfo
        {
            public PerformanceResult PerformanceResult;
            public long ElapsedMilliseconds;
            public int AverageDroppedNodesCount = 0;
            public int BackPropCallsCount = 0;


            public TrainInfo()
            {

            }

        }

        private struct TrainResult
        {
            public double[][][] WeightDerivatives;
            public double[][] BiasDerivatives;
        }

        public struct PerformanceResult
        {
            public double AverageCost;

            public double PercentCorrect;

            public PerformanceResult(double averageCost, double percentCorrect)
            {
                AverageCost = averageCost;
                PercentCorrect = percentCorrect;
            }

        }

        public Training(Network net, TrainingItem[] trainingItems, TrainingItem[] testingItems)
        {
            // i guess this is good to clone because we will be updating weights and such and don't want to step on another threads toes
            Net = UC.Clone(net);
            TrainingItems = trainingItems;
            TestingItems = testingItems;
        }



        public PerformanceResult CalculatePerformance(Network net)
        {
            double avgCost = 0;

            bool isClassifier = net.Type == NetworkType.Classifier;

            // grab a decent amount of testing items to calc the performance
            int testingItemsBatchCount = (Net.Params.BatchSize * 5);

            // make sure we don't exceed the amount of testing items we have
            if (testingItemsBatchCount > TestingItems.Length)
            {
                testingItemsBatchCount = TestingItems.Length;
            }

            int rightWrongSum = 0;

            double correctAverageSum = 0;

            for (int m = 0; m < testingItemsBatchCount; m++)
            {

                net.RunInput(TestingItems[m].Input);

                double[] expectedOutput = TestingItems[m].ExpectedOutput;

                double highestActivation = 0;
                int highestActivationIndex = 0;

                double sum = 0;

                int correctCount = 0;


                for (int e = 0; e < expectedOutput.Length; e++)
                {
                    if (isClassifier)
                    {
                        if (net.Layers[net.Layers.Length - 1].Nodes[e].ActivationValue > highestActivation)
                        {
                            highestActivation = net.Layers[net.Layers.Length - 1].Nodes[e].ActivationValue;
                            highestActivationIndex = e;
                        }
                    }
                    else
                    {
                        if (expectedOutput[e] == net.Layers[net.Layers.Length - 1].Nodes[e].ActivationValue)
                        {
                            correctCount++;
                        }
                    }

                    sum += Math.Pow(net.Layers[net.Layers.Length - 1].Nodes[e].ActivationValue - expectedOutput[e], 2);
                }

                if (isClassifier)
                {
                    // network got it correct, so tally that
                    if (highestActivationIndex == TestingItems[m].ExpectedOutputIndex)
                    {
                        rightWrongSum++;
                    }
                }
                else
                {
                    correctAverageSum += correctCount / expectedOutput.Length;
                }
                    

                avgCost += sum;
            }

            double percCorrect = 0;

            if (isClassifier)
            {
                percCorrect = (double)rightWrongSum / (double)testingItemsBatchCount;
            }
            else
            {
                percCorrect = (double)correctAverageSum / (double)testingItemsBatchCount;
            }

            avgCost /= testingItemsBatchCount;

            return new PerformanceResult(avgCost, percCorrect);
        }

        public void Train(int passThroughs, bool calcPerformance, int threads = 10)
        {
            // time this train method and store it in CurrentTrainInfo
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            CurrentTrainInfo = new TrainInfo();

            int miniBatchSize = (int)Math.Floor((double)Net.Params.BatchSize / threads);

            int leftOverBatchSize = Net.Params.BatchSize - (miniBatchSize * threads);

            // if leftOverBatchSize is 0, it causes chaos
            leftOverBatchSize = leftOverBatchSize == 0 ? 1 : leftOverBatchSize;

            for (int n = 0; n < passThroughs; n++)
            {

                Network[] networks = new Network[threads + 1];

                // make copies of the networks for each thread because values in the network will change (e.g., activation)
                for (int t = 0; t < threads + 1; t++)
                {
                    networks[t] = UC.Clone(Net);
                }

                // these arrays are yikes...
                TrainResult finalTrainResult = new TrainResult();

                TrainResult[] trainResults = new TrainResult[threads + 1];


                Parallel.For(0, threads + 1, m =>
                {
                    trainResults[m] = SubTrain(networks[m], m == threads ? leftOverBatchSize: miniBatchSize);
                });

                for (int t = 0; t < trainResults.Length; t++)
                {
                    if (t == 0)
                    {
                        finalTrainResult.WeightDerivatives = trainResults[t].WeightDerivatives;
                        finalTrainResult.BiasDerivatives = trainResults[t].BiasDerivatives;
                    }
                    else
                    {
                        for (int r = 0; r < trainResults[t].WeightDerivatives.Length; r++)
                        {
                            for (int c = 0; c < trainResults[t].WeightDerivatives[r].Length; c++)
                            {
                                finalTrainResult.BiasDerivatives[r][c] += trainResults[t].BiasDerivatives[r][c];

                                for (int d = 0; d < trainResults[t].WeightDerivatives[r][c].Length; d++)
                                {
                                    finalTrainResult.WeightDerivatives[r][c][d] += trainResults[t].WeightDerivatives[r][c][d];
                                }
                            }
                        }
                    }
                }

                for (int r = 0; r < finalTrainResult.WeightDerivatives.Length; r++)
                {
                    Parallel.For(0, finalTrainResult.WeightDerivatives[r].Length, c =>
                    {
                    
                        finalTrainResult.BiasDerivatives[r][c] = finalTrainResult.BiasDerivatives[r][c] / ((miniBatchSize * threads) + leftOverBatchSize);

                        double biasDer = finalTrainResult.BiasDerivatives[r][c];
                        double biasChange = (-1 * Net.Params.LearningRate * biasDer);

                        if (Net.Features.UseAdam) // <3
                        {
                            Net.Layers[r].Nodes[c].BiasSpecialG = ((Net.Params.AdamB) * (Net.Layers[r].Nodes[c].BiasSpecialG)) + ((1 - Net.Params.AdamB) * (Math.Pow(biasDer, 2)));
                            double m = ((Net.Params.AdamAlpha) * (Net.Layers[r].Nodes[c].BiasAdamLastM)) + ((1 - Net.Params.AdamAlpha) * biasDer);
                            Net.Layers[r].Nodes[c].BiasAdamLastM = m;
                            biasChange = -1 * (Net.Params.LearningRate / (Math.Sqrt(Net.Layers[r].Nodes[c].BiasSpecialG) + Net.Params.AdamE)) * (m);
                        }
                        else
                        {
                            if (Net.Features.UseAdaGrad)
                            {
                                Net.Layers[r].Nodes[c].BiasSpecialG = Net.Layers[r].Nodes[c].BiasSpecialG + (Math.Pow(biasDer, 2));
                                biasChange = -1 * (Net.Params.LearningRate / (Math.Sqrt(Net.Layers[r].Nodes[c].BiasSpecialG) + Net.Params.AdaGradE)) * (biasDer);
                            }
                            else if (Net.Features.UseRMSProp)
                            {
                                Net.Layers[r].Nodes[c].BiasSpecialG = ((Net.Params.RMSPropB) * (Net.Layers[r].Nodes[c].BiasSpecialG)) + ((1 - Net.Params.RMSPropB) * (Math.Pow(biasDer, 2)));
                                biasChange = -1 * (Net.Params.LearningRate / (Math.Sqrt(Net.Layers[r].Nodes[c].BiasSpecialG) + Net.Params.RMSPropE)) * (biasDer);
                            }

                            if (Net.Features.UseMomentum)
                            {
                                biasChange += (Net.Params.MomentumAlpha * Net.Layers[r].Nodes[c].BiasChangeLastValue);
                            }
                        }

                        // store this bias change for optimization algos
                        Net.Layers[r].Nodes[c].BiasChangeLastValue = biasChange;

                        double b = Net.Layers[r].Nodes[c].Bias + biasChange;

                        if (b > Net.Params.MaxBiasValue)
                        {
                            b = Net.Params.MaxBiasValue;
                        }
                        else if (b < Net.Params.MinBiasValue)
                        {
                            b = Net.Params.MinBiasValue;
                        }

                        Net.Layers[r].Nodes[c].Bias = b;

                        for (int d = 0; d < finalTrainResult.WeightDerivatives[r][c].Length; d++)
                        {

                            // the amount we run isn't directly BatchSize, we do that weight leftover thing
                            finalTrainResult.WeightDerivatives[r][c][d] = finalTrainResult.WeightDerivatives[r][c][d] / ((miniBatchSize * threads) + leftOverBatchSize);

                            double weightDer = finalTrainResult.WeightDerivatives[r][c][d];
                            double weightChange = (-1 * Net.Params.LearningRate * weightDer);

                            if (Net.Features.UseAdam)
                            {
                                Net.Layers[r].Nodes[c].WeightSpecialG[d] = ((Net.Params.AdamB) * (Net.Layers[r].Nodes[c].WeightSpecialG[d])) + ((1 - Net.Params.AdamB) * (Math.Pow(weightDer, 2)));
                                double m = ((Net.Params.AdamAlpha) * (Net.Layers[r].Nodes[c].WeightAdamLastM[d])) + ((1 - Net.Params.AdamAlpha) * weightDer);
                                Net.Layers[r].Nodes[c].WeightAdamLastM[d] = m;
                                weightChange = -1 * (Net.Params.LearningRate / (Math.Sqrt(Net.Layers[r].Nodes[c].WeightSpecialG[d]) + Net.Params.AdamE)) * (m);
                            }
                            else
                            {
                                if (Net.Features.UseAdaGrad)
                                {
                                    Net.Layers[r].Nodes[c].WeightSpecialG[d] = Net.Layers[r].Nodes[c].WeightSpecialG[d] + (Math.Pow(weightDer, 2));
                                    weightChange = -1 * (Net.Params.LearningRate / (Math.Sqrt(Net.Layers[r].Nodes[c].WeightSpecialG[d]) + Net.Params.AdaGradE)) * (weightDer);
                                }
                                else if (Net.Features.UseRMSProp)
                                {
                                    Net.Layers[r].Nodes[c].WeightSpecialG[d] = ((Net.Params.RMSPropB) * (Net.Layers[r].Nodes[c].WeightSpecialG[d])) + ((1 - Net.Params.RMSPropB) * (Math.Pow(weightDer, 2)));
                                    weightChange = -1 * (Net.Params.LearningRate / (Math.Sqrt(Net.Layers[r].Nodes[c].WeightSpecialG[d]) + Net.Params.RMSPropE)) * (weightDer);
                                }

                                if (Net.Features.UseMomentum)
                                {
                                    weightChange += (Net.Params.MomentumAlpha * Net.Layers[r].Nodes[c].WeightChangeLastValue[d]);
                                }
                            }

                            // store this weight change for optimization algos
                            Net.Layers[r].Nodes[c].WeightChangeLastValue[d] = weightChange;

                            double w = Net.Layers[r].Nodes[c].Weights[d] + weightChange;

                            if (w > Net.Params.MaxWeightValue)
                            {
                                w = Net.Params.MaxWeightValue;
                            }
                            else if (w < Net.Params.MinWeightValue)
                            {
                                w = Net.Params.MinWeightValue;
                            }

                            Net.Layers[r].Nodes[c].Weights[d] = w;
                        }
                    });

                }
            }


            if (calcPerformance)
            {
                // total backprop calls
                int totalBackPropCalls = ((miniBatchSize * threads) + leftOverBatchSize) * passThroughs;
                CurrentTrainInfo.BackPropCallsCount = totalBackPropCalls;
                CurrentTrainInfo.AverageDroppedNodesCount = (int)Math.Round((double)CurrentTrainInfo.AverageDroppedNodesCount / (double)totalBackPropCalls);
                CurrentTrainInfo.PerformanceResult = CalculatePerformance(Net);
            }

            stopwatch.Stop();
            CurrentTrainInfo.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;


        }

        private int RunDropOut(Network net)
        {
            int dropOutCount = 0;

            if(net.Features.UseRegularDropOut || net.Features.UseDynamicDropOut)
            {
                for (int l = 0; l < net.Layers.Length; l++)
                {
                    for (int n = 0; n < net.Layers[l].Nodes.Length; n++)
                    {
                        if (net.Features.UseRegularDropOut)
                        {
                            net.Layers[l].Nodes[n].Enabled = (Rnd.GetRandomDouble(0, 1) > net.Params.RegularDropoutProbability);
                        }
                        else if(net.Features.UseDynamicDropOut)
                        {
                            net.Layers[l].Nodes[n].Enabled = (Rnd.GetRandomDouble(0, 1) > (net.Layers[l].Nodes.Length / net.Params.DynamicDropoutLayerNodeCountDivisor));
                        }
                        if (!net.Layers[l].Nodes[n].Enabled)
                        {
                            dropOutCount++;
                        }
                    }
                }
            }

            return dropOutCount;
        }

        private void UndoDropOut(Network net)
        {
            for (int l = 0; l < net.Layers.Length; l++)
            {
                for (int n = 0; n < net.Layers[l].Nodes.Length; n++)
                {
                    net.Layers[l].Nodes[n].Enabled = true;
                }
            }
        }

        private TrainResult SubTrain(Network net, int subBatchSize)
        {
            TrainResult subTrainResult = new TrainResult();

            for (int m = 0; m < subBatchSize; m++)
            {
                // select random training item from all training items
                int rndIndex = Rnd.GetRandomInteger(0, TrainingItems.Length - 1);

                // randomly disable nodes... will only run if dropout is enabled
                int droppedCount = RunDropOut(net);

                net.RunInput(TrainingItems[rndIndex].Input);

                // will there be issues with multiple threads??? only time will tell!
                CurrentTrainInfo.AverageDroppedNodesCount += droppedCount;

                TrainResult backPropResult = BackPropogateGradientDescent(net, TrainingItems[rndIndex].ExpectedOutput);

                // this enable all nodes
                UndoDropOut(net);

                if (m == 0)
                {

                    subTrainResult.WeightDerivatives = backPropResult.WeightDerivatives;
                    subTrainResult.BiasDerivatives = backPropResult.BiasDerivatives;
                }
                else
                {
                    // add the weight adjustments to the summed list
                    for (int p = 0; p < backPropResult.WeightDerivatives.Length; p++)
                    {
                        for (int r = 0; r < backPropResult.WeightDerivatives[p].Length; r++)
                        {
                            for (int t = 0; t < backPropResult.WeightDerivatives[p][r].Length; t++)
                            {
                                subTrainResult.WeightDerivatives[p][r][t] += backPropResult.WeightDerivatives[p][r][t];
                            }
                        }
                    }

                    // add the bias adjustments to the summed list
                    for (int p = 0; p < backPropResult.BiasDerivatives.Length; p++)
                    {
                        for (int r = 0; r < backPropResult.BiasDerivatives[p].Length; r++)
                        {
                            subTrainResult.BiasDerivatives[p][r] += backPropResult.BiasDerivatives[p][r];
                        }
                    }
                }
            }

            return subTrainResult;
        }

        
        private TrainResult BackPropogateGradientDescent(Network net, double[] expectedOutput)
        {
            
            TrainResult result = new TrainResult();

            double[][][] weightDerivatives = new double[net.Layers.Length][][];
            double[][] biasDerivatives = new double[net.Layers.Length][];

            // handy storage to make less calculations...
            double[][] derCostActivations = new double[net.Layers.Length][];
            double[][] derActivationValues = new double[net.Layers.Length][];

            for (int l = net.Layers.Length - 1; l >= 0; l--)
            {
                weightDerivatives[l] = new double[net.Layers[l].Nodes.Length][];
                biasDerivatives[l] = new double[net.Layers[l].Nodes.Length];

                derCostActivations[l] = new double[net.Layers[l].Nodes.Length];
                derActivationValues[l] = new double[net.Layers[l].Nodes.Length];
                
                Parallel.For(0, net.Layers[l].Nodes.Length, n =>
                {
                    weightDerivatives[l][n] = new double[net.Layers[l].Nodes[n].Weights.Length];

                    // check if node is not dropped out
                    if (net.Layers[l].Nodes[n].Enabled) { 

                        double derCostActivation = 0;

                        if (l == net.Layers.Length - 1)
                        {
                            derCostActivation = 2 * (net.Layers[l].Nodes[n].ActivationValue - expectedOutput[n]);
                        }
                        else
                        {
                            double sum = 0;

                            // peak forward to next layer to calc derCostActivation of current node
                            for (int k = 0; k < net.Layers[l + 1].Nodes.Length; k++)
                            {
                                if(net.Layers[l + 1].Nodes[k].Enabled) { 
                                    sum += derCostActivations[l + 1][k] * derActivationValues[l + 1][k] * net.Layers[l + 1].Nodes[k].Weights[n];
                                }
                            }

                            derCostActivation = sum;
                        }

                        double derActivationValue = net.ActivationFunction.Derivative(net.Layers[l].Nodes[n].Value);

                        // we store these because we will need these when we peak forward to calculate derCostActivation
                        derCostActivations[l][n] = derCostActivation;
                        derActivationValues[l][n] = derActivationValue;

                        for (int w = 0; w < net.Layers[l].Nodes[n].Weights.Length; w++)
                        {
                            bool enabled = (l == 0 ? true : net.Layers[l - 1].Nodes[w].Enabled);

                            if (enabled) {
                                double derValueWeight = (l == 0 ? net.ActivatedInput[w] : net.Layers[l - 1].Nodes[w].ActivationValue); ;

                                double derCostWeight = derCostActivation * derActivationValue * derValueWeight;

                                weightDerivatives[l][n][w] = derCostWeight;
                            }
                        }

                        // calc bias derivative... .the derValueBias is just 1
                        biasDerivatives[l][n] = derCostActivation * derActivationValue * 1;
                    }
                });
            }

            result.WeightDerivatives = weightDerivatives;
            result.BiasDerivatives = biasDerivatives;

            return result;
        }

    }
}
