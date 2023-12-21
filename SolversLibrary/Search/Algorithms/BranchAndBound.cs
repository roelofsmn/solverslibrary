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
        private ITraverser<SearchNode<T>> _traverser;

        private SearchSolution<T>? bestSolution;
        private BoundingBranchingFunction<T> branching;

        public BranchAndBound(
            Traversers traversalType,
            TraversalStrategies traverseStrategy,
            IBranchingFunction<T> branchingFunction,
            IEqualityComparer<T>? stateEquality = null)
        {
            var strategy = TraversalStrategyFactory.Create<SearchNode<T>>(traverseStrategy);
            branching = new BoundingBranchingFunction<T>(
                branchingFunction);
            _traverser = TraverserFactory.Create<SearchNode<T>>(
                traversalType,
                branching,
                strategy,
                new SearchNodeStateComparer<T>(stateEquality));
        }
        public SearchSolution<T> Search(IGoalDefinition<T> goal, T initialState)
        {
            if (goal.IsTerminal(initialState))
                return new SearchSolution<T>(initialState, Array.Empty<ITraversal<T>>(), initialState, 0.0);

            foreach (var node in _traverser.Traverse(new SearchNode<T>(initialState, null, null, 0.0)))
            {
                //var fval = _cost(node.State);
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
                //else if (fval > bestValue)
                //    _traverser.
                
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
            IBranchingFunction<T> branchingFunction) : base(branchingFunction)
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
