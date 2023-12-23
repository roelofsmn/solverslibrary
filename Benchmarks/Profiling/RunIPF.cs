using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using SolversLibrary.PI;

namespace Benchmarks.Profiling
{
    internal static class RunIPF
    {
        public static void Run()
        {
            int N = 100000;
            var originalData = new double[N];
            var random = new Random(69);
            for (int i = 0; i < N; i++)
                originalData[i] = ContinuousUniform.Sample(random, 0.0, 1.0);
            var ipf = new IPFSparse();
            var data = new DataTable(new Dictionary<string, double[]> {
                { "x", originalData }
            });

            var targets = new Dictionary<string, Dictionary<double, double>>
            {
                { "x", new Dictionary<double, double>
                {
                    { 0.05, ContinuousUniform.InvCDF(0.1, 0.2, 0.05) },
                    { 0.5, ContinuousUniform.InvCDF(0.1, 0.2, 0.5) },
                    { 0.95, ContinuousUniform.InvCDF(0.1, 0.2, 0.95) }
                } }
            };

            var weights = ipf.ComputeSampleWeights(data, targets, 100);

            var newSamples = ipf.Resample(data, weights, rnd: random);


            Console.WriteLine($"Quantile 5 - requested: {ContinuousUniform.InvCDF(0.1, 0.2, 0.05)}, actual: {newSamples["x"].Quantile(0.05)}");
            Console.WriteLine($"Quantile 5 - requested: {ContinuousUniform.InvCDF(0.1, 0.2, 0.5)}, actual: {newSamples["x"].Quantile(0.5)}");
            Console.WriteLine($"Quantile 5 - requested: {ContinuousUniform.InvCDF(0.1, 0.2, 0.95)}, actual: {newSamples["x"].Quantile(0.95)}");
            //Assert.Equal(ContinuousUniform.InvCDF(0.1, 0.2, 0.5), newSamples["x"].Quantile(0.5), 0.01);
            //Assert.Equal(ContinuousUniform.InvCDF(0.1, 0.2, 0.95), newSamples["x"].Quantile(0.95), 0.01);
        }
    }
}
