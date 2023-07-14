using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML;
using CL;
using System.Diagnostics;

namespace MLPG
{
    public class ProjectTraining
    {
        public Project MainProject = null;

        public ProjectTraining(Project project)
        {
            MainProject = project;
        }

        // this is the main training loop where all the training happens
        public void TrainWatchLoop()
        {
            while (MainProject.TrainWatchLoopOn)
            {

                // check if we can start training or not
                if(MainProject != null && MainProject.projectItems != null && MainProject.projectItems.Count > 0 && MainProject.testingItems.Length > 0 && MainProject.trainingItems.Length > 0)
                {

                    // freeze left column UI and don't update it
                    bool updateProjectItemsRankingChangeIndicator = false;

                    for (int p = 0; p < MainProject.projectItems.Count; p++)
                    {

                        MainProject.CurrentProjectItemTrainingId = MainProject.projectItems[p].Id;

                        // check if enabled
                        if (MainProject.projectItems[p].Enabled)
                        {
                            // we are going to train at least one item, so make sure we update the ranking change indicator after the FOR....
                            updateProjectItemsRankingChangeIndicator = true;
                            MainProject.trainingItems.Shuffle();
                            MainProject.testingItems.Shuffle();
                            Training training = new Training(MainProject.projectItems[p].CurrentNetwork, MainProject.trainingItems, MainProject.testingItems);

                            training.Train(MainProject.projectItems[p].CurrentNetwork.Params.PassThroughs, true, 10);
    
                            MainProject.projectItems[p].CurrentNetwork = training.Net;
                            MainProject.projectItems[p].TrainedIterate(training.CurrentTrainInfo);

                            // mark project item as changed
                            MainProject.projectItems[p].ChangeIndicator = Rnd.GetRandomInteger(int.MinValue, int.MaxValue);

                        }
                    }

                    // unfreeze left column UI and allow updating it once
                    if (updateProjectItemsRankingChangeIndicator)
                    {
                        MainProject.SortProjectItems();
                        MainProject.ProjectItemsRankingChangeIndicator = Rnd.GetRandomInteger(int.MinValue, int.MaxValue);
                    }

                }
            }
        }
    }
}
