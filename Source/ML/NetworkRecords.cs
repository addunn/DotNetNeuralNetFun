using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    [Serializable]
    public class NetworkRecords
    {
        public List<NetworkStatsSnapshot> Stats;
        // when serializing the output to the UI, don't traverse each network to pull data...
        // keep data directly on this class for keeping things organized

        // if this list gets too big, start seemlessly saving and retrieving from disk somehow.. 
        // ...possibly use a getter method on this class for items in the list... i'll worry about that later
        public BufferedNetworkList Networks;

        // other configs... maybe something that deletes/duplicates itself if certain conditions are met, etc
        public NetworkRecords(string projectItemDataPath)
        {
            Stats = new List<NetworkStatsSnapshot>();
            Networks = new BufferedNetworkList(projectItemDataPath);
        }

        
    }
}
