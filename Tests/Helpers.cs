using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal static class Helpers
    {
        public static void CheckArray(double[,] expected, double[,] actual, int precision = 4)
        {
            for (int i = 0; i < expected.GetLength(0); i++)
                for (int j = 0; j < expected.GetLength(1); j++)
                {
                    Assert.Equal(expected[i, j], actual[i, j], precision, MidpointRounding.AwayFromZero);
                }
        }
        public static void CheckArray(double[] expected, double[] actual, int precision = 4)
        {
            for (int i = 0; i < expected.GetLength(0); i++)
            {
                Assert.Equal(expected[i], actual[i], precision, MidpointRounding.AwayFromZero);
            }
        }

    }
}
