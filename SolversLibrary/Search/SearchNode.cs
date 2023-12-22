using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    /// <summary>
    /// A node on a search path, that holds a state and from which parent--action pair it was generated.
    /// </summary>
    /// <typeparam name="T">The type of state for the search.</typeparam>
    public class SearchNode<T>
    {
        public SearchNode(T state, SearchNode<T>? parent, ITraversal<T>? action, double? cost = null)
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
        internal SearchNode<T>? Parent { get; init; }

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

    internal class SearchNodeStateComparer<T> : IEqualityComparer<SearchNode<T>>
    {
        private IEqualityComparer<T>? stateEqualityComparer;
        internal SearchNodeStateComparer(IEqualityComparer<T>? stateEquality = null)
        {
            stateEqualityComparer = stateEquality;
        }
        public bool Equals(SearchNode<T>? x, SearchNode<T>? y)
        {
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null) || ReferenceEquals(x.State, null))
                return false;
            return stateEqualityComparer?.Equals(x.State, y.State) ?? x.State.Equals(y.State);
        }

        public int GetHashCode([DisallowNull] SearchNode<T> obj)
        {
            return obj.GetHashCode();
        }
    }
}
