using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    public class SearchNode<T>
    {
        public SearchNode(T state, SearchNode<T>? parent, ITraversal<T>? action, double? cost = null)
        {
            State = state;
            Parent = parent;
            Action = action;
            _cost = cost;
        }
        internal T State { get; init; }
        internal SearchNode<T>? Parent { get; init; }
        internal ITraversal<T>? Action { get; init; }
        private double? _cost;
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
