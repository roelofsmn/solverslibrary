using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    public class SearchNode<T>
    {
        public SearchNode(T state, SearchNode<T>? parent, ISearchAction<T>? action)
        {
            State = state;
            Parent = parent;
            Action = action;
        }
        internal T State { get; init; }
        internal SearchNode<T>? Parent { get; init; }

        internal ISearchAction<T>? Action { get; init; }
    }

    public class CostSearchNode<T> : SearchNode<T>, ICost
    {
        public CostSearchNode(T state, SearchNode<T>? parent, ISearchAction<T>? action, double cost) : base(state, parent, action)
        {
            _cost = cost;
        }

        private double _cost;
        public double Cost => _cost;
    }
}
