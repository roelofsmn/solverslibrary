using SolversLibrary.Search.Factories;
using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;

namespace SolversLibrary.Search.Algorithms
{
    public class BestFirstSearch<T> : ISearchAlgorithm<T>
    {
        private ITraverser<T> _traverser;

        public BestFirstSearch(
            ITraverser<T> traverser)
        {
            _traverser = traverser;
        }

        public event Action<T>? Explored;
        public event Action<T>? FoundSolution;

        public T Search(IGoalDefinition<T> goal, T initialState)
        {
            if (goal.IsTerminal(initialState))
                return initialState;

            foreach (var node in _traverser.Traverse(initialState))
            {
                Explored?.Invoke(node);
                if (goal.IsTerminal(node))
                {
                    FoundSolution?.Invoke(node);
                    return node;
                }
            }

            throw new NoSolutionFoundException();
        }
    }
}
