using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    [Serializable]
    public struct Layer
    {
        public Node[] Nodes;

        public Layer(int nodeCount, int weightCount, double weightStart, double weightEnd)
        {
            Nodes = new Node[nodeCount];

            for (int n = 0; n < nodeCount; n++)
            {
                Nodes[n] = new Node(weightCount, weightStart, weightEnd, n);
            }
        }
    }
}
