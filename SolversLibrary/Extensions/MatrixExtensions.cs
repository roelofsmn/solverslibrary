using MathNet.Numerics.LinearAlgebra;
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

        public static T[] GetRow<T>(this T[,] array, int row)
        {
            var N = array.GetLength(1);
            var output = new T[N];
            for (int j = 0; j < N; j++)
                output[j] = array[row, j];

            return output;
        }
        public static T[] GetColumn<T>(this T[,] array, int column)
        {
            var N = array.GetLength(0);
            var output = new T[N];
            for (int j = 0; j < N; j++)
                output[j] = array[j, column];

            return output;
        }

        public static Vector<double> InsertAt(this Vector<double> vector, int index, double value)
        {
            double[] newValues = new double[vector.Count + 1];
            for (int i = 0; i < vector.Count; i++) // Copy current elements
            {
                int offset = i < index ? 0 : 1;
                newValues[i + offset] = vector[i];
            }
            newValues[index] = value;
            return Vector<double>.Build.DenseOfArray(newValues);
        }
        public static Vector<double> RemoveAt(this Vector<double> vector, int element)
        {
            double[] newValues = new double[vector.Count - 1];
            for (int i = 0; i < vector.Count; i++)
            {
                if (i == element)
                    continue;
                int offset = i < element ? 0 : 1;
                newValues[i - offset] = vector[i];
            }
            return Vector<double>.Build.DenseOfArray(newValues);
        }


    }
}
