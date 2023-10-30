using Solvers.Extensions;

namespace Tests
{
    public class ExtensionsTest
    {
        private void AssertEqualMatrix(double[,] expected, double[,] matrix)
        {
            Assert.Equal(expected.GetLength(0), matrix.GetLength(0));
            Assert.Equal(expected.GetLength(1), matrix.GetLength(1));

            for (int i = 0; i < expected.GetLength(0); i++)
                for (int j = 0; j < expected.GetLength(1); j++)
                    Assert.Equal(expected[i, j], matrix[i, j]);
        }

        [Fact]
        [Trait("Category", "Matrix")]
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
        [Fact]
        [Trait("Category", "Matrix")]
        public void TestSubtractFromRows_InvalidValues()
        {
            double[,] matrix = new double[,]
            {
                { 1, 2, 3 },
                { 5, 11, 100 }
            };
            double[] values = new double[] { 5, 7, 8 };

            Assert.Throws<ArgumentException>(() => matrix.SubtractFromRows(values));
        }
        [Fact]
        [Trait("Category", "Matrix")]
        public void TestSubtractFromColumns()
        {
            double[,] matrix = new double[,]
            {
                { 1, 2, 3 },
                { 5, 11, 100 }
            };
            double[] values = new double[] { 5, 7, 8 };

            double[,] expected = new double[,]
            {
                { -4, -5, -5 },
                { 0, 4, 92 }
            };

            var result = matrix.SubtractFromColumns(values);

            AssertEqualMatrix(expected, result);
        }
        [Fact]
        [Trait("Category", "Matrix")]
        public void TestSubtractFromColumns_InvalidValues()
        {
            double[,] matrix = new double[,]
            {
                { 1, 2, 3 },
                { 5, 11, 100 }
            };
            double[] values = new double[] { 5, 7 };

            Assert.Throws<ArgumentException>(() => matrix.SubtractFromColumns(values));
        }

        [Fact]
        [Trait("Category", "Matrix")]
        public void TestRowMinima()
        {
            double[,] matrix = new double[,]
            {
                { 1, 2, 3 },
                { 5, 11, 100 }
            };
            
            double[] expected = new double[] { 1, 5 };

            Assert.Equal(expected, matrix.RowMinima());
        }

        [Fact]
        [Trait("Category", "Matrix")]
        public void TestColumnMinima()
        {
            double[,] matrix = new double[,]
            {
                { 1, 2, 3 },
                { 5, 11, 100 }
            };

            double[] expected = new double[] { 1, 2, 3 };

            Assert.Equal(expected, matrix.ColumnMinima());
        }
    }
}
