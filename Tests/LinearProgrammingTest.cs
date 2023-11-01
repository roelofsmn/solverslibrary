using SolversLibrary.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace Tests
{
    public class LinearProgrammingTest
    {
        [Fact]
        [Trait("Category", "Linear Programming")]
        public void Problem512()
        {
            // Problem from Principles of Optimal Design, Papalambros and Wilde, 2nd edition, p.203-204
            /* Minimize -2x1 - x2
             * Subject to:
             * g1: x1 + 2x2 - 8 <= 0
             * g2: x1 - x2 - 1.5 <= 0
             * g3: -2 x1 + 1 <= 0
             * g4: -2 x2 + 1 <= 0
             */

            // From the minimization function we have:
            double[,] A =
            {
                { 1, 2 },
                { 1, -1 },
                { -2, 0 },
                { 0, -2 }
            };
            double[] b = { 8, 1.5, -1, -1 };
            double[] c = { -2, -1 };

            var x_actual = new double[] { 3.67, 2.17 };

            var x_optimal = LinearProgramming.Solve(A, b, c)[0];

            Assert.Equal(2, x_optimal.Length);
            Assert.Equal(x_actual[0], x_optimal[0], 2, MidpointRounding.AwayFromZero);
            Assert.Equal(x_actual[1], x_optimal[1], 2, MidpointRounding.AwayFromZero);
        }

        [Fact]
        [Trait("Category", "Linear Programming")]
        public void Problem513_Cycling_ShouldReturnMultipleSolutions()
        {
            // Problem from Principles of Optimal Design, Papalambros and Wilde, 2nd edition, p.205-206
            /* Minimize -2x1 - 4 x2
             * Subject to:
             * g1: x1 + 2x2 - 8 <= 0
             * g2: x1 - x2 - 1.5 <= 0
             * g3: -2 x1 + 1 <= 0
             * g4: -2 x2 + 1 <= 0
             */

            // From the minimization function we have:
            double[,] A =
            {
                { 1, 2 },
                { 1, -1 },
                { -2, 0 },
                { 0, -2 }
            };
            double[] b = { 8, 1.5, -1, -1 };
            double[] c = { -2, -4 };

            var x_actual = new double[][] {
                new double[] { 0.5, 3.75 },
                new double[] { 3.67, 2.17 }
            };

            var x_optimal = LinearProgramming.Solve(A, b, c);

            Assert.Equal(2, x_optimal.Length);
            foreach (var solution in x_actual.Zip(x_optimal))
            {
                Assert.Equal(solution.First[0], solution.Second[0], 2, MidpointRounding.AwayFromZero);
                Assert.Equal(solution.First[1], solution.Second[1], 2, MidpointRounding.AwayFromZero);
            }
        }

        [Fact]
        [Trait("Category", "Linear Programming")]
        public void Problem516_EqualityConstraint()
        {
            // Problem from Principles of Optimal Design, Papalambros and Wilde, 2nd edition, p.212-213
            /* Minimize -x2
             * Subject to:
             * h1: 2x1 + x2 - 9 = 0
             * g1: x1 - 4 <= 0
             * g2: x1 + 3x2 - 12 <= 0
             * g3: -x1 <= 0
             * g4: -x2 <= 0
             */

            // From the minimization function we have:
            double[,] A =
            {
                { 1, 0 },
                { 1, 3 },
                { -1, 0 },
                { 0, -1 }
            };
            double[] b = { 4, 12, 0, 0 };
            double[] c = { 0, -1 };
            double[,] Aeq =
            {
                { 2, 1 }
            };
            double[] beq = { 9 };

            var x_actual = new double[] { 3, 3 };

            var x_optimal = LinearProgramming.Solve(A, b, c, Aeq: Aeq, beq: beq)[0];

            Assert.Equal(2, x_optimal.Length);
            Assert.Equal(x_actual[0], x_optimal[0], 2, MidpointRounding.AwayFromZero);
            Assert.Equal(x_actual[1], x_optimal[1], 2, MidpointRounding.AwayFromZero);
        }

        [Fact]
        [Trait("Category", "Linear Programming")]
        public void OilRecipes()
        {
            // Problem from http://kirkmcdonald.github.io/posts/calculation.html
            /* x = [ HOC, LOC, BOP, AOP, Water, Crude ]
             * Minimize Crude
             * We want to produce AT LEAST a certain amount of materials
             * So for the outputs, we specify a positive value
             * For the rest of the materials (intermediates and inputs) we need 0 or more
             * So the problem is subject to:
             * g1 (HO):     -40 HOC + 30 BOP + 10 AOP >= 10
             * g2 (LO):     30 HOC - 30 LOC + 30 BOP + 45 AOP >= 0
             * g3 (P):      20 LOC + 40 BOP + 55 AOP >= 45
             * g4 (Water):  -30 HOC - 30 LOC - 50 AOP + 1 Water >= 0 // This is going to enable the LP solver to set water to the required amount
             * g5 (Crude):  -100 BOP - 100 AOP + 1 Crude >= 0 // Same as for water
             * g6-g11:      xi >= 0 // constrain all variables to be positive
             * We can flip all the signs (greater than to lesser than) by multiplying all numbers by -1
             * We have to because our solver expects inequality constraints in the form of A * x <= b
             */

            double[,] A =
            {
                { 40, 0, -30, -10, 0, 0 },
                { -30, 30, -30, -45, 0, 0 },
                { 0, -20, -40, -55, 0, 0 },
                { 30, 30, 0, 50, -1, 0 },
                { 0, 0, 100, 100, 0, -1 },
                { -1, 0, 0, 0, 0, 0 },
                { 0, -1, 0, 0, 0, 0 },
                { 0, 0, -1, 0, 0, 0 },
                { 0, 0, 0, -1, 0, 0 },
                { 0, 0, 0, 0, -1, 0 },
                { 0, 0, 0, 0, 0, -1 }
            };
            double[] b = { -10, 0, -45, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] c = { 0, 0, 0, 0, 0, 1 }; // Minimize Crude oil
            
            var x_actual = new double[] { 0.00, 0.78, 0.21, 0.38, 42.69, 58.97 };

            var x_optimal = LinearProgramming.Solve(A, b, c)[0];

            Assert.Equal(6, x_optimal.Length);
            for (int i = 0; i < 6; i++)
            {
                Assert.Equal(x_actual[i], x_optimal[i], 2, MidpointRounding.AwayFromZero);
            }
        }
    }
}
