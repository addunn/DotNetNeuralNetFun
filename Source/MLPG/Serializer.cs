using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML;
using CL;
using System.Web;

namespace MLPG
{
    public class Serializer
    {
        public static string JSONSerializeStats(List<NetworkStatsSnapshot> stats, int startIndex, int endIndex)
        {

            
            if(stats.Count == 0 || startIndex < 0 || endIndex > stats.Count - 1 || startIndex > endIndex)
            {
                // we got bad data, so no stats for you...
                return ("{}");
            }
            else
            {

                StringBuilder sb = new StringBuilder();
            
                // yikes
                Dictionary<string, Dictionary<string, double[]>> flatStats = new Dictionary<string, Dictionary<string, double[]>>();

                // create the structure of the flatStats by going through the first index
                foreach (KeyValuePair<string, Dictionary<string, double>> kv1 in stats[startIndex].Stats)
                {
                    string key1 = kv1.Key;
                    Dictionary<string, double> value1 = kv1.Value;

                    flatStats.Add(key1, new Dictionary<string, double[]>());

                    foreach (KeyValuePair<string, double> kv2 in value1)
                    {
                        string key2 = kv2.Key;
                        double value2 = kv2.Value;

                        flatStats[key1].Add(key2, new double[(endIndex - startIndex) + 1]);
                        flatStats[key1][key2][0] = value2;
                    }
                }

                // go through the rest (exclude the first index because we already did that)
                for (int n = startIndex + 1; n <= endIndex; n++)
                {
                    foreach (KeyValuePair<string, Dictionary<string, double>> kv1 in stats[n].Stats)
                    {
                        string key1 = kv1.Key;
                        Dictionary<string, double> value1 = kv1.Value;

                        foreach (KeyValuePair<string, double> kv2 in value1)
                        {
                            string key2 = kv2.Key;
                            double value2 = kv2.Value;

                            flatStats[key1][key2][n] = value2;
                        }
                    }
                }


                sb.Append("{");

                int count1 = 0;
                

                // finally output flatStats
                foreach (KeyValuePair<string, Dictionary<string, double[]>> kv1 in flatStats)
                {
                    // performance, hyperParameters, etc
                    string key1 = kv1.Key;
                    Dictionary<string, double[]> value1 = kv1.Value;

                    int count2 = 0;

                    sb.Append("\"" + key1 + "\":{");

                    foreach (KeyValuePair<string, double[]> kv2 in value1)
                    {
                        // cost, totalWeights, etc
                        string key2 = kv2.Key;
                        double[] value2 = kv2.Value;

                        sb.Append("\"" + key2 + "\":");
                        if (startIndex == endIndex)
                        {
                            // no array
                            sb.Append(value2[0]);
                        }
                        else
                        {
                            // array
                            sb.Append("[" + String.Join(",", value2) + "]");
                        }

                        count2++;
                        if (count2 < value1.Count)
                        {
                            sb.Append(",");
                        }
                    }

                    sb.Append("}");

                    count1++;
                    if (count1 < flatStats.Count)
                    {
                        sb.Append(",");
                    }
                }

                sb.Append("}");

                return sb.ToString();
            }
        }

        public static string JSONSerializeSelectedProjectItem(Project project, int projectIndex, bool includeProjectIndex)
        {

            StringBuilder sb = new StringBuilder();

            // get the selected project item by selected id
            ProjectItem selectedProjectItem = project.projectItems.Find(o => o.Id == project.SelectedProjectItemId);

            if (selectedProjectItem == null)
            {
                if (includeProjectIndex)
                {
                    sb.Append("{\"projectIndex\":" + projectIndex + "}");
                }
                else
                {
                    sb.Append("{");
                }
            }
            else { 
                // if the SelectedProjectItemIteration is -1, send the most recent iteration
                int selectedIteration = project.SelectedProjectItemIteration == -1 ? selectedProjectItem.NetworkRecords.Stats.Count - 1 : project.SelectedProjectItemIteration;
                sb.Append("{");

                if (includeProjectIndex) { 
                    sb.Append("\"projectIndex\":" + projectIndex + ",");
                }

                sb.Append("\"name\":\"" + selectedProjectItem.Name + "\",");
                sb.Append("\"id\":\"" + selectedProjectItem.Id + "\",");
                sb.Append("\"enabled\":" + (selectedProjectItem.Enabled ? "true" : "false") + ",");
                sb.Append("\"iterations\":" + selectedProjectItem.Iterations + ",");
                sb.Append("\"selectedIteration\":" + selectedIteration + ",");

                sb.Append("\"stats\":");
                sb.Append(JSONSerializeStats(selectedProjectItem.NetworkRecords.Stats, 0, selectedIteration));
                sb.Append(",");

                sb.Append("\"network\":");

                if (selectedProjectItem.NetworkRecords.Stats.Count == 0)
                {
                    sb.Append("{}");
                }
                else
                {
                    sb.Append(ML.Serializer.JSONSerializeNetwork(selectedProjectItem.NetworkRecords.Networks[selectedIteration]));
                }
                sb.Append("}");
            }

            return sb.ToString();
        }

        public static string JSONSerializeProjectItems(Project project, int projectIndex, bool includeProjectIndex)
        {
            StringBuilder sb = new StringBuilder();

            if (includeProjectIndex)
            {
                sb.Append("{\"projectIndex\":" + projectIndex + ",");
                sb.Append("\"items\":");
            }

            sb.Append("[");

            for (int n = 0; n < project.projectItems.Count; n++)
            {
                sb.Append("{");
                sb.Append("\"name\":\"" + project.projectItems[n].Name + "\",");
                sb.Append("\"id\":\"" + project.projectItems[n].Id + "\",");
                sb.Append("\"enabled\":" + (project.projectItems[n].Enabled ? "true" : "false") + ",");
                sb.Append("\"iterations\":" + project.projectItems[n].Iterations + ",");
                sb.Append("\"stats\":");
                sb.Append(JSONSerializeStats(project.projectItems[n].NetworkRecords.Stats, project.projectItems[n].NetworkRecords.Stats.Count - 1, project.projectItems[n].NetworkRecords.Stats.Count - 1));
                sb.Append("}");

                if (n < project.projectItems.Count - 1)
                {
                    sb.Append(",");
                }
            }
            if (includeProjectIndex)
            {
                sb.Append("]}");
            }
            else
            {
                sb.Append("]");
            }

            return sb.ToString();
        }


        public static string JSONSerializeProject(Project project, int projectIndex)
        {
            

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"index\":" + projectIndex + ",\"name\":\"" + HttpUtility.JavaScriptStringEncode(project.Name) + "\",\"trainingItems\":{\"count\":" + project.trainingItems.Length + "},\"testingItems\":{\"count\":" + project.testingItems.Length + "},");

            sb.Append("\"projectItems\":");

            sb.Append(JSONSerializeProjectItems(project, projectIndex, false));

            sb.Append("}");



            return sb.ToString();
        }
    }
}
