using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    internal static class Extensions
    {
        internal static ITraversal<T>[] GetActionsToNode<T>(this PathSearchState<T> node)
        {
            LinkedList<ITraversal<T>> actions = new LinkedList<ITraversal<T>>();
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
