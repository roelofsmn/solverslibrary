using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solvers.Extensions;

namespace SolverTests
{

    [TestClass]
    public class ExtensionsTest
    {
        private void AssertEqualMatrix(double[,] expected, double[,] matrix)
        {
            Assert.Equals(expected.GetLength(0), matrix.GetLength(0));
            Assert.Equals(expected.GetLength(1), matrix.GetLength(1));

            for (int i = 0; i < expected.GetLength(0); i++)
                for (int j = 0; j < expected.GetLength(1); j++)
                    Assert.Equals(expected[i, j], matrix[i, j]);
        }

        [TestCategory("Matrix"), TestMethod]
        public void TestSubtractFromRows()
        {
            double[,] matrix = new double[,]
            {
                { 1, 2, 3 },
                { 5, 11, 100 }
            };
            double[] values = new double[] { 5, 7 };

            double[,] expected = new double[,]
            {
                { -4, -3, -2 },
                { -2, 4, 93 }
            };

            var result = matrix.SubtractFromRows(values);

            AssertEqualMatrix(expected, result);
        }
    }
}
