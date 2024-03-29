﻿using System;
using System.Collections.Generic;

namespace SolversLibrary.Search.Traversal
{
    public class GraphTraverser<T> : ITraverser<T>
    {
        private readonly IBranchingFunction<T> branchingFunction;
        private readonly ITraversalStrategy<T> strategy;
        private readonly HashSet<T> explored;

        /// <summary>
        /// Graph traversal algorithm. The graph is implicitly defined in the traversal function.
        /// </summary>
        /// <param name="generator">A branching function that generates next states from a current state.</param>
        /// <param name="strategy">A traversal strategy that decides which states to explore next.</param>
        /// <param name="stateEquality">An equality comparer used to compare states, to keep track of whether a state was explored already or not.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>We could have used an enum for the strategy, and a factory to create a new strategy object on every Traverse call. Instead, we opted for the Clear() method on the strategy interface, because this reuses the same object (hence less GC).</remarks>
        public GraphTraverser(
            IBranchingFunction<T> generator,
            ITraversalStrategy<T> strategy,
            IEqualityComparer<T>? stateEquality = null)
        {
            this.branchingFunction = generator ?? throw new ArgumentNullException(nameof(generator));
            this.strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            explored = new HashSet<T>(stateEquality);
        }

        public event Action<T>? Generated;
        public event Predicate<T>? Skip;

        public IEnumerable<T> Traverse(T start)
        {
            strategy.Clear();
            explored.Clear();

            strategy.AddCandidateState(start);

            while (strategy.ContainsCandidates())
            {
                T node = strategy.NextState();
                yield return node;

                if (Skip?.Invoke(node) ?? false)
                    continue;

                explored.Add(node);

                foreach (var traversal in branchingFunction.Branch(node))
                {
                    var child = traversal.Traverse(node);
                    Generated?.Invoke(child);

                    if (!explored.Contains(child) && !strategy.Contains(child))
                        strategy.AddCandidateState(child);
                }
            }
        }
    }
}
