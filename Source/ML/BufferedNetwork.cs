using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML;
using CL;
using System.IO;

namespace ML
{
    /*
     * 
     * This is just like a List<Network> but only the last added item is stored in memory... 
     * All other added items are stored on disk
     * 
     * 
     */
    [Serializable]
    public class BufferedNetworkList
    {
        private Network LastAddedNetwork = null;

        private string ProjectItemDataPath;

        private int Count;

        public BufferedNetworkList(string projectItemDataPath)
        {
            ProjectItemDataPath = projectItemDataPath;
            Count = 0;
        }
        public void Add(Network network)
        {

            if(LastAddedNetwork != null)
            {
                // UC.WriteToBinaryFile(ProjectItemDataPath + "nn_" + (Count - 1) + ".dat", network);
            }

            LastAddedNetwork = network;

            Count++;
        }
        public Network this[int index]
        {
            get => GetNetwork(index);
            set => SetNetwork(index, value);
        }
        private Network GetNetwork(int index)
        {
            Network result = null;

            if(index == Count - 1)
            {
                result = LastAddedNetwork;
            }
            else
            {
                if(File.Exists(ProjectItemDataPath + "nn_" + index + ".dat"))
                {
                    result = UC.ReadFromBinaryFile<Network>(ProjectItemDataPath + "nn_" + index + ".dat");
                }
            }

            return result;
            
        }
        private void SetNetwork(int index, Network network)
        {
            // UC.WriteToBinaryFile(ProjectItemDataPath + "nn_" + index + ".dat", network);
        }
    }
}
