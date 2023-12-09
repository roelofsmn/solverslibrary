using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    public interface ISearchProblem<T>
    {
        IEnumerable<ISearchAction<T>> Branch(T state);
        bool IsTerminal(T state);
    }
}
