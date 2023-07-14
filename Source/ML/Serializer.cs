using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    public class Serializer
    {
        public static string JSONSerializeNetwork(Network NN)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{\"input\":[");
            for (int n = 0; n < NN.ActivatedInput.Length; n++)
            {
                sb.Append(NN.ActivatedInput[n] + (n != NN.ActivatedInput.Length - 1 ? "," : ""));
            }


            sb.Append("],\"outputLabels\":[");

            for (int n = 0; n < NN.OutputLabels.Count; n++)
            {
                sb.Append("\"" + NN.OutputLabels[n] + (n != NN.OutputLabels.Count - 1 ? "\"," : "\""));
            }


            sb.Append("],\"layers\":[");

            for (int n = 0; n < NN.Layers.Length; n++)
            {
                sb.Append("[");
                for (int m = 0; m < NN.Layers[n].Nodes.Length; m++)
                {
                    sb.Append("{");
                    sb.Append("\"bias\":" + NN.Layers[n].Nodes[m].Bias + ",");
                    sb.Append("\"activationValue\":" + NN.Layers[n].Nodes[m].ActivationValue + ",");
                    sb.Append("\"value\":" + NN.Layers[n].Nodes[m].Value + ",");
                    sb.Append("\"weights\":[");
                    for (int w = 0; w < NN.Layers[n].Nodes[m].Weights.Length; w++)
                    {
                        sb.Append(NN.Layers[n].Nodes[m].Weights[w] + (w == NN.Layers[n].Nodes[m].Weights.Length - 1 ? "" : ","));
                    }
                    sb.Append("]");

                    sb.Append("}" + (m != NN.Layers[n].Nodes.Length - 1 ? "," : ""));

                }
                sb.Append("]" + (n != NN.Layers.Length - 1 ? "," : ""));
            }

            sb.Append("]}");
            return sb.ToString();
        }
    }
}
