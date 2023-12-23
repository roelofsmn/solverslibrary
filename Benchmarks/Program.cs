// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using Benchmarks;

void PrintArray(double[] array)
{
    Console.WriteLine(string.Join(", ", array));
}

var b = new SubmatrixBenchmark();
var output = b.GetSubMatrix_array_unsafe(
    new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
    new int[] { 3, 3 },
    1,
    0,
    1,
    3);
PrintArray(output);
output = b.GetSubMatrix_array_unsafe(
    new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
    new int[] { 3, 3 },
    0,
    1,
    3,
    1);
PrintArray(output);
var x = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
var outputSpan = b.GetSubMatrix_array_span(
    x,
    new int[] { 3, 3 },
    0,
    1,
    3,
    1);
PrintArray(outputSpan.ToArray());
//var summary = BenchmarkRunner.Run<SubmatrixBenchmark>();

BenchmarkRunner.Run<StackBenchmark>();