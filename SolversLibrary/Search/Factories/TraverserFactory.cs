using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Factories
{
    internal static class TraverserFactory
    {
        internal static ITraverser<T> Create<T>(
            Traversers traverser,
            IBranchingFunction<T> exploreFunction,
            ITraversalStrategy<T> strategy,
            IEqualityComparer<T>? stateEquality = null)
        {
            switch (traverser)
            {
                case Traversers.Graph:
                    return new GraphTraverser<T>(exploreFunction, strategy, stateEquality);
                case Traversers.Tree:
                    return new TreeTraverser<T>(exploreFunction, strategy);
                default:
                    return new GraphTraverser<T>(exploreFunction, strategy, stateEquality);
            }
        }
    }
}
