using SolversLibrary.Optimization;
using SolversLibrary.Search.Factories;
using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Algorithms
{
    public class BranchAndBound<T> : ISearchAlgorithm<T>
    {
        private readonly Func<T, double> actualCost;
        private ITraverser<T> _traverser;

        private T? bestSolution;
        private BoundingBranchingFunction<T> branching;

        public BranchAndBound(
            Traversers traversalType,
            ITraversalStrategy<T> traverseStrategy,
            IBranchingFunction<T> branchingFunction,
            Func<T, double> stateCost,
            IEqualityComparer<T>? stateEquality = null)
        {
            //var strategy = TraversalStrategyFactory.Create<SearchNode<T>>(traverseStrategy);

            branching = new BoundingBranchingFunction<T>(
                branchingFunction,
                stateCost);
            _traverser = TraverserFactory.Create<T>(
                traversalType,
                branching,
                traverseStrategy,
                stateEquality);
            _traverser.Skip += _traverser_Skip;

            this.actualCost = stateCost;
        }

        public event Action<T>? Explored;

        public T Search(IGoalDefinition<T> goal, T initialState)
        {
            if (goal.IsTerminal(initialState))
                return initialState;

            foreach (T node in _traverser.Traverse(initialState))
            {
                Explored?.Invoke(node);

                if (goal.IsTerminal(node))
                {
                    _goalStates.Add(node);
                    if (actualCost(node) < branching.BestValue)
                    {
                        bestSolution = node;
                        branching.BestValue = actualCost(node);
                    }
                }
            }
            if (bestSolution != null)
                return bestSolution;
            else
                throw new NoSolutionFoundException();
        }

        private HashSet<T> _goalStates = new HashSet<T>();
        private bool _traverser_Skip(T obj)
        {
            return _goalStates.Contains(obj);
        }
    }

    internal class BoundingBranchingFunction<T> : IBranchingFunction<T>
    {
        private readonly IBranchingFunction<T> branchingFunction;
        private readonly Func<T, double> costOfState;

        public double BestValue { get; set; } = double.PositiveInfinity;
        internal BoundingBranchingFunction(
            IBranchingFunction<T> branchingFunction,
            Func<T, double> costOfState)
        {
            this.branchingFunction = branchingFunction;
            this.costOfState = costOfState;
        }

        public IEnumerable<ITraversal<T>> Branch(T current)
        {
            if (costOfState(current) > BestValue)
                return Enumerable.Empty<ITraversal<T>>();
            return branchingFunction.Branch(current);
        }
    }
}
