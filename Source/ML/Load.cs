using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CL;

namespace ML
{
    public class Load
    {
        public static List<TrainingItem> ClassifierTrainingSubset(double[] expectedOutput, Int16 expectedOutputIndex, string folderPath)
        {
            List<TrainingItem> result = new List<TrainingItem>();

            string[] allfiles = Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);

            for(int n = 0; n < allfiles.Length; n++)
            {
                //// UNCOMMENT IF LOADING IS TAKING FOREVER
                //if (n > 100)
                //{
                //    break;
                //}

                double[] input = UC.ImageToInput(allfiles[n]);
                //don't normalize because we don't know the min and max yet of all items
                //Normalizer.ScaleToRange(input, 0, 255, 0, 1);
                result.Add(new TrainingItem(input, expectedOutput, expectedOutputIndex));
            }

            return result;
        }

        // folderPath should just contain a list of folders, the folder name is the label of the expected output
        public static List<TrainingItem> ClassifierTrainingSet(string folderPath)
        {
            List<TrainingItem> result = new List<TrainingItem>();

            string[] folders = Directory.GetDirectories(folderPath).OrderBy(s => s).ToArray();

            List<List<TrainingItem>> tempList = new List<List<TrainingItem>>();

            for(int n = 0; n < folders.Length; n++)
            {
                tempList.Add(null);
            }

            Parallel.For(0, folders.Length, n =>
            {
                double[] expectedOut = new double[folders.Length];

                expectedOut[n] = 1;

                tempList[n] = ClassifierTrainingSubset(expectedOut, (Int16)n, folders[n]);
            });


            for (int n = 0; n < folders.Length; n++)
            {
                result.AddRange(tempList[n]);
            }

            result.Shuffle(5);

            return result;
        }
    }
}
