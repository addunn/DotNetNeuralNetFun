using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using System.Threading.Tasks;
using System.Drawing;
using System.Xml.Serialization;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.Threading;
using System.Collections.Specialized;

namespace CL
{
    public class UC
    {

        public static long GetFolderSize(string s)
        {
            string[] fileNames = Directory.GetFiles(s, "*.*");

            long size = 0;

            // Calculate total size by looping through files in the folder and totalling their sizes
            foreach (string name in fileNames)
            {
                // length of each file.
                FileInfo details = new FileInfo(name);
                size += details.Length;
            }

            return size;
        }


        public static string NameValueCollectionToString(NameValueCollection nvc)
        {
            return String.Join("&", nvc.AllKeys.Select(a => a + "=" + nvc[a]));
        }
        public static string DownloadString(string url)
        {
            // make this better
            return new WebClient().DownloadString(url);
        }

        public static string DownloadStringAsync(string url)
        {
            string result = "";

            bool tryAgain = false;

            try
            {
                Task<string> t = new WebClient().DownloadStringTaskAsync(url);
                result = t.Result;
            }
            catch (Exception)
            {
                tryAgain = true;
            }

            while (tryAgain)
            {

                Thread.Sleep(5000);

                tryAgain = false;

                try
                {
                    Task<string> t = new WebClient().DownloadStringTaskAsync(url);
                    result = t.Result;
                }
                catch (Exception)
                {
                    tryAgain = true;
                }
            }

            return result;
        }
        public static string ListToString(List<string> source, string delimeter)
        {
            return String.Join(delimeter, source);
        }

        public static List<string> MultilineStringToList(string source)
        {
            List<string> result = new List<string>();
            using (StringReader reader = new StringReader(source))
            {

                string line = "";

                while ((line = reader.ReadLine()) != null)
                {
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        result.Add(line);
                    }
                }
            }
            return result;
        }


        public static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(sourceBMP, 0, 0, width, height);
            }
            return result;
        }

        public static void ResizeImage(string path, string originalFilename,
                     /* note changed names */
                     int canvasWidth, int canvasHeight,
                     /* new */
                     int originalWidth, int originalHeight)
        {
            Image image = Image.FromFile(path + originalFilename);

            System.Drawing.Image thumbnail =
                new Bitmap(canvasWidth, canvasHeight); // changed parm names
            System.Drawing.Graphics graphic =
                         System.Drawing.Graphics.FromImage(thumbnail);

            graphic.InterpolationMode = InterpolationMode.Low;
            graphic.SmoothingMode = SmoothingMode.None;
            graphic.PixelOffsetMode = PixelOffsetMode.None;
            graphic.CompositingQuality = CompositingQuality.AssumeLinear;

            /* ------------------ new code --------------- */

            // Figure out the ratio
            double ratioX = (double)canvasWidth / (double)originalWidth;
            double ratioY = (double)canvasHeight / (double)originalHeight;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - (originalWidth * ratio)) / 2);
            int posY = Convert.ToInt32((canvasHeight - (originalHeight * ratio)) / 2);

            graphic.Clear(Color.White); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);

            /* ------------- end new code ---------------- */

            System.Drawing.Imaging.ImageCodecInfo[] info =
                             ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;
            encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            thumbnail.Save(path + newWidth + "." + originalFilename, info[1],
                             encoderParameters);
        }

        public static int DeleteFoldersAndFiles(string folderPath)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);

            int itemsDeleted = 0;

            foreach (FileInfo file in di.GetFiles())
            {
                itemsDeleted++;
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                itemsDeleted++;
                dir.Delete(true);
            }

            return itemsDeleted;
        }

        public static string[] ReadFileLinesToArray(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            return lines;
        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int min = 0;
            int max = chars.Length - 1;
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Rnd.GetRandomInteger(min, max)]).ToArray());
        }
        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the binary file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the binary file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite)
        {
            using (Stream stream = File.Open(filePath, FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the binary file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
        /// <summary>
        /// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
        /// Provides a method for performing a deep copy of an object.
        /// Binary Serialization is used to perform the copy.
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", nameof(source));
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
        public static double[] ImageToInput(string path)
        {
            Bitmap img = new Bitmap(path);

            int h = img.Height;
            int w = img.Width;

            double[] result = new double[w * h];

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    Color oc = img.GetPixel(j, i);
                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    result[(i * w) + j] = (double)grayScale;
                }
            }
            return result;
        }

    }

    
}
