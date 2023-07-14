using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL;
using ML;


namespace MLPG
{
    [Serializable]
    public class ProjectItem
    {  
        public string DataPath = "";

        public bool Enabled = false;

        public Network CurrentNetwork;

        public NetworkRecords NetworkRecords;

        public string Name;
        // this gets refreshed on every iteration
        public int ChangeIndicator;
        public int Iterations { get { return NetworkRecords.Stats.Count; } }

        public string Id;
        
        public ProjectItem(string projectDataPath)
        {
            
            // generate random name
            Name = Data.FemaleNames[Rnd.GetRandomInteger(0, Data.FemaleNames.Length - 1)];
            // generate random ID
            Id = UC.RandomString(15);

            ChangeIndicator = Rnd.GetRandomInteger(int.MinValue, int.MaxValue);

            BuildDataPath(projectDataPath);

            NetworkRecords = new NetworkRecords(DataPath);
        }
        private void BuildDataPath(string projectDataPath)
        {
            DataPath = projectDataPath + Id.ToLower() + "\\";
            Directory.CreateDirectory(DataPath);
        }
        public void TrainedIterate(Training.TrainInfo trainingInfo)
        {
            NetworkStatsSnapshot stats = new NetworkStatsSnapshot();

            // clone the network
            Network clonedNet = UC.Clone(CurrentNetwork);

            clonedNet.Stats.CalculateStats();



            // calc NetworkStatsSnapshot...

            // training
            Dictionary<string, double> training = new Dictionary<string, double>();
            training.Add("iteration", NetworkRecords.Stats.Count + 1);
            training.Add("iterationStopwatch", trainingInfo.ElapsedMilliseconds);
            training.Add("useMomentum", (clonedNet.Features.UseMomentum ? 1 : 0));
            training.Add("useAdaGrad", (clonedNet.Features.UseAdaGrad ? 1 : 0));
            training.Add("useRMSProp", (clonedNet.Features.UseRMSProp ? 1 : 0));
            training.Add("useAdam", (clonedNet.Features.UseAdam ? 1 : 0));
            training.Add("useDynamicDropOut", (clonedNet.Features.UseDynamicDropOut ? 1 : 0));
            training.Add("useRegularDropOut", (clonedNet.Features.UseRegularDropOut ? 1 : 0));
            training.Add("backPropCallsCount", trainingInfo.BackPropCallsCount);

            // performance
            Dictionary<string, double> performance = new Dictionary<string, double>();

            performance.Add("cost", trainingInfo.PerformanceResult.AverageCost);
            performance.Add("costChange", NetworkRecords.Stats.Count == 0 ? 0 : trainingInfo.PerformanceResult.AverageCost - NetworkRecords.Stats[NetworkRecords.Stats.Count - 1].Stats["performance"]["cost"]);
            performance.Add("percCorrect", trainingInfo.PerformanceResult.PercentCorrect);
            performance.Add("percCorrectChange", NetworkRecords.Stats.Count == 0 ? 0 : trainingInfo.PerformanceResult.PercentCorrect - NetworkRecords.Stats[NetworkRecords.Stats.Count - 1].Stats["performance"]["percCorrect"]);

            // hyper params
            Dictionary<string, double> hyperParameters = new Dictionary<string, double>();
            hyperParameters.Add("learningRate", clonedNet.Params.LearningRate);
            hyperParameters.Add("batchSize", clonedNet.Params.BatchSize);
            hyperParameters.Add("maxWeightValue", clonedNet.Params.MaxWeightValue);
            hyperParameters.Add("minWeightValue", clonedNet.Params.MinWeightValue);
            hyperParameters.Add("maxBiasValue", clonedNet.Params.MaxBiasValue);
            hyperParameters.Add("minBiasValue", clonedNet.Params.MinBiasValue);
            hyperParameters.Add("dynamicDropoutLayerNodeCountDivisor", clonedNet.Params.DynamicDropoutLayerNodeCountDivisor);
            hyperParameters.Add("regularDropoutProbability", clonedNet.Params.RegularDropoutProbability);

            // network (store activation function too)
            Dictionary<string, double> network = new Dictionary<string, double>();
            network.Add("activationFunction", clonedNet.ActivationFunction.Id());
            network.Add("totalLayers", clonedNet.Stats.TotalLayers);
            network.Add("averageDisabledNodes", trainingInfo.AverageDroppedNodesCount);
            network.Add("totalNodes", clonedNet.Stats.TotalNodes);
            network.Add("totalHiddenNodes", clonedNet.Stats.TotalHiddenNodes);
            network.Add("totalWeights", clonedNet.Stats.TotalWeights);
            network.Add("totalActivationValue", clonedNet.Stats.TotalActivationValue);
            network.Add("maxActivationValue", clonedNet.Stats.MaxActivationValue);
            network.Add("minActivationValue", clonedNet.Stats.MinActivationValue);
            network.Add("averageActivationValue", clonedNet.Stats.AverageActivationValue);
            network.Add("totalWeightValue", clonedNet.Stats.TotalWeightValue);
            network.Add("maxWeightValue", clonedNet.Stats.MaxWeightValue);
            network.Add("minWeightValue", clonedNet.Stats.MinWeightValue);
            network.Add("averageWeightValue", clonedNet.Stats.AverageWeightValue);
            network.Add("totalBiasValue", clonedNet.Stats.TotalBiasValue);
            network.Add("maxBiasValue", clonedNet.Stats.MaxBiasValue);
            network.Add("minBiasValue", clonedNet.Stats.MinBiasValue);
            network.Add("averageBiasValue", clonedNet.Stats.AverageBiasValue);

            stats.Stats["performance"] = performance;
            stats.Stats["network"] = network;
            stats.Stats["hyperParameters"] = hyperParameters;
            stats.Stats["training"] = training;

            // ths needs to be at the bottom
            NetworkRecords.Stats.Add(stats);
            NetworkRecords.Networks.Add(clonedNet);
        }

    }
}
