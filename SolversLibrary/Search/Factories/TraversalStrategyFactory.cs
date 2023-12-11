using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Factories
{
    internal static class TraversalStrategyFactory
    {
        internal static ITraversalStrategy<T> Create<T>(TraversalStrategies strategy)
        {
            switch (strategy)
            {
                case TraversalStrategies.BreadthFirst:
                    return new BreadthFirstTraversalStrategy<T>();
                case TraversalStrategies.DepthFirst:
                    return new DepthFirstTraversalStrategy<T>();
                case TraversalStrategies.Priority:
                    throw new ArgumentException($"Strategy {TraversalStrategies.Priority} not supported!");
                default:
                    return new DepthFirstTraversalStrategy<T>();
            }
        }
    }
}
