using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    public interface ISearchProblem<T> : IExploreFunction<T, ISearchAction<T>>
    {
        bool IsTerminal(T state);
    }

    public interface ICostSearchProblem<T> : IExploreFunction<T, ICostSearchAction<T>>
    {
        bool IsTerminal(T state);
    }
}
