using MathNet.Numerics.Distributions;
using SolversLibrary.Search.Factories;
using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Algorithms
{
    public class UniformCostSearch<T> : ISearchAlgorithm<T>
    {
        private PriorityTraversalStrategy<PathSearchState<T>> _strategy;
        private ITraverser<PathSearchState<T>> _traverser;

        public UniformCostSearch(
            Traversers traverseType,
            IBranchingFunction<T> branchingFunction,
            IEqualityComparer<T>? stateEquality = null)
            : this(traverseType, branchingFunction, x => x.Cost, stateEquality)
        {
        }

        internal UniformCostSearch(
            Traversers traverseType,
            IBranchingFunction<T> branchingFunction,
            CostFunction<PathSearchState<T>> costFunction,
            IEqualityComparer<T>? stateEquality = null)
        {
            var searchNodeComparer = new SearchNodeStateComparer<T>(stateEquality);

            _strategy = new PriorityTraversalStrategy<PathSearchState<T>>(
                costFunction,
                searchNodeComparer);

            _traverser = TraverserFactory.Create<PathSearchState<T>>(
                traverseType,
                new PathSearchBranchingFunction<T>(branchingFunction,
                    (parentState, parentCost, traversal) => parentCost + traversal.Cost(parentState) ?? double.NaN),
                _strategy,
                searchNodeComparer);
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
                return new SearchSolution<T>(initialState, Array.Empty<ITraversal<T>>(), initialState, 0.0);

            foreach (var node in _traverser.Traverse(new PathSearchState<T>(initialState, null, null, 0.0)))
            {
                ProgressUpdated?.Invoke(new SearchSolution<T>(
                    initialState,
                    node.GetActionsToNode(),
                    node.State,
                    node.Cost));

                if (goal.IsTerminal(node.State))
                    return new SearchSolution<T>(
                        initialState,
                        node.GetActionsToNode(),
                        node.State,
                        node.Cost);

                if (_strategy.Contains(node))
                {
                    var match = _strategy.Find(node)!;
                    if (match.Cost > node.Cost)
                        _strategy.ReplaceCandidateState(match, node);
                }
            }
            throw new NoSolutionFoundException();
        }
    }
}
