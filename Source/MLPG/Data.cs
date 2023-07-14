using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLPG
{
    public class Solution
    {
        public static string DataPath = "";
        public static string Id = "";
        public static int CurrentSelectedProjectIndex = 0;
        public static List<Project> CurrentProjects = new List<Project>();
        public static List<ProjectTraining> ProjectTrainings = new List<ProjectTraining>();
        // linked list would be better
        public static List<string> Commands = new List<string>();

        public static void BuildDataPath()
        {
            DataPath = Data.ExecutablePath + "runtime-storage\\" + Id.ToLower() + "\\";
            Directory.CreateDirectory(DataPath);
        }
    }
    public class Data
    {
        public static string ExecutablePath = "";
        public static string[] FemaleNames;
        

        
    }
    public class CleanUp
    {
        public static void RunTimeStorage()
        {
            string fullPath = Data.ExecutablePath + "runtime-storage\\";

            DirectoryInfo di = new DirectoryInfo(fullPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }


    }
}
