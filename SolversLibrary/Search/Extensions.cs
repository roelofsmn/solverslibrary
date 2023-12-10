using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    internal static class Extensions
    {
        internal static ISearchAction<T>[] GetActionsToNode<T>(this SearchNode<T> node)
        {
            LinkedList<ISearchAction<T>> actions = new LinkedList<ISearchAction<T>>();
            var currentNode = node;
            while (currentNode.Parent != null)
            {
                actions.AddFirst(currentNode.Action ?? throw new NullReferenceException("Search node action is null, while parent isn't."));
                currentNode = currentNode.Parent;
            }
            return actions.ToArray();
        }
    }
}
