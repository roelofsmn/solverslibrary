using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Traversal
{
    public interface ITraversal<T>
    {
        IEnumerable<T> Traverse(T start);
    }
}
