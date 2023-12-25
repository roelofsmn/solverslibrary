using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolversLibrary.Search
{
    /// <summary>
    /// A node on a search path, that holds a state and from which parent--action pair it was generated.
    /// </summary>
    /// <typeparam name="T">The type of state for the search.</typeparam>
    public class PathSearchState<T>
    {
        public PathSearchState(T state, PathSearchState<T>? parent, ITraversal<T>? action, double? cost = null)
        {
            State = state;
            Parent = parent;
            Action = action;
            _cost = cost;
        }
        /// <summary>
        /// The actual state represented by this point in the search path.
        /// </summary>
        internal T State { get; init; }

        /// <summary>
        /// The parent search node from which this node was generated.
        /// </summary>
        internal PathSearchState<T>? Parent { get; init; }

        /// <summary>
        /// The action applied to the previous state to arrive at this one.
        /// </summary>
        internal ITraversal<T>? Action { get; init; }

        private double? _cost;
        /// <summary>
        /// The cost associated with this state at this point in the search.
        /// </summary>
        public double Cost => _cost ?? double.NaN;
    }

    internal class PathSearchNodeStateComparer<T> : IEqualityComparer<PathSearchState<T>>
    {
        private IEqualityComparer<T>? stateEqualityComparer;
        internal PathSearchNodeStateComparer(IEqualityComparer<T>? stateEquality = null)
        {
            stateEqualityComparer = stateEquality;
        }
        public bool Equals(PathSearchState<T>? x, PathSearchState<T>? y)
        {
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null) || ReferenceEquals(x.State, null))
                return false;
            return stateEqualityComparer?.Equals(x.State, y.State) ?? x.State.Equals(y.State);
        }

        public int GetHashCode([DisallowNull] PathSearchState<T> obj)
        {
            return obj.GetHashCode();
        }
    }

    internal class PathSearchGoal<T> : IGoalDefinition<PathSearchState<T>>
    {
        private readonly IGoalDefinition<T> _goal;

        public PathSearchGoal(IGoalDefinition<T> goal)
        {
            this._goal = goal;
        }
        public bool IsTerminal(PathSearchState<T> state)
        {
            return _goal.IsTerminal(state.State);
        }
    }

    internal class PathSearchBranchingFunction<T> : IBranchingFunction<PathSearchState<T>>
    {
        private IBranchingFunction<T> _branchingFunction;
        private readonly Func<T, double, ITraversal<T>, double> _newStateCost;

        internal PathSearchBranchingFunction(
            IBranchingFunction<T> branchingFunction,
            Func<T, double, ITraversal<T>, double> newStateCost)
        {
            _branchingFunction = branchingFunction;
            _newStateCost = newStateCost;
        }
        public virtual IEnumerable<ITraversal<PathSearchState<T>>> Branch(PathSearchState<T> current)
        {
            foreach (var traversal in _branchingFunction.Branch(current.State))
                yield return new PathSearchTraversal<T>(traversal, _newStateCost);
        }
    }

    internal class PathSearchTraversal<T> : ITraversal<PathSearchState<T>>
    {
        private ITraversal<T> _stateTraversal;
        private readonly Func<T, double, ITraversal<T>, double> _newStateCost;

        internal PathSearchTraversal(
            ITraversal<T> stateTraversal,
            Func<T, double, ITraversal<T>, double> newStateCost)
        {
            _stateTraversal = stateTraversal;
            _newStateCost = newStateCost;
        }

        public double? Cost(PathSearchState<T> state)
        {
            return _stateTraversal.Cost(state.State);
        }

        public PathSearchState<T> Traverse(PathSearchState<T> state)
        {
            return new PathSearchState<T>(
                _stateTraversal.Traverse(state.State),
                state,
                _stateTraversal,
                _newStateCost(state.State, state.Cost, _stateTraversal));
        }
    }
}
