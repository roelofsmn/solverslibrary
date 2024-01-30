using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Traversal
{
    public interface ITraverser<T>
    {
        /// <summary>
        /// Enumerate next states from a starting state in order.
        /// </summary>
        /// <param name="start">State to start enumerating from.</param>
        /// <returns>A sequence of states reached from the starting state.</returns>
        IEnumerable<T> Traverse(T start);

        event Action<T>? Generated;
        event Predicate<T>? Skip;
    }
}
