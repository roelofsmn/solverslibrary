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
    public class MultiplyBenchmark
    {
        //private double[,] data;

        //private ITensor matrix;
        //private ITensor matrixParallel;

        private Random rnd;

        public MultiplyBenchmark()
        {
            rnd = new Random(42);
            //matrix = new Matrix(data);
            //matrixParallel = new MatrixParallel(data);
        }

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public double[,] Multiply_Single(double[,] left)
        {
            var result = new double[left.GetLength(0), left.GetLength(1)];

            for (int i = 0; i < left.GetLength(0); i++)
                for (int j = 0; j < left.GetLength(1); j++)
                    for (int n = 0; n < left.GetLength(1); n++)
                        result[i, j] += left[i, n] * left[n, j];

            return result;
        }

        //[Benchmark]
        //[ArgumentsSource(nameof(Data))]
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public double[,] Multiply_Single_Inline(double[,] left)
        //{
        //    var result = new double[left.GetLength(0), left.GetLength(1)];

        //    for (int i = 0; i < left.GetLength(0); i++)
        //        for (int j = 0; j < left.GetLength(1); j++)
        //            for (int n = 0; n < left.GetLength(1); n++)
        //                result[i, j] += left[i, n] * left[n, j];

        //    return result;
        //}

        [Benchmark]
        [ArgumentsSource(nameof(Data))]
        public unsafe double[,] Multiply_Single_Unsafe(double[,] left)
        {
            var result = new double[left.GetLength(0), left.GetLength(1)];

            fixed (double* resultPointer = &result[0, 0])
            fixed (double* arrayPointer = &left[0, 0])
            {
                for (int i = 0; i < left.GetLength(0); i++)
                    for (int j = 0; j < left.GetLength(1); j++)
                        for (int n = 0; n < left.GetLength(1); n++)
                            resultPointer[i * left.GetLength(0) + j] += arrayPointer[i * left.GetLength(0) + n ] * arrayPointer[n * left.GetLength(0) + j];
            }

            return result;
        }

        //[Benchmark]
        //[ArgumentsSource(nameof(DataJagged))]
        //public double[][] Multiply_Jagged(double[][] left)
        //{
        //    var result = new double[left.Length][];

        //    for (int i = 0; i < left.Length; i++)
        //    {
        //        result[i] = new double[left[i].Length];
        //        for (int j = 0; j < left[i].Length; j++)
        //            for (int n = 0; n < left[i].Length; n++)
        //                result[i][j] += left[i][n] * left[n][j];
        //    }

        //    return result;
        //}

        //[Benchmark]
        //[ArgumentsSource(nameof(DataJagged))]
        //public unsafe double[][] Multiply_Jagged_Unsafe(double[][] left)
        //{
        //    var result = new double[left.Length][];

        //    fixed (double* arrayPointer = &left[0][0])
        //    {
        //        for (int i = 0; i < left.Length; i++)
        //        {
        //            result[i] = new double[left[i].Length];
        //            fixed (double* resultPointer = &result[i][0])
        //            {
        //                for (int j = 0; j < left[i].Length; j++)
        //                    for (int n = 0; n < left[i].Length; n++)
        //                        resultPointer[j] += arrayPointer[i * left.Length + n] * arrayPointer[n * left.Length + j];
        //            }
        //        }
        //    }

        //    return result;
        //}

        [Benchmark]
        [ArgumentsSource(nameof(DataArray))]
        public double[] Multiply_Array(double[] data, int[] shape)
        {
            var result = new double[data.Length];

            for (int i = 0; i < shape[0]; i++)
                for (int j = 0; j < shape[1]; j++)
                    for (int n = 0; n < shape[1]; n++)
                        result[i * shape[0] + j] += data[i * shape[0] + n] * data[n * shape[0] + j];

            return result;
        }

        [Benchmark]
        [ArgumentsSource(nameof(DataArray))]
        public unsafe double[] Multiply_Array_Unsafe(double[] data, int[] shape)
        {
            var result = new double[data.Length];

            fixed (double* resultPointer = &result[0])
            fixed (double* arrayPointer = &data[0])
            {
                for (int i = 0; i < shape[0]; i++)
                    for (int j = 0; j < shape[1]; j++)
                        for (int n = 0; n < shape[1]; n++)
                            resultPointer[i * shape[0] + j] += arrayPointer[i * shape[0] + n] * arrayPointer[n * shape[0] + j];
            }

            return result;
        }
        //[Benchmark]
        //[ArgumentsSource(nameof(Data))]
        //public double[,] Multiply_Parallel(double[,] left)
        //{
        //    if (left.GetLength(1) != left.GetLength(0))
        //        throw new ArgumentException("Left and right matrices do not have congruent sizes. Columns left should equal rows right.");

        //    var intermediate = new double[left.GetLength(0), left.GetLength(1), left.GetLength(1)];

        //    Parallel.For(0, left.GetLength(1), (n) => {
        //        for (int i = 0; i < left.GetLength(0); i++)
        //            for (int j = 0; j < left.GetLength(1); j++)
        //                intermediate[i, j, n] += left[i, n] * left[n, j];
        //    });
        //    var result = new double[left.GetLength(0), left.GetLength(1)];
        //    Parallel.For(0, left.GetLength(0), (i) =>
        //    {
        //        for (int j = 0; j < left.GetLength(1); j++)
        //            for (int n = 0; n < left.GetLength(1); n++)
        //                result[i, j] += intermediate[i, j, n];
        //    });

        //    return result;
        //}

        private double[,] CreateData(int n)
        {
            Random rnd = new Random(42);
            var data = new double[n, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    data[i, j] = rnd.NextDouble();
            return data;
        }

        private double[][] CreateDataJagged(int n)
        {
            Random rnd = new Random(42);
            var data = new double[n][];

            for (int i = 0; i < n; i++)
            {
                data[i] = new double[n];
                for (int j = 0; j < n; j++)
                    data[i][j] = rnd.NextDouble();
            }
            return data;
        }
        private double[] CreateDataArray(int n)
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

        public IEnumerable<object> Data() // for single argument it's an IEnumerable of objects (object)
        {
            yield return CreateData(5);
            yield return CreateData(10);
            yield return CreateData(50);
        }

        public IEnumerable<object> DataJagged() // for single argument it's an IEnumerable of objects (object)
        {
            yield return CreateDataJagged(5);
            yield return CreateDataJagged(10);
            yield return CreateDataJagged(50);
        }

        public IEnumerable<object[]> DataArray() // for single argument it's an IEnumerable of objects (object)
        {
            yield return new object[] { CreateDataArray(5), new int[] { 5, 5 } };
            yield return new object[] { CreateDataArray(10), new int[] { 10, 10 } };
            yield return new object[] { CreateDataArray(50), new int[] { 50, 50 } };
        }
    }
}
