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
        internal static ITraversalStrategy<T> Create<T>(TraversalStrategies strategy, CostFunction<T>? cost = null)
        {
            switch (strategy)
            {
                case TraversalStrategies.BreadthFirst:
                    return new BreadthFirstTraversalStrategy<T>();
                case TraversalStrategies.DepthFirst:
                    return new DepthFirstTraversalStrategy<T>();
                case TraversalStrategies.Priority:
                    ArgumentNullException.ThrowIfNull(cost);
                    return new PriorityTraversalStrategy<T>(cost);
                default:
                    return new DepthFirstTraversalStrategy<T>();
            }
        }
    }
}
