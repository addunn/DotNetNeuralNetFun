using CL;
using ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPG
{
    [Serializable]
    public class Project
    {
        public string Id = "";

        public string Name = "";

        public string DataPath = "";

        public bool TrainWatchLoopOn = true;

        public string SortProjectItemsByKey = "performance";

        public string SortProjectItemsBySubKey = "cost";

        public string SortProjectDirection = "asc";

        public string CurrentProjectItemTrainingId = "";

        public string SelectedProjectItemId = "";

        // 0 based index
        public int SelectedProjectItemIteration = 0;

        // when a project item is selected, we make this value the selected project item's ChangeIndicator...
        // ... The ChangeIndicator changes on every iteration...
        // We refresh the UI when the two indicators don't match, then update this value
        public int SelectedProjectChangeIndicator = 0;


        public int ProjectItemsRankingChangeIndicator = 0;

        public int ProjectItemsRankingLastChangeIndicator = 0;

        /// <summary>
        /// This is each network the user is training
        /// </summary>
        public List<ProjectItem> projectItems;

        public TrainingItem[] trainingItems;

        public TrainingItem[] testingItems;

        public Project(string name, string solutionDataPath)
        {
            Id = UC.RandomString(15);
            Name = name;
            BuildDataPath(solutionDataPath);
        }
        private void BuildDataPath(string solutionDataPath)
        {
            DataPath = solutionDataPath + Id.ToLower() + "\\";
            Directory.CreateDirectory(DataPath);
        }
        public void SortProjectItems()
        {
            // yikes sort...
            projectItems = projectItems.OrderBy(p => p.NetworkRecords.Stats[p.NetworkRecords.Stats.Count - 1].Stats[SortProjectItemsByKey][SortProjectItemsBySubKey]).ToList();

            if(SortProjectDirection == "desc")
            {
                projectItems.Reverse();
            }
        }
    }
}
