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
    // Difference between A* search and Branch-and-Bound optimization is that B&B does not have a definition for IsTerminal.
    // In other words, B&B only considers the objective function, whereas A* knows of a certain goal state to reach.
    public class AstarSearch<T> : UniformCostSearch<T>
    {
        public AstarSearch(
            Traversers traverseType,
            IBranchingFunction<T> branchingFunction,
            Func<T, double> stateCost,
            Func<T, double> heuristic,
            IEqualityComparer<T>? stateEquality = null)
            : base(traverseType, branchingFunction, x => stateCost(x) + heuristic(x), stateEquality)
        {
        }
    }
}
