using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solvers.Extensions
{
    public static class MatrixExtensions
    {
        public static double[,] SubtractFromRows(this double[,] matrix, double[] rowValues)
        {
            var result = new double[matrix.GetLength(0), matrix.GetLength(1)];
            if (matrix.GetLength(0) != rowValues.Length)
                throw new ArgumentException("Argument rowValues must have length equal to number of rows in matrix.");
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    result[i, j] = matrix[i, j] - rowValues[i];

            return result;
        }
        public static double[,] SubtractFromColumns(this double[,] matrix, double[] columnValues)
        {
            var result = new double[matrix.GetLength(0), matrix.GetLength(1)];
            if (matrix.GetLength(1) != columnValues.Length)
                throw new ArgumentException("Argument columnValues must have length equal to number of columns in matrix.");
            for (int i = 0; i < matrix.GetLength(0); i++)
                for (int j = 0; j < matrix.GetLength(1); j++)
                    result[i, j] = matrix[i, j] - columnValues[j];

            return result;
        }

        public static double[] RowMinima(this double[,] matrix)
        {
            var result = new double[matrix.GetLength(0)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                double minimum = double.PositiveInfinity;
                for (int j = 0; j < matrix.GetLength(1); j++)
                    if (matrix[i, j] < minimum)
                        minimum = matrix[i, j];
                result[i] = minimum;
            }
            return result;
        }
        public static double[] ColumnMinima(this double[,] matrix)
        {
            var result = new double[matrix.GetLength(1)];
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                double minimum = double.PositiveInfinity;
                for (int i = 0; i < matrix.GetLength(0); i++)
                    if (matrix[i, j] < minimum)
                        minimum = matrix[i, j];
                result[j] = minimum;
            }
            return result;
        }
    }
}
