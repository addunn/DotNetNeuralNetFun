using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    [Serializable]
    public class NetworkStatsSnapshot
    {

        public Dictionary<string, Dictionary<string, double>> Stats;

        public NetworkStatsSnapshot()
        {
            Stats = new Dictionary<string, Dictionary<string, double>>();

            Stats.Add("performance", new Dictionary<string, double>());
            Stats.Add("network", new Dictionary<string, double>());
            Stats.Add("hyperParameters", new Dictionary<string, double>());
            Stats.Add("training", new Dictionary<string, double>());
        }
    }
}
