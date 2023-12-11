using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Traversal
{
    public interface ITraversal<T>
    {
        /// <summary>
        /// Traverse to the next state defined by this traversal.
        /// </summary>
        /// <param name="state">Current state to traverse from.</param>
        /// <returns>The next state reached.</returns>
        T Traverse(T state);

        double? Cost(T state);
    }
}
