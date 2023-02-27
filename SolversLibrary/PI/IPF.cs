using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.Distributions;
using System.Diagnostics;

namespace SolversLibrary.PI
{
    /// <summary>
    /// Implementation of the Iterative Proportional Fitting algorithm for Probability Inversion
    /// </summary>
    public class IPF
    {
        public bool[,] BinIndicators(double[] values, double[] binEdges)
        {
            var binIndicators = new bool[values.Length, binEdges.Length - 1];

            for (int i = 0; i < values.Length; i++)
                for (int j = 1; j < binEdges.Length; j++)
                {
                    //var min = j == 0 ? double.NegativeInfinity : quantileValues[j - 1];
                    //var max = j == quantileValues.Length - 1 ? double.PositiveInfinity : quantileValues[j];
                    if (values[i] > binEdges[j - 1] && values[i] <= binEdges[j])
                        binIndicators[i, j - 1] = true;
                }
            return binIndicators;
        }

        public double[] Uniform(int N, double value)
        {
            var sampleWeights = new double[N];
            for (int i = 0; i < N; i++)
                sampleWeights[i] = value;
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
            //var quantiles = new Dictionary<string, Dictionary<double, double>>();
            var sampleIndicators = new Dictionary<string, bool[,]>();
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

                //quantiles.Add(variable, sortedQuantiles);

                // Indicator matrix of size (number of samples, number of inter-quantiles for this variable)
                var sampleIndicatorsForVariable = BinIndicators(samples[variable], sortedQuantileValues);

                sampleIndicators.Add(variable, sampleIndicatorsForVariable);
            }

            // Initialize sample weights
            var sampleWeights = Uniform(samples.NumberOfRows, 1.0 / (double)samples.NumberOfRows);

            // The IPF loop
            for (int iter = 0; iter < maxIter; iter++)
            {
                Debug.WriteLine($"Iteration {iter}");
                foreach (var variable in quantileConstraints.Keys.ToArray())
                {
                    sampleWeights = UpdateVariable(
                        samples,
                        sampleIndicators[variable],
                        sampleWeights,
                        variableInterQuantiles[variable]);
                }
            }
            return sampleWeights;
        }

        public double[] UpdateVariable(IDataTable samples, bool[,] sampleIndicators, double[] sampleWeights, double[] variableInterQuantiles)
        {
            double[] nextSampleWeights = new double[samples.NumberOfRows];
            double[] denominators = new double[variableInterQuantiles.Length];
            for (int i = 0; i < samples.NumberOfRows; i++)
                for (int j = 0; j < variableInterQuantiles.Length; j++)
                    if (sampleIndicators[i, j])
                        denominators[j] += sampleWeights[i];

            if (denominators.Any(d => d <= 0.0))
                throw new ArgumentOutOfRangeException(nameof(denominators));
            //double denominator = 0.0;
            //for (int k = 0; k < samples.NumberOfRows; k++)
            //    if (sampleIndicators[variable][k, j])
            //        denominator += sampleWeights[k];
            for (int i = 0; i < samples.NumberOfRows; i++)
            {
                for (int j = 0; j < variableInterQuantiles.Length; j++)
                {
                    if (sampleIndicators[i, j])
                    {
                        double nominator = sampleWeights[i] * variableInterQuantiles[j];
                        nextSampleWeights[i] += nominator / denominators[j];
                    }
                }
            }
            return nextSampleWeights;
        }

        public IDataTable Resample(IDataTable samples, double[] weights, int amount = 0)
        {
            if (amount <= 0)
                amount = samples.NumberOfRows;
            Multinomial mult = new Multinomial(weights, amount);
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
