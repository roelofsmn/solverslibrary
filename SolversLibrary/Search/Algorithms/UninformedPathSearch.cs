using SolversLibrary.Search.Factories;
using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;

namespace SolversLibrary.Search.Algorithms
{
    public class UninformedPathSearch<T> : ISearchAlgorithm<T>
    {
        private ITraverser<PathSearchState<T>> _traverser;

        public UninformedPathSearch(
            TraversalStrategies strategyType,
            Traversers traverseType,
            IBranchingFunction<T> branchingFunction,
            IEqualityComparer<T>? stateEquality = null)
        {
            var strategy = TraversalStrategyFactory.Create<PathSearchState<T>>(strategyType);
            _traverser = TraverserFactory.Create<PathSearchState<T>>(traverseType,
                new PathSearchBranchingFunction<T>(branchingFunction,
                    (parentState, parentCost, traversal) => parentCost + traversal.Cost(parentState) ?? double.NaN),
                strategy,
                new SearchNodeStateComparer<T>(stateEquality));
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
            if (goal.IsTerminal(initialState))
                return new SearchSolution<T>(initialState, Array.Empty<ITraversal<T>>(), initialState);

            foreach (var node in _traverser.Traverse(new PathSearchState<T>(initialState, null, null)))
            {
                ProgressUpdated?.Invoke(new SearchSolution<T>(
                    initialState,
                    node.GetActionsToNode(),
                    node.State));

                if (goal.IsTerminal(node.State))
                    return new SearchSolution<T>(
                        initialState,
                        node.GetActionsToNode(),
                        node.State);

            }

            throw new NoSolutionFoundException();
        }
    }

    internal class PathSearchBranchingFunction<T> : IBranchingFunction<PathSearchState<T>>
    {
        private IBranchingFunction<T> _branchingFunction;
        private readonly Func<T, double, ITraversal<T>, double> _newStateCost;

        internal PathSearchBranchingFunction(
            IBranchingFunction<T> branchingFunction,
            Func<T, double, ITraversal<T>, double> newStateCost)
        {
            _branchingFunction = branchingFunction;
            _newStateCost = newStateCost;
        }
        public virtual IEnumerable<ITraversal<PathSearchState<T>>> Branch(PathSearchState<T> current)
        {
            foreach (var traversal in _branchingFunction.Branch(current.State))
                yield return new PathSearchTraversal<T>(traversal, _newStateCost);
        }
    }

    internal class PathSearchTraversal<T> : ITraversal<PathSearchState<T>>
    {
        private ITraversal<T> _stateTraversal;
        private readonly Func<T, double, ITraversal<T>, double> _newStateCost;

        internal PathSearchTraversal(
            ITraversal<T> stateTraversal,
            Func<T, double, ITraversal<T>, double> newStateCost)
        {
            _stateTraversal = stateTraversal;
            _newStateCost = newStateCost;
        }

        public double? Cost(PathSearchState<T> state)
        {
            return _stateTraversal.Cost(state.State);
        }

        public PathSearchState<T> Traverse(PathSearchState<T> state)
        {
            return new PathSearchState<T>(
                _stateTraversal.Traverse(state.State), 
                state, 
                _stateTraversal,
                _newStateCost(state.State, state.Cost, _stateTraversal));
        }
    }
}
