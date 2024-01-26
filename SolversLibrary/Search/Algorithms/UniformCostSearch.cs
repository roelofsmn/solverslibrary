using SolversLibrary.Search.Factories;
using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;

namespace SolversLibrary.Search.Algorithms
{
    public class UniformCostSearch<T> : ISearchAlgorithm<T>
    {
        private PriorityTraversalStrategy<T> _strategy;
        private ITraverser<T> _traverser;

        private BestFirstSearch<T> _search;

        public UniformCostSearch(
            Traversers traverserType,
            IBranchingFunction<T> branchingFunction,
            Func<T, double> costFunction,
            IEqualityComparer<T>? stateEquality = null)
        {
            _strategy = new PriorityTraversalStrategy<T>(
                costFunction,
                stateEquality);

            _traverser = TraverserFactory.Create<T>(
                traverserType,
                branchingFunction,
                _strategy,
                stateEquality);

            _search = new BestFirstSearch<T>(_traverser);
            _search.Explored += (n) => Explored?.Invoke(n); // Just bubble up the event
        }

        public event Action<T>? Explored;

        public T Search(IGoalDefinition<T> goal, T initialState)
        {
            return _search.Search(goal, initialState);
        }
    }
}
