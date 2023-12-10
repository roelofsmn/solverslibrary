using System;
using System.Collections.Generic;

namespace SolversLibrary.Search.Traversal
{
    public class TreeTraversal<T> : ITraversal<T>
    {
        private readonly IExploreFunction<T, T> branchingFunction;
        private readonly ITraversalStrategy<T> strategy;

        /// <summary>
        /// Tree traversal algorithm. The tree is implicitly defined in the traversal function.
        /// </summary>
        /// <param name="generator">A branching function that generates next states from a current state.</param>
        /// <param name="strategy">A traversal strategy that decides which states to explore next.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>We could have used an enum for the strategy, and a factory to create a new strategy object on every Traverse call. Instead, we opted for the Clear() method on the strategy interface, because this reuses the same object (hence less GC).</remarks>
        public TreeTraversal(
            IExploreFunction<T, T> generator,
            ITraversalStrategy<T> strategy)
        {
            this.branchingFunction = generator ?? throw new ArgumentNullException(nameof(generator));
            this.strategy = strategy;
        }

        public IEnumerable<T> Traverse(T start)
        {
            strategy.Clear();

            strategy.AddCandidateState(start);

            while (strategy.ContainsCandidates())
            {
                T node = strategy.NextState();
                yield return node;
               
                foreach (var child in branchingFunction.Branch(node))
                    strategy.AddCandidateState(child);
            }
        }
    }
}
