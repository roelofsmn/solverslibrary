using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Solvers.Extensions;

namespace SolversLibrary.Optimization
{
    public class LinearProgramming : IAlgorithm<double[]>
    {
        private Matrix<double> _inequalityMatrix;
        private Vector<double> _inequalityVector;
        private double[] _originalCost;
        private Vector<double> _costVector;
        private double[,]? _equalityMatrix;
        private double[]? _equalityVector;

        public Matrix<double> InequalityMatrix { get => _inequalityMatrix; }
        public Vector<double> InequalityVector { get => _inequalityVector; }

        public Vector<double> CostVector { get => _costVector; }

        private bool _processedEqualityConstraints = false;


        /// <summary>
        /// Creates a linear programming problem: min f = c^T x for x.
        /// Subject to inequality constraints g : A x &lt;= b
        /// And equality constraints h : A_eq x = b_eq
        /// </summary>
        /// <param name="A">The inequality constraint LHS</param>
        /// <param name="b">The inequality constraint RHS</param>
        /// <param name="c">The minimization function coefficients</param>
        /// <param name="Aeq">Equality constraint LHS</param>
        /// <param name="beq">Equality constraint RHS</param>
        public LinearProgramming(double[,] A, double[] b, double[] c, double[,]? Aeq = null, double[]? beq = null)
        {
            if (b.Length != A.GetLength(0))
                throw new ArgumentException();
            if (c.Length != A.GetLength(1))
                throw new ArgumentException();

            if (Aeq != null && Aeq.GetLength(1) != A.GetLength(1))
                throw new ArgumentException();
            if (beq != null ^ Aeq != null)
                throw new ArgumentException();
            if (beq != null && beq.Length != Aeq!.GetLength(0))
                throw new ArgumentException();

            _inequalityMatrix = Matrix<double>.Build.DenseOfArray(A);
            _inequalityVector = Vector<double>.Build.DenseOfArray(b);
            _originalCost = c;
            _costVector = Vector<double>.Build.DenseOfArray(c);
            _equalityMatrix = Aeq;
            _equalityVector = beq;
        }

        private Stack<(int variableIndex, Vector<double> equalityCoefficients, double RHS)> _substitutionEquations = new Stack<(int, Vector<double>, double)>();

        public event Action<double[]>? ProgressUpdated;

        /// <summary>
        /// Removes equality constraints from the problem, and rewrites the inequality constraints and cost vector accordingly.
        /// This process removes variables from the problem.
        /// Those substitutions are stored in a stack, so they can be replayed on a solution for the reduced problem, 
        /// to translate the solution into the original space <see cref="SubstituteBackOriginalVariables(double[])"/>.
        /// </summary>
        public void RemoveEqualityConstraints()
        {
            if (_processedEqualityConstraints) return;

            if (_equalityMatrix != null && _equalityVector != null)
            {
                var remainingEqualityConstraints = Matrix<double>.Build.DenseOfArray(_equalityMatrix);
                var remainingEqualityVector = Vector<double>.Build.DenseOfArray(_equalityVector);
                // Process each equality constraint
                for (int equalityConstraint = 0; equalityConstraint < _equalityMatrix.GetLength(0); equalityConstraint++)
                {
                    var currentConstraint = remainingEqualityConstraints.Row(equalityConstraint);
                    var RHS = remainingEqualityVector[equalityConstraint];
                    // just pick the first variable that appears in the constraint (i.e. has nonzero coefficient)
                    int variableToSubstitute = currentConstraint.Find(val => val != 0.0).Item1;

                    // substitute variable in inequalities
                    _inequalityMatrix = ApplyEqualityConstraint(_inequalityMatrix, _inequalityVector, currentConstraint, RHS, variableToSubstitute);
                    // substitute variable in remaining equality equations
                    remainingEqualityConstraints = ApplyEqualityConstraint(remainingEqualityConstraints, remainingEqualityVector, currentConstraint, RHS, variableToSubstitute);

                    // substitute variable in cost
                    var multCost = _costVector[variableToSubstitute] / currentConstraint[variableToSubstitute];
                    _costVector -= multCost * currentConstraint;
                    _costVector = _costVector.RemoveAt(variableToSubstitute);

                    // store the substitution for later
                    _substitutionEquations.Push(
                        (variableToSubstitute,
                        currentConstraint,
                        RHS)
                    );
                }
            }

            _processedEqualityConstraints = true;
        }

        /// <summary>
        /// Substitute back removed variables from original problem into final solution.
        /// </summary>
        /// <param name="finalSolution">The final solution of the reduced problem.</param>
        /// <returns>The full solution to the original problem.</returns>
        public double[] SubstituteBackOriginalVariables(double[] finalSolution)
        {
            return SubstituteBackOriginalVariables(finalSolution, _substitutionEquations);
        }

        /// <summary>
        /// Substitute back removed variables from original problem into final solution.
        /// For example, the final solution is [x2=5, x3=9], but we subsituted an original x1 through x1 + 3 x2 + 4 x3 = 9.
        /// Then the original full solution would be [x1=-42, x2=5, x3=9].
        /// </summary>
        /// <param name="finalSolution">The final solution of the reduced problem.</param>
        /// <param name="substitutions">The substitution equations for the original variables.</param>
        /// <returns>The full solution to the original problem.</returns>
        public static double[] SubstituteBackOriginalVariables(double[] finalSolution, Stack<(int variableIndex, Vector<double> equalityCoefficients, double RHS)> substitutions)
        {
            Vector<double> solution = Vector<double>.Build.DenseOfArray(finalSolution);
            // work through substitutions in backwards order
            while (substitutions.Count > 0)
            {
                var substitution = substitutions.Pop();

                var coeff = substitution.equalityCoefficients[substitution.variableIndex];
                var otherCoeff = substitution.equalityCoefficients.RemoveAt(substitution.variableIndex);

                var value = (substitution.RHS - otherCoeff.Zip(solution).Select(t => t.First * t.Second).Sum()) / coeff;

                solution = solution.InsertAt(substitution.variableIndex, value);
            }
            return solution.ToArray();
        }

        /// <summary>
        /// Reduces a linear system of equations by applying a equality constraint to substitute the first variable.
        /// </summary>
        /// <param name="matrix">The matrix A.</param>
        /// <param name="vector">The vector b.</param>
        /// <param name="constraint">The equality constraint coefficients.</param>
        /// <param name="b">The equality constraint RHS.</param>
        /// <returns>A new matrix A. The vector b is modified in-place.</returns>
        public static Matrix<double> ApplyEqualityConstraint(Matrix<double> matrix, Vector<double> vector, Vector<double> constraint, double b, int variableToSubstitute)
        {
            for (int i = 0; i < matrix.RowCount; i++)
            {
                var currentRow = matrix.Row(i);
                var multiplier = currentRow[variableToSubstitute] / constraint[variableToSubstitute];
                if (multiplier == 0.0 || double.IsInfinity(multiplier) || double.IsNaN(multiplier))
                    continue;
                matrix.SetRow(i, currentRow - multiplier * constraint); // This should make the first element zero
                vector[i] -= b * multiplier;
            }
            return matrix.RemoveColumn(variableToSubstitute);
        }

        /// <summary>
        /// Solves the linear program from a given initial point
        /// </summary>
        /// <param name="x0">The initial point. If null, it is computed.</param>
        /// <returns>A set of optimal solutions.</returns>
        /// <exception cref="InfeasibleProblemException"></exception>
        public double[][] Solve(double[]? x0 = null)
        {
            if (!_processedEqualityConstraints)
                RemoveEqualityConstraints();

            if (x0 == null)
                x0 = FindInitialSolution();

            // Following Principles Of Optimal Design, 2ed, P.Y. Papalambros and D.J. Wilde - Ch. 5 p. 208-213
            var curSol = Vector<double>.Build.DenseOfArray(x0);
            var ineqConstraints = Enumerable.Range(0, _inequalityVector.Count);
            //var eqConstraints = hasEqConstraint ? Enumerable.Range(_inequalityVector.Count, _equalityVector.Length) : Enumerable.Empty<int>();

            var Amat = _inequalityMatrix;
                        
            var b0 = Amat * curSol;
            var activeConstraints = _inequalityVector.Select((s, i) => i).Where(i => Precision.AlmostEqualRelative(_inequalityVector[i], b0[i], 1e-9)).ToList();

            Matrix<double> Atotal;
            //if (hasEqConstraint)
            //{
            //    var Aeqmat = Matrix<double>.Build.DenseOfArray(_equalityMatrix);
            //    Atotal = Matrix<double>.Build.DenseOfMatrixArray(new Matrix<double>[,] { { Amat }, { Aeqmat } });
            //    var b0eq = Aeqmat * curSol;
            //    activeConstraints.AddRange(_equalityVector.Select((s, i) => i).Where(i => Precision.AlmostEqualRelative(_equalityVector[i], b0eq[i], 1e-9)).Select(i => i + _inequalityVector.Count));
            //}
            //else
            //{
                Atotal = Matrix<double>.Build.DenseOfMatrixArray(new Matrix<double>[,] { { Amat } });
            //}
            if (activeConstraints.Count > x0.Length)
                activeConstraints = activeConstraints.Take(x0.Length).ToList();

            // Select only part of Amat that represents active constraints
            var A0 = Matrix<double>.Build.DenseOfRowVectors(activeConstraints.Select(ac => Atotal.Row(ac)));
            var D = A0.Inverse();

            int? prevSearchDir = null;

            var solutions = new List<Vector<double>>();

            while (true)
            {
                ProgressUpdated?.Invoke(curSol.ToArray());

                var search = D.Transpose() * _costVector;
                // get index of search direction d_s (leaving variable)
                // indices that represent an equality constraint may not be removed
                var searchDir = ArgMax(search.ToArray(), activeConstraints.Select(ac => ineqConstraints.Contains(ac)).ToArray());

                //var openEqConstraints = eqConstraints.Except(activeConstraints).ToArray();

                if (search[searchDir] < 0) // No more improvement can be made
                {
                    //if (openEqConstraints.Any())
                    //    throw new InfeasibleProblemException();
                    solutions.Add(curSol);
                    break;
                }

                var alpha = double.PositiveInfinity;
                int j = -1;

                // Until update current point: find entering variable
                // First add equality constraints to active set
                
                //if (openEqConstraints.Any())
                //{
                //    j = openEqConstraints[0];
                //    var b_index = j - _inequalityVector.Count;
                //    alpha = (Atotal.Row(j) * curSol - _equalityVector[b_index]) / (Atotal.Row(j) * D.Column(searchDir));
                //}
                // If all equality constraints in solution, find an inequality constraint
                //else
                //{
                    foreach (var openConstraint in ineqConstraints.Except(activeConstraints))
                    {
                        var ad = Amat.Row(openConstraint) * D.Column(searchDir);
                        if (ad >= 0)
                            continue;
                        var alpha_c = (Amat.Row(openConstraint) * curSol - _inequalityVector[openConstraint]) / ad;

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
            return solutions.Select(sol => SubstituteBackOriginalVariables(sol.ToArray(), _substitutionEquations)).ToArray();
        }

        /// <summary>
        /// Finds an initial feasible solution to the problem.
        /// </summary>
        /// <returns>A point in the solution space that is feasible, i.e. does not violate any constraint.</returns>
        /// <exception cref="InfeasibleProblemException">Thrown when the problem is infeasible, i.e. there is no space where no constraint is violated.</exception>
        public double[] FindInitialSolution()
        {
            var m = _inequalityMatrix.RowCount;
            var n = _inequalityMatrix.ColumnCount;

            var Amat = Matrix<double>.Build.DenseOfMatrix(_inequalityMatrix);

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
                    
                    var solution = SolveForBasis(m, n, transposed, validBasis, basisMat);

                    if (IsSolutionFeasible(solution))
                        return solution;
                }
            }
            throw new InfeasibleProblemException();
        }

        private double[] SolveForBasis(int m, int n, bool transposed, int[] validBasis, Matrix<double> basisMat)
        {
            if (transposed)
            {
                return basisMat.Solve(Vector<double>.Build.DenseOfEnumerable(validBasis.Select(i => _inequalityVector[i]))).ToArray();
            }
            else
            {
                var sol = basisMat.Solve(_inequalityVector).ToArray();

                var completeSol = new double[n];
                for (int i = 0; i < m; i++)
                    completeSol[validBasis[i]] = sol[i];
                return completeSol;
            }
        }


        public bool IsSolutionFeasible(double[] x)
        {
            var result = _inequalityMatrix * Vector<double>.Build.DenseOfArray(x);
            return result.Select((val, i) => val <= _inequalityVector[i]).All(t => t);
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

        

        public double ComputeObjective(double[] x)
        {
            return _originalCost.Zip(x).Select(t => t.First * t.Second).Sum();
        }

        public double[] Run()
        {
            return Solve()[0];
        }
    }

    public class InfeasibleProblemException : Exception
    {

    }
}
