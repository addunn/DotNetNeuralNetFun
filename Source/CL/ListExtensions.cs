using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL
{
    /// <summary>
    /// Helper methods for the lists.
    /// </summary>
    public static class ListExtensions
    {

        public static void Shuffle<T>(this IList<T> list, int amount = 1)
        {
            Random rnd = new Random();

            for (int m = 0; m < amount; m++)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = rnd.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }
        }
    }
}
