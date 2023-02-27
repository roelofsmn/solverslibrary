using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Threading.Tasks;

namespace SolversLibrary.PI
{
    /// <summary>
    /// Implementation of the Iterative Proportional Fitting algorithm for Probability Inversion
    /// </summary>
    public class IPFSparse
    {
        public List<int>[][] BinIndicators(IDataTable values, Dictionary<string, double[]> binEdges, string[] variables)
        {
            var binIndicators = new int[values.NumberOfRows, binEdges.Count];
            var quantileSampleIndicators = new List<int>[variables.Length][];
            
            Parallel.For(0, variables.Length, v =>
            {
                var variable = variables[v];
                quantileSampleIndicators[v] = new List<int>[binEdges[variable].Length - 1];
                for (int j = 1; j < binEdges[variable].Length; j++)
                    quantileSampleIndicators[v][j - 1] = new List<int>();

                for (int i = 0; i < values.NumberOfRows; i++)
                {
                    for (int j = 1; j < binEdges[variable].Length; j++)
                    {
                        if (values[variable][i] > binEdges[variable][j - 1] && values[variable][i] <= binEdges[variable][j])
                        {
                            binIndicators[i, v] = j - 1;
                            quantileSampleIndicators[v][j - 1].Add(i);
                        }
                    }
                }
            });
            return quantileSampleIndicators;
        }

        public double[] Uniform(int N, double value)
        {
            var sampleWeights = new double[N];
            Parallel.For(0, N, i =>
            {
                sampleWeights[i] = value;
            });
            return sampleWeights;
        }

        /// <summary>
        /// Performs the Iterative Proportional Fitting algorithm
        /// </summary>
        /// <param name="samples">A data table of samples that is reweighted</param>
        /// <param name="quantileConstraints">A set of constraints for columns of the samples. The constraints are the values at </param>
        /// <returns></returns>
        public double[] ComputeSampleWeights(IDataTable samples, IDictionary<string, Dictionary<double, double>> quantileConstraints, int maxIter = 100)
        {
            var variables = quantileConstraints.Keys.ToArray();

            //var quantiles = new Dictionary<string, Dictionary<double, double>>();
            var variableQuantileValues = new Dictionary<string, double[]>();
            var variableInterQuantiles = new Dictionary<string, double[]>();
            foreach (var variable in quantileConstraints.Keys.ToArray())
            {
                var quantilesForVariable = quantileConstraints[variable];
                quantilesForVariable.Add(0.0, double.NegativeInfinity);
                quantilesForVariable.Add(1.0, double.PositiveInfinity);
                var sortedQuantiles = quantilesForVariable.OrderBy(k => k.Key).Select(kvp => kvp.Key).ToArray();
                var sortedQuantileValues = quantilesForVariable.OrderBy(k => k.Key).Select(kvp => kvp.Value).ToArray();

                var sortedInterQuantiles = new double[sortedQuantiles.Length - 1];
                for (int i = 1; i < sortedQuantiles.Length; i++)
                    sortedInterQuantiles[i - 1] = sortedQuantiles[i] - sortedQuantiles[i - 1];
                variableInterQuantiles.Add(variable, sortedInterQuantiles);

                variableQuantileValues.Add(variable, sortedQuantileValues);
            }

            // Indicator matrix of size (number of samples, number of inter-quantiles for this variable)
            var sampleIndicators = BinIndicators(samples, variableQuantileValues, variables);

            // Initialize sample weights
            var sampleWeights = Uniform(samples.NumberOfRows, 1.0 / (double)samples.NumberOfRows);

            // The IPF loop
            for (int iter = 0; iter < maxIter; iter++)
            {
                Debug.WriteLine($"Iteration {iter}");
                for (int v = 0; v < variables.Length; v++)
                {
                    sampleWeights = UpdateVariable(
                        samples,
                        v,
                        sampleIndicators,
                        sampleWeights,
                        variableInterQuantiles[variables[v]]);
                }
            }
            return sampleWeights;
        }

        public double[] UpdateVariable(IDataTable samples, int variableIndex, List<int>[][] sampleIndicators, double[] sampleWeights, double[] variableInterQuantiles)
        {
            double[] nextSampleWeights = new double[samples.NumberOfRows];
            var denominators = new double[variableInterQuantiles.Length];

            Parallel.For(0, variableInterQuantiles.Length, q =>
            {
                foreach (var sample in sampleIndicators[variableIndex][q])
                    denominators[q] += sampleWeights[sample];
            });

            if (denominators.Any(d => d <= 0.0))
                throw new ArgumentOutOfRangeException(nameof(denominators));

            Parallel.For(0, variableInterQuantiles.Length, q => {
                var interQuantileSamples = sampleIndicators[variableIndex][q];
                foreach (var sample in interQuantileSamples)
                {
                    double nominator = sampleWeights[sample] * variableInterQuantiles[q];
                    nextSampleWeights[sample] += nominator / denominators[q];
                }
            });
            return nextSampleWeights;
        }

        public IDataTable Resample(IDataTable samples, double[] weights, int amount = 0, Random rnd = null)
        {
            if (amount <= 0)
                amount = samples.NumberOfRows;
            Multinomial mult = new Multinomial(weights, amount, rnd);
            var indexCounts = mult.Sample();

            DataTable newSamples = new DataTable(samples.ColumnNames);
            for (int i = 0; i < amount; i++)
            {
                for (int k = 0; k < indexCounts[i]; k++)
                    newSamples.AddRow(samples.GetRow(i));
            }

            return newSamples;
        }
    }

}
