using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Complex;
using System.ComponentModel;

namespace SolversLibrary.Optimization
{
    public static class LinearProgramming
    {
        /// <summary>
        /// Solves a linear programming problem: min f = c^T x for x.
        /// Subject to inequality constraints g : A x &lt;= b
        /// And equality constraints h : A_eq x = b_eq
        /// </summary>
        /// <param name="A">The inequality constraint LHS</param>
        /// <param name="b">The inequality constraint RHS</param>
        /// <param name="c">The minimization function coefficients</param>
        /// <param name="x0">Initial solution</param>
        /// <param name="Aeq">Equality constraint LHS</param>
        /// <param name="beq">Equality constraint RHS</param>
        /// <returns></returns>
        public static double[][] Solve(double[,] A, double[] b, double[] c, double[]? x0 = null, double[,]? Aeq = null, double[]? beq = null)
        {
            bool hasEqConstraint = Aeq != null;
            if (Aeq == null)
                Aeq = new double[,] { };
            if (beq == null)
                beq = new double[] { };

            if (x0 == null)
                x0 = FindInitialSolution(A, b);

            // Following Principles Of Optimal Design, 2ed, P.Y. Papalambros and D.J. Wilde - Ch. 5 p. 208-213
            var curSol = Vector<double>.Build.DenseOfArray(x0);
            var ineqConstraints = Enumerable.Range(0, b.Length);
            var eqConstraints = Enumerable.Range(b.Length, beq.Length);

            var Amat = Matrix<double>.Build.DenseOfArray(A);
                        
            var b0 = Amat * curSol;
            var activeConstraints = b.Select((s, i) => i).Where(i => Precision.AlmostEqualRelative(b[i], b0[i], 1e-9)).ToList();

            Matrix<double> Atotal;
            if (hasEqConstraint)
            {
                var Aeqmat = Matrix<double>.Build.DenseOfArray(Aeq);
                Atotal = Matrix<double>.Build.DenseOfMatrixArray(new Matrix<double>[,] { { Amat }, { Aeqmat } });
                var b0eq = Aeqmat * curSol;
                activeConstraints.AddRange(beq.Select((s, i) => i).Where(i => Precision.AlmostEqualRelative(beq[i], b0eq[i], 1e-9)).Select(i => i + b.Length));
            }
            else
            {
                Atotal = Matrix<double>.Build.DenseOfMatrixArray(new Matrix<double>[,] { { Amat } });
            }
            if (activeConstraints.Count > x0.Length)
                activeConstraints = activeConstraints.Take(x0.Length).ToList();

            // Select only part of Amat that represents active constraints
            var A0 = Matrix<double>.Build.DenseOfRowVectors(activeConstraints.Select(ac => Atotal.Row(ac)));
            var D = A0.Inverse();

            int? prevSearchDir = null;

            var solutions = new List<Vector<double>>();

            while (true)
            {
                var search = D.Transpose() * Vector<double>.Build.DenseOfArray(c);
                // get index of search direction d_s (leaving variable)
                // indices that represent an equality constraint may not be removed
                var searchDir = ArgMax(search.ToArray(), activeConstraints.Select(ac => ineqConstraints.Contains(ac)).ToArray());

                var openEqConstraints = eqConstraints.Except(activeConstraints).ToArray();

                if (search[searchDir] < 0) // No more improvement can be made
                {
                    if (openEqConstraints.Any())
                        throw new InfeasibleProblemException();
                    solutions.Add(curSol);
                    break;
                }

                var alpha = double.PositiveInfinity;
                int j = -1;

                // Until update current point: find entering variable
                // First add equality constraints to active set
                
                if (openEqConstraints.Any())
                {
                    j = openEqConstraints[0];
                    var b_index = j - b.Length;
                    alpha = (Atotal.Row(j) * curSol - beq[b_index]) / (Atotal.Row(j) * D.Column(searchDir));
                }
                // If all equality constraints in solution, find an inequality constraint
                //else
                //{
                    foreach (var openConstraint in ineqConstraints.Except(activeConstraints))
                    {
                        var ad = Amat.Row(openConstraint) * D.Column(searchDir);
                        if (ad >= 0)
                            continue;
                        var alpha_c = (Amat.Row(openConstraint) * curSol - b[openConstraint]) / ad;

                        if (alpha_c < alpha)
                        {
                            alpha = alpha_c;
                            j = openConstraint;
                        }
                    }
                    if (j == -1)
                    {
                        solutions.Add(curSol);
                        break;
                    }
                //}
                activeConstraints[searchDir] = j;

                // Already add current (i.e. previous) solution to solutions.
                if (searchDir == prevSearchDir)
                    solutions.Add(curSol);

                // update current point
                curSol = curSol - alpha * D.Column(searchDir);

                #region Check for oscillations (i.e. cycling between two solutions)

                if (searchDir == prevSearchDir)
                {
                    solutions.Add(curSol);
                    break;
                }
                prevSearchDir = searchDir;
                #endregion

                var newColumns = new Vector<double>[D.ColumnCount];
                // update D
                for (int i = 0; i < activeConstraints.Count; i++)
                {
                    newColumns[i] = D.Column(i) - ((Atotal.Row(j) * D.Column(i)) / (Atotal.Row(j) * D.Column(searchDir))) * D.Column(searchDir);
                }
                newColumns[searchDir] = D.Column(searchDir) / (Atotal.Row(j) * D.Column(searchDir));
                D = Matrix<double>.Build.DenseOfColumnVectors(newColumns);
            }
            return solutions.Select(sol => sol.ToArray()).ToArray();
        }

        public static double[] FindInitialSolution(double[,] A, double[] b)
        {
            var m = A.GetLength(0);
            var n = A.GetLength(1);

            var Amat = Matrix<double>.Build.DenseOfArray(A);

            bool transposed = false;
            if (m > n)
            {
                transposed = true;
                Amat = Amat.Transpose();
                m = Amat.RowCount;
                n = Amat.ColumnCount;
            }

            var colCombinations = Combinations(Enumerable.Range(0, n), m);

            int[] validBasis = null;
            Matrix<double> basisMat = null;

            foreach (IEnumerable<int> basis in colCombinations)
            {
                var subMatrix = Matrix<double>.Build.DenseOfColumnVectors(basis.Select(i => Amat.Column(i)));
                if (transposed)
                    subMatrix = subMatrix.Transpose();
                var det = subMatrix.Determinant();
                if (det != 0.0 && !double.IsNaN(det))
                {
                    basisMat = subMatrix;
                    validBasis = basis.ToArray();
                    break;
                }
            }
            if (basisMat != null)
            {
                if (transposed)
                {
                    return basisMat.Solve(Vector<double>.Build.DenseOfEnumerable(validBasis.Select(i => b[i]))).ToArray();
                }
                else
                {
                    var sol = basisMat.Solve(Vector<double>.Build.DenseOfArray(b)).ToArray();

                    var completeSol = new double[n];
                    for (int i = 0; i < m; i++)
                        completeSol[validBasis[i]] = sol[i];
                    return completeSol;
                }
            }
            else
                throw new InfeasibleProblemException();
        }
        private static bool NextCombination(IList<int> num, int n, int k)
        {
            bool finished;

            var changed = finished = false;

            if (k <= 0) return false;

            for (var i = k - 1; !finished && !changed; i--)
            {
                if (num[i] < n - 1 - (k - 1) + i)
                {
                    num[i]++;

                    if (i < k - 1)
                        for (var j = i + 1; j < k; j++)
                            num[j] = num[j - 1] + 1;
                    changed = true;
                }
                finished = i == 0;
            }

            return changed;
        }

        private static IEnumerable Combinations<T>(IEnumerable<T> elements, int k)
        {
            var elem = elements.ToArray();
            var size = elem.Length;

            if (k > size) yield break;

            var numbers = new int[k];

            for (var i = 0; i < k; i++)
                numbers[i] = i;

            do
            {
                yield return numbers.Select(n => elem[n]);
            } while (NextCombination(numbers, size, k));
        }
        public static int ArgMax(double[] data, bool[] allowIndex)
        {
            int maxIndex = -1;
            double maxVal = double.NegativeInfinity;
            for (int i = 0; i < data.Length; i++)
            {
                if (!allowIndex[i])
                    continue;
                if (data[i] > maxVal)
                {
                    maxVal = data[i];
                    maxIndex = i;
                }
            }
            if (maxIndex == -1)
                throw new InvalidOperationException();
            return maxIndex;
        }

        public static T[] GetRow<T>(this T[,] array, int row)
        {
            var N = array.GetLength(1);
            var output = new T[N];
            for (int j = 0; j < N; j++)
                output[j] = array[row, j];

            return output;
        }
        public static T[] GetColumn<T>(this T[,] array, int column)
        {
            var N = array.GetLength(0);
            var output = new T[N];
            for (int j = 0; j < N; j++)
                output[j] = array[j, column];

            return output;
        }

        public static double ComputeObjective(double[] cost, double[] x)
        {
            return cost.Zip(x).Select(t => t.First * t.Second).Sum();
        }
    }

    public class InfeasibleProblemException : Exception
    {

    }
}
