using MathNet.Numerics.RootFinding;
using SolversLibrary.Search.Factories;
using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;

namespace SolversLibrary.Search.Algorithms
{
    public class UninformedPathSearch<T> : ISearchAlgorithm<T>
    {
        private ITraverser<SearchNode<T>> _traverser;

        public UninformedPathSearch(
            TraversalStrategies strategyType,
            Traversers traverseType,
            IBranchingFunction<T> branchingFunction,            
            IEqualityComparer<T>? stateEquality = null)
        {
            var strategy = TraversalStrategyFactory.Create<SearchNode<T>>(strategyType);
            _traverser = TraverserFactory.Create<SearchNode<T>>(traverseType,
                new SearchNodeBranchingFunction<T>(branchingFunction),
                strategy,
                new SearchNodeStateComparer<T>(stateEquality));
        }
        public SearchSolution<T> Search(IGoalDefinition<T> goal, T initialState)
        {
            if (goal.IsTerminal(initialState))
                return new SearchSolution<T>(initialState, Array.Empty<ITraversal<T>>(), initialState);

            foreach (var node in _traverser.Traverse(new SearchNode<T>(initialState, null, null)))
            {
                if (goal.IsTerminal(node.State))
                    return new SearchSolution<T>(
                        initialState,
                        node.GetActionsToNode(),
                        node.State);
            }

            throw new NoSolutionFoundException();
        }
    }

    internal class SearchNodeBranchingFunction<T> : IBranchingFunction<SearchNode<T>>
    {
        private IBranchingFunction<T> _branchingFunction;
        internal SearchNodeBranchingFunction(IBranchingFunction<T> branchingFunction)
        {
            _branchingFunction = branchingFunction;
        }
        public IEnumerable<ITraversal<SearchNode<T>>> Branch(SearchNode<T> current)
        {
            foreach (var traversal in _branchingFunction.Branch(current.State))
                yield return new SearchTraversal<T>(traversal);
        }
    }

    internal class SearchTraversal<T> : ITraversal<SearchNode<T>>
    {
        private ITraversal<T> _stateTraversal;
        internal SearchTraversal(ITraversal<T> stateTraversal)
        {
            _stateTraversal = stateTraversal;
        }

        public double? Cost(SearchNode<T> state)
        {
            return _stateTraversal.Cost(state.State);
        }

        public SearchNode<T> Traverse(SearchNode<T> state)
        {
            return new SearchNode<T>(_stateTraversal.Traverse(state.State), state, _stateTraversal, state.Cost + _stateTraversal.Cost(state.State) ?? double.NaN);
        }
    }
}
