using SolversLibrary.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class MixedIntegerProgrammingTest
    {
        [Fact]
        public void SolveProblem85()
        {
            double[] c = new double[] { -1, -2, -3, -1.5 };
            double[,] A = new double[,]
            {
                { 1, 1, 2, 2 },
                { 0, 0, 0, -1 },
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
            };
            double[] b = new double[] { 10, 0, 0, 0, 0 };

            double[,] Aeq = new double[,]
            {
                { 7, 8, 5, 1 }
            };
            double[] beq = new double[] { 31.5 };
            int[] inds = new int[] { 0, 1, 2 };

            var solver = new LinearMixedIntegerProgramming(A, b, c, inds, Aeq, beq);

            // Act
            var solution = solver.Solve(SolversLibrary.Search.Factories.TraversalStrategies.BreadthFirst);
            var objVal = solver.ComputeObjective(solution);

            // Assert
            Helpers.CheckArray(new double[] { 0, 2, 3, 0.5 }, solution);
            Assert.Equal(-13.75, objVal, 6, MidpointRounding.AwayFromZero);
        }

        [Fact]
        public void CreateNewProblemWithExtraConstraint_LessThanOrEqualTo()
        {
            double[] x = new double[] { 0, 1.1818, 4.4091, 0 };
            double[,] A = new double[,]
            {
                { 1, 1, 2, 2 },
                { 0, 0, 0, -1 },
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
            };
            double[] b = new double[] { 10, 0, 0, 0, 0 };
            var state = new MixedIntegerProblemState(A, b, x);

            // Act
            var newState = MixedIntegerProblemState.CreateFrom(state, (2, 4, true)); // Add x3 <= 4

            // Assert
            var Anew = new double[,]
            {
                { 1, 1, 2, 2 },
                { 0, 0, 0, -1 },
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
                { 0, 0, 1, 0 }
            };
            double[] bnew = new double[] { 10, 0, 0, 0, 0, 4 };
            //double[] xnew = new double[] { 0, 1.1818, 4, 0 };

            Assert.Equal(Anew, newState.A);
            Assert.Equal(bnew, newState.b);
            //Assert.Equal(xnew, newState.x);
        }

        [Fact]
        public void CreateNewProblemWithExtraConstraint_GreaterThanOrEqualTo()
        {
            double[] x = new double[] { 0, 1.1818, 4.4091, 0 };
            double[,] A = new double[,]
            {
                { 1, 1, 2, 2 },
                { 0, 0, 0, -1 },
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
            };
            double[] b = new double[] { 10, 0, 0, 0, 0 };
            var state = new MixedIntegerProblemState(A, b, x);

            // Act
            var newState = MixedIntegerProblemState.CreateFrom(state, (2, 5, false)); // Add x3 >= 5

            // Assert
            var Anew = new double[,]
            {
                { 1, 1, 2, 2 },
                { 0, 0, 0, -1 },
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
                { 0, 0, -1, 0 }
            };
            double[] bnew = new double[] { 10, 0, 0, 0, 0, -5 };
            //double[] xnew = new double[] { 0, 1.1818, 5, 0 };

            Assert.Equal(Anew, newState.A);
            Assert.Equal(bnew, newState.b);
            //Assert.Equal(xnew, newState.x);
        }
    }
}
