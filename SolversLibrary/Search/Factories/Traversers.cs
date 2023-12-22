using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Factories
{
    public enum Traversers
    {
        /// <summary>
        /// Graph traversal; keeps an explored set to prevent examining the same state multiple times.
        /// </summary>
        Graph,
        /// <summary>
        /// Tree traversal; assumes the branching function is a tree, and therefore, repeated states do not exist.
        /// </summary>
        Tree
    }
}
