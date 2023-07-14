using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML
{
    public interface IActivationFunction
    {
        double Id();
        double Run(double d);
        double Derivative(double d);
        double Reverse(double d);
    }

    public class ActivationFunctions
    {

        [Serializable]
        public class Step : IActivationFunction
        {
            public double Id()
            {
                return 1;
            }
            public double Run(double d)
            {
                if (d > 0)
                {
                    return 1;
                }
                else if (d < 0)
                {
                    return 0;
                }
                else
                {
                    return 0.5;
                }
            }
            public double Derivative(double d)
            {
                if (d == 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            public double Reverse(double d)
            {
                if (d >= 1)
                {
                    return double.PositiveInfinity;
                }
                else if (d <= 0)
                {
                    return double.NegativeInfinity;
                }
                else
                {
                    return 0;
                }
            }
        }
        [Serializable]
        public class TanH : IActivationFunction
        {
            public double Id()
            {
                return 2;
            }
            public double Run(double d)
            {
                if (d < -400)
                {
                    return -0.999999999999999;
                }
                else if (d > 400)
                {
                    return 0.9999999999999999;
                }
                return (Math.Pow(Math.E, d) - Math.Pow(Math.E, -d)) / (Math.Pow(Math.E, d) + Math.Pow(Math.E, -d));
            }
            public double Derivative(double d)
            {
                return (1 - Math.Pow(Run(d), 2));
            }
            public double Reverse(double d)
            {
                return ((0.5) * Math.Log((1 + d) / (1 - d)));
            }
        }
        [Serializable]
        public class Sigmoid : IActivationFunction
        {
            public double Id()
            {
                return 3;
            }
            public double Run(double d)
            {
                if (d < -400)
                {
                    return 0.0000000000000001;
                }
                else if (d > 400)
                {
                    return 0.9999999999999999;
                }
                return 1 / (1 + (Math.Pow(Math.E, -d)));
            }
            public double Derivative(double d)
            {
                double r = Run(d);
                return (r * (1 - r));
            }
            public double Reverse(double d)
            {
                if (d <= 0)
                {
                    d = 0.0000000000000001;
                }
                else if (d >= 1)
                {
                    d = 0.9999999999999999;
                }
                return Math.Log(d / (1 - d));
            }
        }
        [Serializable]
        public class LeakyReLU : IActivationFunction
        {
            public double Id()
            {
                return 4;
            }
            public double Run(double d)
            {
                if (d > 0)
                {
                    return d;
                }
                else
                {
                    return d * 0.01;
                }
            }
            public double Derivative(double d)
            {
                if (d > 0)
                {
                    return 1;
                }
                else
                {
                    return 0.01;
                }
            }
            public double Reverse(double d)
            {
                if (d > 0)
                {
                    return d;
                }
                else
                {
                    return d / 0.01;
                }
            }
        }
        [Serializable]
        public class ReLU : IActivationFunction
        {
            public double Id()
            {
                return 5;
            }
            public double Run(double d)
            {
                if (d > 0)
                {
                    return d;
                }
                else
                {
                    return 0;
                }
            }
            public double Derivative(double d)
            {
                if (d > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            public double Reverse(double d)
            {
                if (d > 0)
                {
                    return d;
                }
                else if (d == 0)
                {
                    return 0;
                }
                else // less than 0
                {
                    return Double.NegativeInfinity;
                }
            }
        }

    }
}
