using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolversLibrary.PI;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace SolverTests
{
    [TestClass]
    public class IPFTest
    {
        [TestMethod, TestCategory("PI")]
        public void Binning()
        {
            var data = new double[] { 1, 2, 3, 4, 5, 6 };
            var edges = new double[] { 0, 3, 6 };
            var ipf = new IPF();
            var result = ipf.BinIndicators(data, edges);
            var expected = new bool[,]
            {
                { true, false },
                { true, false },
                { true, false },
                { false, true },
                { false, true },
                { false, true }
            };
            Assert.AreEqual(expected.GetLength(0), result.GetLength(0));
            Assert.AreEqual(expected.GetLength(1), result.GetLength(1));
            CollectionAssert.AreEqual(expected, result);
        }
        [TestMethod, TestCategory("PI")]
        public void Binning_Inf()
        {
            var data = new double[] { 1, 2, 3, 4, 5, 6 };
            var edges = new double[] { double.NegativeInfinity, 3, double.PositiveInfinity };
            var ipf = new IPF();
            var result = ipf.BinIndicators(data, edges);
            var expected = new bool[,]
            {
                { true, false },
                { true, false },
                { true, false },
                { false, true },
                { false, true },
                { false, true }
            };
            Assert.AreEqual(expected.GetLength(0), result.GetLength(0));
            Assert.AreEqual(expected.GetLength(1), result.GetLength(1));
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod, TestCategory("PI")]
        public void Test_IPF()
        {
            int N = 100000;
            var originalData = new double[N];
            for (int i = 0; i < N; i++)
                originalData[i] = Normal.Sample(0.0, 1.0);
            var ipf = new IPFSparse();
            var data = new DataTable(new Dictionary<string, double[]> {
                { "x", originalData }
            });

            var targets = new Dictionary<string, Dictionary<double, double>>
            {
                { "x", new Dictionary<double, double>
                {
                    { 0.05, Normal.InvCDF(0.1, 0.2, 0.05) },
                    { 0.5, Normal.InvCDF(0.1, 0.2, 0.5) },
                    { 0.95, Normal.InvCDF(0.1, 0.2, 0.95) }
                } }
            };

            var weights = ipf.ComputeSampleWeights(data, targets, 1);

            var newSamples = ipf.Resample(data, weights);

            Assert.AreEqual(Normal.InvCDF(0.1, 0.2, 0.05), newSamples["x"].Quantile(0.05), 0.001);
            Assert.AreEqual(Normal.InvCDF(0.1, 0.2, 0.5), newSamples["x"].Quantile(0.5), 0.001);
            Assert.AreEqual(Normal.InvCDF(0.1, 0.2, 0.95), newSamples["x"].Quantile(0.95), 0.001);
        }

        /// <summary>
        /// Performs one variable update pass. We sample from a uniform distribution [0, 10].
        /// The resampled quantiles should be 0.25=2, 0.5=4 and 0.75=6.
        /// </summary>
        [TestMethod, TestCategory("PI")]
        public void IPF_VariableUpdate()
        {
            var ipf = new IPF();
            var data = new DataTable(new Dictionary<string, double[]> {
                { "x", new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } }
            });

            //var targets = new Dictionary<string, Dictionary<double, double>>
            //{
            //    { "x", new Dictionary<double, double>
            //    {
            //        { 0.25, 2 },
            //        { 0.5, 4 },
            //        { 0.75, 6 }
            //    } }
            //};
            var sampleIndicators = new bool[,]
            {
                { true, false, false, false }, // 0
                { true, false, false, false }, // 1
                { true, false, false, false }, // 2
                { false, true, false, false }, // 3
                { false, true, false, false }, // 4
                { false, false, true, false }, // 5
                { false, false, true, false }, // 6
                { false, false, false, true }, // 7
                { false, false, false, true }, // 8
                { false, false, false, true }, // 9
                { false, false, false, true } // 10
            };
            var interQuantiles = new double[] { 0.25, 0.25, 0.25, 0.25 };
            var startWeights = ipf.Uniform(11, 1.0 / 11.0);
            var weights = ipf.UpdateVariable(data, sampleIndicators, startWeights, interQuantiles);

            // Expected values are for each inter-quantile q_i: q_i / N_i, where N_i is the number of samples in that inter-quantile.
            var expected = new double[] {
                0.25 / 3, 0.25 / 3, 0.25 / 3, // q0
                0.25 / 2, 0.25 / 2, // q1
                0.25 / 2, 0.25 / 2, // q2
                0.25 / 4, 0.25 / 4, 0.25 / 4, 0.25 / 4 // q3
            };

            CollectionAssert.AreEqual(expected.Select(w => Math.Round(w, 6)).ToArray(), weights.Select(w => Math.Round(w, 6)).ToArray());
        }

        /// <summary>
        /// Performs one variable update pass. We sample from a uniform distribution [0, 10].
        /// The resampled quantiles should be 0.25=2, 0.5=4 and 0.75=6.
        /// </summary>
        [TestMethod, TestCategory("PI")]
        public void IPF_VariableUpdate_Sparse()
        {
            var ipf = new IPFSparse();
            var data = new DataTable(new Dictionary<string, double[]> {
                { "x", new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } }
            });

            var sampleIndicators = new SparseMatrix(11, 4);
            sampleIndicators[0, 0] = 1;
            sampleIndicators[1, 0] = 1;
            sampleIndicators[2, 0] = 1;
            sampleIndicators[3, 1] = 1;
            sampleIndicators[4, 1] = 1;
            sampleIndicators[5, 2] = 1;
            sampleIndicators[6, 2] = 1;
            sampleIndicators[7, 3] = 1;
            sampleIndicators[8, 3] = 1;
            sampleIndicators[9, 3] = 1;
            sampleIndicators[10, 3] = 1;

            var interQuantiles = new double[] { 0.25, 0.25, 0.25, 0.25 };
            var startWeights = ipf.Uniform(11, 1.0 / 11.0);
            var weights = ipf.UpdateVariable(data, sampleIndicators, startWeights, interQuantiles);

            // Expected values are for each inter-quantile q_i: q_i / N_i, where N_i is the number of samples in that inter-quantile.
            var expected = new double[] {
                0.25 / 3, 0.25 / 3, 0.25 / 3, // q0
                0.25 / 2, 0.25 / 2, // q1
                0.25 / 2, 0.25 / 2, // q2
                0.25 / 4, 0.25 / 4, 0.25 / 4, 0.25 / 4 // q3
            };

            CollectionAssert.AreEqual(expected.Select(w => Math.Round(w, 6)).ToArray(), weights.Select(w => Math.Round(w, 6)).ToArray());
        }
    }
}
