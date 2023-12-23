using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    [MemoryDiagnoser(false)]
    public class SubmatrixBenchmark
    {
        //private double[,] data;

        //private ITensor matrix;
        //private ITensor matrixParallel;

        private Random rnd;

        public SubmatrixBenchmark()
        {
            rnd = new Random(42);
            //matrix = new Matrix(data);
            //matrixParallel = new MatrixParallel(data);
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public double[,] GetSubMatrix(double[,] array, int minI, int minJ, int H, int W)
        {
            var result = new double[H, W];
            for (int i = minI; i < minI + H; i++)
                for (int j = minJ; j < minJ + W; j++)
                    result[i - minI, j - minJ] = array[i, j];
            return result;
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public double[,] GetSubMatrix_Unsafe(double[,] array, int minI, int minJ, int H, int W)
        {
            var result = new double[H, W];
            unsafe
            {
                fixed (double* arrayPointer = &array[0, 0])
                {
                    for (int i = minI; i < minI + H; i++)
                        for (int j = minJ; j < minJ + W; j++)
                            result[i - minI, j - minJ] = arrayPointer[i * array.GetLength(0) + j];
                }
            }

            return result;
        }

        [Benchmark]
        [ArgumentsSource(nameof(DataArray))]
        public double[] GetSubMatrix_array_unsafe(double[] array, int[] shape, int minI, int minJ, int H, int W)
        {
            var result = new double[H * W];
            unsafe
            {
                fixed (double* arrayPointer = &array[0])
                {
                    for (int i = minI; i < minI + H; i++)
                        for (int j = minJ; j < minJ + W; j++)
                            result[(i - minI) * W + j - minJ] = arrayPointer[i * shape[1] + j];
                }
            }

            return result;
        }

        [Benchmark]
        [ArgumentsSource(nameof(DataArray))]
        public Span<double> GetSubMatrix_array_span(double[] array, int[] shape, int minI, int minJ, int H, int W)
        {
            Span<double> arraySpan = array.AsSpan();
            Span<double> resultSpan = stackalloc double[H * W];

            for (int i = minI; i < minI + H; i++)
                for (int j = minJ; j < minJ + W; j++)
                    resultSpan[(i - minI) * W + j - minJ] = arraySpan[i * shape[1] + j];

            return resultSpan.ToArray();
        }

        [Benchmark]
        [ArgumentsSource(nameof(DataArray))]
        public double[] GetSubMatrix_array_span2(double[] array, int[] shape, int minI, int minJ, int H, int W)
        {
            Span<double> arraySpan = array.AsSpan();
            double[] resultSpan = new double[H * W];

            for (int i = minI; i < minI + H; i++)
                for (int j = minJ; j < minJ + W; j++)
                    resultSpan[(i - minI) * W + j - minJ] = arraySpan[i * shape[1] + j];

            return resultSpan;
        }

        private double[,] CreateData(int n)
        {
            Random rnd = new Random(42);
            var data = new double[n, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    data[i, j] = rnd.NextDouble();
            return data;
        }

        private double[] CreateDataArray_RowBase(int n)
        {
            Random rnd = new Random(42);
            var data = new double[n * n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    data[i * n + j] = rnd.NextDouble();
            }
            return data;
        }

        private double[] CreateDataArray_ColBase(int n)
        {
            Random rnd = new Random(42);
            var data = new double[n * n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    data[i + j * n] = rnd.NextDouble();
            }
            return data;
        }

        public IEnumerable<object[]> Data() // for single argument it's an IEnumerable of objects (object)
        {
            yield return new object[] { CreateData(50), 1, 0, 1, 50 }; // get second row
            yield return new object[] { CreateData(50), 0, 1, 50, 1 }; // get second column
        }

        public IEnumerable<object[]> DataArray() // for single argument it's an IEnumerable of objects (object)
        {
            yield return new object[] { CreateDataArray_RowBase(50), new int[] { 50, 50 }, 1, 0, 1, 50 }; // get second row
            yield return new object[] { CreateDataArray_RowBase(50), new int[] { 50, 50 }, 0, 1, 50, 1 }; // get second column
        }

        public IEnumerable<object[]> DataArrayColumnBase() // for single argument it's an IEnumerable of objects (object)
        {
            yield return new object[] { CreateDataArray_ColBase(50), new int[] { 50, 50 }, 1, 0, 1, 50 }; // get second row
            yield return new object[] { CreateDataArray_ColBase(50), new int[] { 50, 50 }, 0, 1, 50, 1 }; // get second column
        }
    }
}
