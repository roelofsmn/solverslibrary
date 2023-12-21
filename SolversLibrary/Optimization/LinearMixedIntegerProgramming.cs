using SolversLibrary.Search;
using SolversLibrary.Search.Factories;
using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolversLibrary.Optimization
{
    public class LinearMixedIntegerProgramming
    {
        private readonly double[,] inequalityMatrix;
        private readonly double[,]? equalityMatrix;
        private readonly double[] inequalityVector;
        private readonly double[]? equalityVector;
        private readonly double[] cost;
        private readonly int[] integerIndices;

        public LinearMixedIntegerProgramming(
            double[,] A,
            double[] b,
            double[] c,
            int[] integerIndices,
            double[,]? Aeq = null,
            double[]? beq = null)
        {
            if (integerIndices.Max() >= c.Length)
                throw new IndexOutOfRangeException();
            if (integerIndices.Min() < 0)
                throw new IndexOutOfRangeException();
            this.integerIndices = integerIndices;

            this.cost = c;
            this.inequalityMatrix = A;
            this.inequalityVector = b;
            this.equalityMatrix = Aeq;
            this.equalityVector = beq;
        }

        private double[]? bestSolution;
        private double bestValue = double.PositiveInfinity;

        public double[] Solve(TraversalStrategies strategyType)
        {
            var strategy = TraversalStrategyFactory.Create<MixedIntegerProblemState>(strategyType);
            var branchingFunction = new MixedIntegerBranching(integerIndices);
            var goal = new MixedIntegerGoal(integerIndices);

            var root = new MixedIntegerProblemState(
                inequalityMatrix,
                inequalityVector,
                new double[0]);

            strategy.AddCandidateState(root);

            while (strategy.ContainsCandidates())
            {
                var currentProblem = strategy.NextState();
                try
                {
                    var lp = new LinearProgramming(currentProblem.A, currentProblem.b, cost, equalityMatrix, equalityVector); // No more equality constraints. Already removed in initial problem...
                    var currentSolution = lp.Solve()[0];

                    currentProblem.x = currentSolution;
                    double fval = ComputeObjective(currentSolution);

                    if (goal.IsTerminal(currentProblem) && fval < bestValue)
                    {
                        bestSolution = currentSolution;
                        bestValue = fval;
                    }
                    else if (fval > bestValue) // Bound
                        continue; // Prune
                    else
                    {
                        // Branch
                        foreach (var traversal in branchingFunction.Branch(currentProblem))
                            strategy.AddCandidateState(traversal.Traverse(currentProblem));
                    }
                }
                catch (InfeasibleProblemException) // Infeasible problem
                {
                    continue; // Prune
                }
            }

            if (bestSolution != null)
                return bestSolution;
            else
                throw new NoSolutionFoundException();
        }

        public double ComputeObjective(double[] x)
        {
            return cost.Zip(x).Select(t => t.First * t.Second).Sum();
        }
    }

    internal class MixedIntegerGoal : IGoalDefinition<MixedIntegerProblemState>
    {
        private readonly int[] integerIndices;

        public MixedIntegerGoal(int[] integerIndices)
        {
            this.integerIndices = integerIndices;
        }
        public bool IsTerminal(MixedIntegerProblemState state)
        {
            return integerIndices.All(i => double.IsInteger(Math.Round(state.x[i], 6)));
        }
    }

    internal struct MixedIntegerProblemState
    {
        internal readonly double[,] A;
        internal readonly double[] b;
        internal double[] x { get; set; }

        internal MixedIntegerProblemState(double[,] A, double[] b, double[] x)
        {
            this.A = A;
            this.b = b;
            this.x = x;
        }

        /// <summary>
        /// Creates a new problem state from an existing one, with an optionally extra inequality constraint.
        /// </summary>
        /// <param name="current">The current problem state.</param>
        /// <param name="newInequalityConstraint">A new inequality constraint. When lessThanOrEqual=true, we add x[var] <= val. Otherwise, we add x[var] >= val.</param>
        /// <returns>A new linear mixed-integer problem.</returns>
        internal static MixedIntegerProblemState CreateFrom(
            MixedIntegerProblemState current,
            (int var, double val, bool lessThanOrEqual)? newInequalityConstraint = null)
        {
            var newX = current.x;

            // Copy the b vector
            double[] newB;
            if (newInequalityConstraint == null)
                newB = new double[current.b.Length];
            else
                newB = new double[current.b.Length + 1];
            Buffer.BlockCopy(current.b, 0, newB, 0, current.b.Length * sizeof(double));

            // Copy the A matrix
            double[,] newA = new double[newB.Length, newX.Length];
            Buffer.BlockCopy(current.A, 0, newA, 0, current.A.Length * sizeof(double));

            // Add constraint values if necessary
            if (newInequalityConstraint != null)
            {
                var sign = newInequalityConstraint.Value.lessThanOrEqual ? 1 : -1;

                // Create new b vector with added value.
                newB[^1] = newInequalityConstraint.Value.val * sign;

                // Add the inequality constraint
                newA[newB.Length - 1, newInequalityConstraint.Value.var] = sign;

                // Overwrite new solution vector to have valid value for constrained variable.
                newX[newInequalityConstraint.Value.var] = newInequalityConstraint.Value.val;
            }

            return new MixedIntegerProblemState(
                newA,
                newB,
                newX);
        }
    }

    internal struct MixedIntegerTraversal : ITraversal<MixedIntegerProblemState>
    {
        private readonly MixedIntegerProblemState newState;

        internal MixedIntegerTraversal(MixedIntegerProblemState newState)
        {
            this.newState = newState;
        }
        public double? Cost(MixedIntegerProblemState state)
        {
            return null;
        }

        public MixedIntegerProblemState Traverse(MixedIntegerProblemState state)
        {
            return newState;
        }
    }

    internal class MixedIntegerBranching : IBranchingFunction<MixedIntegerProblemState>
    {
        private readonly int[] integerIndices;
        /// <summary>
        /// Construct a branching mechanism for linear mixed-integer programming problems.
        /// </summary>
        /// <param name="integerIndices">The indices of the solution vector that have to have integer values.</param>
        internal MixedIntegerBranching(int[] integerIndices)
        {
            this.integerIndices = integerIndices;
        }

        /// <summary>
        /// Creates two new branches from the current problem by selecting an integer variable to branch on.
        /// For example, the current solution is [0, 1.4, 4, 0.3], but all variables have to be integer.
        /// Then, we select x2 (with value 1.4). <see cref="SelectIntegerVariableToBranchOn(double[])"/>
        /// For that variable, we create a branch with an LP problem with the added constraint x2 <= 1,
        /// and a branch with an LP problem with the added constraint x2 >= 2.
        /// </summary>
        /// <param name="state">The current linear mixed-integer problem.</param>
        /// <returns>Two new branches represented by new linear mixed-integer problems, or nothing if there are no more branches.</returns>
        public IEnumerable<ITraversal<MixedIntegerProblemState>> Branch(MixedIntegerProblemState state)
        {
            var branchingVariable = SelectIntegerVariableToBranchOn(state.x);
            if (branchingVariable < 0) // invalid index
                yield break;

            var variableValue = state.x[branchingVariable];

            yield return new MixedIntegerTraversal(
                MixedIntegerProblemState.CreateFrom(
                    state,
                    (branchingVariable, double.Floor(variableValue), true)));

            yield return new MixedIntegerTraversal(
                MixedIntegerProblemState.CreateFrom(
                    state,
                    (branchingVariable, double.Ceiling(variableValue), false)));
        }

        /// <summary>
        /// Selects the integer variable to branch on.
        /// This is a variable that needs to have an integer value, but currently has a real value.
        /// The variable whose fractional component is closest to 0.5 is selected.
        /// </summary>
        /// <param name="x">The current solution.</param>
        /// <returns>The integer variable to branch on, or -1 if no such variable was found.</returns>
        private int SelectIntegerVariableToBranchOn(double[] x)
        {
            double minimumFractionDistanceToHalf = double.PositiveInfinity;
            int selectedVariable = -1;
            for (int i = 0; i < integerIndices.Length; i++)
            {
                var variableValue = x[integerIndices[i]];
                var fractionalDistanceToHalf = Math.Abs(variableValue - (int)variableValue - 0.5);
                if (!double.IsInteger(variableValue) // there is a fractional component
                    && fractionalDistanceToHalf < minimumFractionDistanceToHalf) // it is closest to 0.5
                {
                    selectedVariable = integerIndices[i];
                    minimumFractionDistanceToHalf = fractionalDistanceToHalf;
                }
            }
            return selectedVariable;
        }
    }
}
