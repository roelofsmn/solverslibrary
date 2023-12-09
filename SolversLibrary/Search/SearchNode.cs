using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    internal class SearchNode<T>
    {
        public SearchNode(T state, SearchNode<T>? parent, ISearchAction<T>? action, double pathCost)
        {
            State = state;
            Parent = parent;
            Action = action;
            PathCost = pathCost;
        }
        internal T State { get; init; }
        internal SearchNode<T>? Parent { get; init; }

        internal ISearchAction<T>? Action { get; init; }
        internal double PathCost { get; init; }
    }
}
