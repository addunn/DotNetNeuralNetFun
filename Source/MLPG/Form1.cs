using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using CL;

namespace MLPG
{
    public partial class Form1 : Form
    {
        public ChromiumWebBrowser browser;

        public Form1()
        {
            InitializeComponent();

            Data.ExecutablePath = AppDomain.CurrentDomain.BaseDirectory;

            Cef.Initialize(new CefSettings());
            browser = new ChromiumWebBrowser("file:///" + Global.SharedPath + "html\\mlpg\\index.html");
            
            this.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;
            
            browser.JavascriptObjectRepository.Register("boundAsync", new BoundObject(), true);

            browser.LoadingStateChanged += OnLoadingStateChanged;

        }
        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            
            if (!args.IsLoading)
            {
                // page has finished loading...
                

                browser.ShowDevTools();
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // kludgy way to do this, but it works
            Global.SharedPath = AppDomain.CurrentDomain.BaseDirectory.Replace("\\Source\\MLPG\\compiled", "\\Shared\\");

            resizeBrowser();

            // load female names
            Data.FemaleNames = UC.ReadFileLinesToArray(Global.SharedPath + "assets\\female-names.txt");

            CleanUp.RunTimeStorage();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            resizeBrowser();
        }
        private void resizeBrowser()
        {
            browser.Width = this.DisplayRectangle.Width;
            browser.Height = this.DisplayRectangle.Height;
            browser.Top = 0;
            browser.Left = 0;
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            resizeBrowser();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            resizeBrowser();
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {

            for(int n = 0; n < Solution.CurrentProjects.Count; n++)
            {
                Project project = Solution.CurrentProjects[n];
                if(project.projectItems != null && project.projectItems.Count > 0)
                {
                    // start detecting changes and update the UI accordingly:


                    if (project.ProjectItemsRankingChangeIndicator != project.ProjectItemsRankingLastChangeIndicator)
                    {
                        // refresh the nodes ranking column...
                        project.ProjectItemsRankingChangeIndicator = project.ProjectItemsRankingLastChangeIndicator;
                        string json = Serializer.JSONSerializeProjectItems(project, n, true);
                        string escapedJson = HttpUtility.JavaScriptStringEncode(json);
                        browser.ExecuteScriptAsync("app.formReceive.networkListRefresh(\"" + escapedJson + "\")");
                    }


                    ProjectItem projectItem = project.projectItems.Find(p => p.Id == project.SelectedProjectItemId);

                    // if selected project exists AND the change indicators are different, update the UI with it
                    if (Solution.CurrentSelectedProjectIndex == n && projectItem != null && projectItem.ChangeIndicator != project.SelectedProjectChangeIndicator)
                    {
                        // refresh the selected project...
                        project.SelectedProjectChangeIndicator = projectItem.ChangeIndicator;
                        string json = Serializer.JSONSerializeSelectedProjectItem(project, n, true);
                        string escapedJson = HttpUtility.JavaScriptStringEncode(json);
                        browser.ExecuteScriptAsync("app.formReceive.selectedProjectItemRefresh(\"" + escapedJson + "\")");
                    }


                }
            }

        }

        private void CommandWatcher_Tick(object sender, EventArgs e)
        {
            if(Solution.Commands.Count > 0)
            {
                // take the first one
                string cmd = Solution.Commands[0];
                string[] cmdValues = cmd.Split(':');
                switch (cmdValues[0])
                {

                    case "set-selected-projectitem":

                        if (Solution.CurrentProjects.Count > 0)
                        {
                            // set-selected-projectitem:INDEX:ID:ITERATION
                            int index = int.Parse(cmdValues[1]);
                            string id = cmdValues[2];
                            int iteration = int.Parse(cmdValues[3]);

                            // check that these values are good
                            ProjectItem projectItem = Solution.CurrentProjects[index].projectItems.Find(p => p.Id == id);

                            if (projectItem != null && iteration <= projectItem.Iterations)
                            {
                                Solution.CurrentSelectedProjectIndex = index;
                                Solution.CurrentProjects[index].SelectedProjectItemId = id;
                                Solution.CurrentProjects[index].SelectedProjectItemIteration = iteration;
                            }

                        }
                        break;
                    case "start-training":


                        browser.ExecuteScriptAsync("app.formReceive.logMessage(\"Loading and building test and training data...\")");

                        Solution.Id = UC.RandomString(15);
                        Solution.BuildDataPath();

                        Solution.CurrentProjects.Add(Testing.CreateTestProjectForMNIST());

                        browser.ExecuteScriptAsync("app.formReceive.logMessage(\"Done!\")");

                        Solution.ProjectTrainings.Add(new ProjectTraining(Solution.CurrentProjects[0]));

                        string json1 = Serializer.JSONSerializeProject(Solution.CurrentProjects[0], 0);
                        string escapedJson1 = HttpUtility.JavaScriptStringEncode(json1);
                        browser.ExecuteScriptAsync("app.formReceive.projectAdd(\"" + escapedJson1 + "\")");

                        browser.ExecuteScriptAsync("app.formReceive.buildAllProjects()");

                        browser.ExecuteScriptAsync("app.formReceive.logMessage(\"Training networks...\")");

                        // TrainWatchLoop is the main training loop where all training of the neural networks are done.
                        Task.Run(() => Solution.ProjectTrainings[0].TrainWatchLoop());
                        


                        break;
                    default:
                        break;
                }

                // remove that item (will always be the first element)
                Solution.Commands.Remove(cmd);
            }
        }

        private void ProjectDetector_Tick(object sender, EventArgs e)
        {
            /*
            if(Data.CurrentProject != null && Data.ProjectTraining.MainProject == null)
            {
                Data.ProjectTraining.MainProject = Data.CurrentProject;

                string json = Serializer.JSONSerializeProject(Data.CurrentProject);
                string escapedJson = HttpUtility.JavaScriptStringEncode(json);
                browser.ExecuteScriptAsync("app.formReceive.projectRefresh(\"" + escapedJson + "\")");
            }
            */
        }

        private void NodeTrainingRefresh_Tick(object sender, EventArgs e)
        {
            for (int n = 0; n < Solution.CurrentProjects.Count; n++)
            {
                Project project = Solution.CurrentProjects[n];
                if (project.projectItems != null && project.projectItems.Count > 0)
                {
                    browser.ExecuteScriptAsync("app.formReceive.currentProjectItemTrainingRefresh(" + n + ",\"" + project.CurrentProjectItemTrainingId + "\")");
                }
            }
            
        }
    }

    // declare the new class
    public class BoundObject
    {
        // declare the procedure called by JS
        // in this example a value passed from JS is assigned to a TextBox1 on Form1
        public void uiCommand(string cmd)
        {
            Solution.Commands.Add(cmd);
        }
    }

}
