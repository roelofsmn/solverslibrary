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
        private readonly Func<T, double, ITraversal<T>, double> newStateCost;
        private ITraverser<SearchNode<T>> _traverser;

        private SearchSolution<T>? bestSolution;
        private BoundingBranchingFunction<T> branching;

        public BranchAndBound(
            Traversers traversalType,
            TraversalStrategies traverseStrategy,
            IBranchingFunction<T> branchingFunction,
            Func<T, double, ITraversal<T>, double> newStateCost,
            IEqualityComparer<T>? stateEquality = null)
        {
            var strategy = TraversalStrategyFactory.Create<SearchNode<T>>(traverseStrategy);
            branching = new BoundingBranchingFunction<T>(
                branchingFunction,
                newStateCost);
            _traverser = TraverserFactory.Create<SearchNode<T>>(
                traversalType,
                branching,
                strategy,
                new SearchNodeStateComparer<T>(stateEquality));
            this.newStateCost = newStateCost;
        }

        public IGoalDefinition<T> Goal { get; set; }
        public T InitialState { get; set; }

        public event Action<SearchSolution<T>>? ProgressUpdated;

        public SearchSolution<T> Run()
        {
            if (Goal == null || InitialState == null)
                throw new ArgumentNullException();
            return Search(Goal, InitialState);
        }

        public SearchSolution<T> Search(IGoalDefinition<T> goal, T initialState)
        {
            var rootCost = newStateCost(initialState, 0.0, null);
            if (goal.IsTerminal(initialState))
                return new SearchSolution<T>(initialState, Array.Empty<ITraversal<T>>(), initialState, rootCost);

            foreach (var node in _traverser.Traverse(new SearchNode<T>(initialState, null, null, rootCost)))
            {
                ProgressUpdated?.Invoke(new SearchSolution<T>(
                    initialState,
                    node.GetActionsToNode(),
                    node.State,
                    node.Cost));

                if (goal.IsTerminal(node.State) && node.Cost < branching.BestValue)
                {
                    bestSolution = new SearchSolution<T>(
                        initialState,
                        node.GetActionsToNode(),
                        node.State,
                        node.Cost);

                    branching.BestValue = node.Cost;
                    continue;
                }
            }
            if (bestSolution != null)
                return bestSolution;
            else
                throw new NoSolutionFoundException();
        }
    }

    internal class BoundingBranchingFunction<T> : SearchNodeBranchingFunction<T>
    {
        public double BestValue { get; set; } = double.PositiveInfinity;
        internal BoundingBranchingFunction(
            IBranchingFunction<T> branchingFunction,
            Func<T, double, ITraversal<T>, double> newStateCost) : base(branchingFunction, newStateCost)
        {
        }

        public override IEnumerable<ITraversal<SearchNode<T>>> Branch(SearchNode<T> current)
        {
            if (current.Cost > BestValue)
                return Enumerable.Empty<ITraversal<SearchNode<T>>>();
            return base.Branch(current);
        }
    }
}
