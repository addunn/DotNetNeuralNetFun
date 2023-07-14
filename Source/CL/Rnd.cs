using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CL
{
    public class Rnd
    {

        // includes start and end
        public static int GetRandomInteger(double start, double end)
        {
            // not efficient
            return (int)Math.Round(GetRandomDouble(start - 0.5, end + 0.5), MidpointRounding.AwayFromZero);
        }


        // gets very close to minimum but never touches, same for maximum
        public static double GetRandomDouble(double minimum, double maximum)
        {
            // Step 1: fill an array with 8 random bytes
            var rng = new RNGCryptoServiceProvider();
            var bytes = new Byte[8];
            rng.GetBytes(bytes);
            // Step 2: bit-shift 11 and 53 based on double's mantissa bits
            var ul = BitConverter.ToUInt64(bytes, 0) / (1 << 11);
            Double d = ul / (Double)(1UL << 53);
            return d * (maximum - minimum) + minimum;
        }

    }
}
