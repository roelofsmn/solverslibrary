﻿using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
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

            var lp = new LinearProgramming(A, b, c);

            var x_actual = new double[] { 3.67, 2.17 };

            var x_optimal = lp.Solve()[0];

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

            var lp = new LinearProgramming(A, b, c);

            var x_actual = new double[][] {
                new double[] { 3.67, 2.17 },
                new double[] { 0.5, 3.75 }
            };

            var x_optimal = lp.Solve();

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

            var lp = new LinearProgramming(A, b, c, Aeq, beq);

            var x_actual = new double[] { 3, 3 };

            var x_optimal = lp.Solve()[0];

            Assert.Equal(2, x_optimal.Length);
            Assert.Equal(x_actual[0], x_optimal[0], 2, MidpointRounding.AwayFromZero);
            Assert.Equal(x_actual[1], x_optimal[1], 2, MidpointRounding.AwayFromZero);
        }

        [Fact]
        [Trait("Category", "Linear Programming")]
        public void OilRecipes_Backward()
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
            // NOTE: the bit on surplus variables is not necessary in our case, bacuse we only build this LP problem from a graph that does not contain unnecessary processes

            double[,] A =
            {
                { 40, 0, -30, -10, 0, 0 }, // heavy oil
                { -30, 30, -30, -45, 0, 0 }, // light oil
                { 0, -20, -40, -55, 0, 0 }, // petroleum
                { 30, 30, 0, 50, -1, 0 }, // water
                { 0, 0, 100, 100, 0, -1 }, // crude oil
                { -1, 0, 0, 0, 0, 0 },
                { 0, -1, 0, 0, 0, 0 },
                { 0, 0, -1, 0, 0, 0 },
                { 0, 0, 0, -1, 0, 0 },
                { 0, 0, 0, 0, -1, 0 },
                { 0, 0, 0, 0, 0, -1 }
            };
            double[] b = { -10, 0, -45, 0, 0, 0, 0, 0, 0, 0, 0 };
            double[] c = { 0, 0, 0, 0, 0, 1 }; // Minimize Crude oil

            var lp = new LinearProgramming(A, b, c);

            var x_actual = new double[] { 0.00, 0.78, 0.21, 0.38, 42.69, 58.97 };

            var x_optimal = lp.Solve()[0];

            Assert.Equal(6, x_optimal.Length);
            for (int i = 0; i < 6; i++)
            {
                Assert.Equal(x_actual[i], x_optimal[i], 2, MidpointRounding.AwayFromZero);
            }
        }

        [Fact]
        [Trait("Category", "Linear Programming")]
        public void OilRecipes_Forward()
        {
            // Problem from http://kirkmcdonald.github.io/posts/calculation.html but inverse
            /* x = [ HOC, LOC, BOP, AOP ]
             * Maximize Petroleum
             * We want to produce AT LEAST a certain amount of materials
             * So for the outputs, we specify a positive value
             * For the rest of the materials (intermediates and inputs) we need 0 or more
             * So the problem is subject to:
             * g1 (HO):     -40 HOC + 30 BOP + 10 AOP >= 0
             * g2 (LO):     30 HOC - 30 LOC + 30 BOP + 45 AOP >= 0
             * g3 (P):      20 LOC + 40 BOP + 55 AOP >= 0
             * g4-g7:      xi >= 0 // constrain all variables to be positive
             * h1 (Crude oil):  100 BOP + 100 AOP = 58.97
             * h2 (Water):      30 HOC + 30 LOC + 50 AOP = 42.69
             * We can flip all the signs (greater than to lesser than) by multiplying all numbers by -1
             * We have to because our solver expects inequality constraints in the form of A * x <= b
             */

            double[,] A =
            {
                { 40, 0, -30, -10 }, // heavy oil
                { -30, 30, -30, -45 }, // light oil
                { 0, -20, -40, -55 }, // petroleum
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
                { 0, 0, 0, -1 }
            };
            double[] b = { 0, 0, 0, 0, 0, 0, 0 };
            //double[] c = { 0, -20, -40, -55 }; // Maximize Petroleum (so minimize its negative value)
            // Perhaps same as for 45 to 10 ratio of petroleum vs heavy oil, i.e. maximize both in that ratio
            double[] c = { 10 * 40, 45 * -20, 10 * -30 - 40 * 45, -10 * 10 - 55 * 45 };

            double[,] Aeq =
            {
                { 30, 30, 0, 50 }, // water consumption
                { 0, 0, 100, 100 }, // crude oil consumption
            };
            double[] beq = { 42.69, 58.97 };

            var lp = new LinearProgramming(A, b, c, Aeq, beq);

            // Expected solution is same as for backwards problem, because there we minimized resource use for certain amount petroleum
            var x_actual = new double[] { 0.00, 0.78, 0.21, 0.38 };

            var x_optimal = lp.Solve()[0];

            Assert.Equal(4, x_optimal.Length);
            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(x_actual[i], x_optimal[i], 2, MidpointRounding.AwayFromZero);
            }
        }

        [Fact]
        [Trait("Category", "Linear Programming")]
        public void OilRecipes_Forward_InequalityDefinition()
        {
            // Problem from http://kirkmcdonald.github.io/posts/calculation.html but inverse
            /* x = [ HOC, LOC, BOP, AOP ]
             * Maximize Petroleum
             * We want to produce AT LEAST a certain amount of materials
             * So for the outputs, we specify a positive value
             * For the rest of the materials (intermediates and inputs) we need 0 or more
             * So the problem is subject to:
             * g1 (HO):     -40 HOC + 30 BOP + 10 AOP >= 0
             * g2 (LO):     30 HOC - 30 LOC + 30 BOP + 45 AOP >= 0
             * g3 (P):      20 LOC + 40 BOP + 55 AOP >= 0
             * g6-g9:      xi >= 0 // constrain all variables to be positive
             * g5 (Crude oil):  100 BOP + 100 AOP <= 58.97
             * g4 (Water):      30 HOC + 30 LOC + 50 AOP <= 42.69
             * We can flip all the signs (greater than to lesser than) by multiplying all numbers by -1
             * We have to because our solver expects inequality constraints in the form of A * x <= b
             */

            double[,] A =
            {
                { 0,0, 40, 0, -30, -10 }, // heavy oil
                { 0,0, -30, 30, -30, -45 }, // light oil
                { 0,0, 0, -20, -40, -55 }, // petroleum
                { -1,0, 30, 30, 0, 50 }, // water
                { 0,-1, 0, 0, 100, 100 }, // crude oil
                { -1,0, 0, 0, 0, 0 },
                { 0,-1, 0, 0, 0, 0 },
                { 0,0, -1, 0, 0, 0 },
                { 0,0, 0, -1, 0, 0 },
                { 0,0, 0, 0, -1, 0 },
                { 0,0, 0, 0, 0, -1 },
                { 1,0, 0, 0, 0, 0 },
                { 0,1, 0, 0, 0, 0 }
            };
            double[] b = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 42.69, 58.97 };
            //double[] c = { 0, -20, -40, -55 }; // Maximize Petroleum (so minimize its negative value)
            // Perhaps same as for 45 to 10 ratio of petroleum vs heavy oil, i.e. maximize both in that ratio
            double[] c = { 0, 0, 10 * 40, 45 * -20, 10 * -30 - 40 * 45, -10 * 10 - 55 * 45 };

            var lp = new LinearProgramming(A, b, c);

            // Expected solution is same as for backwards problem, because there we minimized resource use for certain amount petroleum
            var x_actual = new double[] { 42.69, 58.97, 0.00, 0.78, 0.21, 0.38 };

            var x_optimal = lp.Solve()[0];

            Assert.Equal(6, x_optimal.Length);
            for (int i = 0; i < 6; i++)
            {
                Assert.Equal(x_actual[i], x_optimal[i], 2, MidpointRounding.AwayFromZero);
            }
        }

        [Fact]
        public void SolveProblem85MDO()
        {
            double[] c = new double[] { -1, -2, -3, -1.5 };
            double[,] A = new double[,]
            {
                { 1, 1, 2, 2 },
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
                { 0, 0, 0, -1 },
            };
            double[] b = new double[] { 10, 0, 0, 0, 0 };

            double[,] Aeq = new double[,]
            {
                { 7, 8, 5, 1 }
            };
            double[] beq = new double[] { 31.5 };

            var lp = new LinearProgramming(A, b, c, Aeq, beq);

            // Act
            var solution = lp.Solve(null)[0];
            var objVal = lp.ComputeObjective(solution);

            // Assert
            var expected = new double[] { 0, 1.1818, 4.4091, 0 };
            Helpers.CheckArray(expected, solution);
            Assert.Equal(-15.59, objVal, 2, MidpointRounding.AwayFromZero);
        }
        [Fact]
        public void SolveProblem85MDOStep2()
        {
            double[] c = new double[] { -1, -2, -3, -1.5 };
            double[,] A = new double[,]
            {
                { 1, 1, 2, 2 },
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
                { 0, 0, 0, -1 },
                { 0, 0, 1, 0 }
            };
            double[] b = new double[] { 10, 0, 0, 0, 0, 4 };

            double[,] Aeq = new double[,]
            {
                { 7, 8, 5, 1 }
            };
            double[] beq = new double[] { 31.5 };

            var lp = new LinearProgramming(A, b, c, Aeq, beq);

            // Act
            var solution = lp.Solve(null)[0];
            var objVal = lp.ComputeObjective(solution);

            // Assert
            var expected = new double[] { 0, 1.4, 4, 0.3 };
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], solution[i], 4, MidpointRounding.AwayFromZero);
            }
            Assert.Equal(-15.25, objVal, 2, MidpointRounding.AwayFromZero);
        }
        [Fact]
        public void SolveProblem85MDOStep2_DifferentOrderConstraints()
        {
            double[] c = new double[] { -1, -2, -3, -1.5 };
            double[,] A = new double[,]
            {
                { 1, 1, 2, 2 },
                { 0, 0, 0, -1 },
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
                { 0, 0, 1, 0 }
            };
            double[] b = new double[] { 10, 0, 0, 0, 0, 4 };

            double[,] Aeq = new double[,]
            {
                { 7, 8, 5, 1 }
            };
            double[] beq = new double[] { 31.5 };

            var lp = new LinearProgramming(A, b, c, Aeq, beq);

            // Act
            var solution = lp.Solve(null)[0];
            var objVal = lp.ComputeObjective(solution);

            // Assert
            var expected = new double[] { 0, 1.4, 4, 0.3 };
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i], solution[i], 4, MidpointRounding.AwayFromZero);
            }
            Assert.Equal(-15.25, objVal, 2, MidpointRounding.AwayFromZero);
        }
        [Fact]
        public void SolveProblem85MDOStep3_Infeasible()
        {
            double[] c = new double[] { -1, -2, -3, -1.5 };
            double[,] A = new double[,]
            {
                { 1, 1, 2, 2 },
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
                { 0, 0, 0, -1 },
                { 0, 0, -1, 0 }
            };
            double[] b = new double[] { 10, 0, 0, 0, 0, -5 };

            double[,] Aeq = new double[,]
            {
                { 7, 8, 5, 1 }
            };
            double[] beq = new double[] { 31.5 };

            var lp = new LinearProgramming(A, b, c, Aeq, beq);

            // Act & Assert
            Assert.Throws<InfeasibleProblemException>(() => lp.Solve());
        }

        [Fact]
        public void SubstituteEqualityConstraintInMatrix()
        {
            double[,] A = new double[,]
            {
                { 1, 1, 2, 2 },
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
                { 0, 0, 0, -1 }
            };
            var Amat = Matrix<double>.Build.DenseOfArray(A);
            double[] b = new double[] { 10, 0, 0, 0, 0 };
            var bVec = Vector<double>.Build.DenseOfArray(b);

            double[] Aeq = new double[] { 2, 8, 5, 1 };
            var AeqMat = Vector<double>.Build.DenseOfArray(Aeq);

            // Act
            var result = LinearProgramming.ApplyEqualityConstraint(Amat, bVec, AeqMat, 4, 0);

            // Assert
            double[,] expectedA = new double[,]
            {
                { -3, -0.5, 1.5 },
                { 4, 2.5, 0.5 },
                { -1, 0, 0 },
                { 0, -1, 0 },
                { 0, 0, -1 }
            };
            double[] expectedB = new double[] { 8, 2, 0, 0, 0 };

            Assert.Equal(expectedA, result.ToArray());
            Assert.Equal(expectedB, bVec.ToArray());
        }

        [Fact]
        public void RemoveEqualityConstraint()
        {
            double[] c = new double[] { -1, -2, -3, -1.5 };
            double[,] A = new double[,]
            {
                { 1, 1, 2, 2 },
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
                { 0, 0, 0, -1 }
            };
            var Amat = Matrix<double>.Build.DenseOfArray(A);
            double[] b = new double[] { 10, 0, 0, 0, 0 };
            var bVec = Vector<double>.Build.DenseOfArray(b);

            double[,] Aeq = new double[,] { { 7, 8, 5, 1 } };

            double[] beq = new double[] { 31.5 };

            var lp = new LinearProgramming(A, b, c, Aeq, beq);

            // Act
            lp.RemoveEqualityConstraints();

            // Assert
            double[,] expectedA = new double[,]
            {
                { 1.0-8.0/7.0, 2.0-5.0/7.0, 2.0-1.0/7.0 },
                { 8.0/7.0, 5.0/7.0, 1.0/7.0 },
                { -1, 0, 0 },
                { 0, -1, 0 },
                { 0, 0, -1 }
            };
            double[] expectedB = new double[] { 10.0 - 31.5 / 7.0, 31.5 / 7.0, 0, 0, 0 };

            var multCost = -1.0 / 7.0;
            double[] expectedC = new double[] { -2 - multCost * 8.0, -3 - multCost * 5.0, -1.5 - multCost };

            Helpers.CheckArray(expectedA, lp.InequalityMatrix.ToArray());
            Helpers.CheckArray(expectedB, lp.InequalityVector.ToArray());
            Helpers.CheckArray(expectedC, lp.CostVector.ToArray());
        }

        [Fact]
        public void RemoveEqualityConstraint_Multiple()
        {
            double[] c = new double[] { 1, 2, 3, 4 };
            double[,] A = new double[,]
            {
                { 1, 1, 2, 2 },
                { -1, 0, 0, 0 },
                { 0, -1, 0, 0 },
                { 0, 0, -1, 0 },
                { 0, 0, 0, -1 }
            };
            double[] b = new double[] { 10, 0, 0, 0, 0 };
            double[,] Aeq = new double[,] { { 1, 4, 5, 2 }, { 1, 2, 2, 2 } };
            double[] beq = new double[] { 4, 9 };

            var lp = new LinearProgramming(A, b, c, Aeq, beq);

            // Act
            lp.RemoveEqualityConstraints();

            // Assert
            double[,] expectedA = new double[,]
            {
                { 1.5, 0.0 },
                { -1, 2.0 },
                { 1.5, 0 },
                { -1, 0 },
                { 0, -1 }
            };
            double[] expectedB = new double[] { -1.5, 14, -2.5, 0, 0 };

            double[] expectedC = new double[] { 1, 2 };

            Helpers.CheckArray(expectedA, lp.InequalityMatrix.ToArray());
            Helpers.CheckArray(expectedB, lp.InequalityVector.ToArray());
            Helpers.CheckArray(expectedC, lp.CostVector.ToArray());
        }

        [Fact]
        public void SubstituteBackOriginalVariables()
        {
            var subs = new Stack<(int, Vector<double>, double)>();
            subs.Push((0, Vector<double>.Build.DenseOfArray(new double[] { -2, -3, 0 }), 5));

            var finalSolution = new double[] { 5, 4 };

            var result = LinearProgramming.SubstituteBackOriginalVariables(finalSolution, subs);

            var expected = new double[] { -10, 5, 4 };

            Helpers.CheckArray(expected, result);
        }

    }
}
