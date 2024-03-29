﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    public interface ISearchAlgorithm<T>
    {
        T Search(IGoalDefinition<T> problemStatement, T initialState);
        event Action<T>? Explored;
        event Action<T>? FoundSolution;
    }

    public interface IGoalDefinition<T>
    {
        bool IsTerminal(T state);
    }

    public interface IHeuristic<T>
    {
        /// <summary>
        /// A heuristic cost from state to terminal state.
        /// For tree search, it should be admissible.
        /// For graph search, it should be consistent (and therefore also admissible).
        /// Admissible means it never overestimates the true cost to reach the goal.
        /// Consistent means that h(n) <= c(n, a, n') + h(n'), where h is the heuristic and c is the cost of moving from state n to n'.
        /// </summary>
        /// <param name="state">State to compute the heuristic (future) cost for.</param>
        /// <returns>The estimated cost to reach the goal from the given state.</returns>
        double Heuristic(T state);
    }
}
