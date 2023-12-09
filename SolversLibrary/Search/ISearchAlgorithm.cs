using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    internal interface ISearchAlgorithm<T>
    {
        IEnumerable<SearchSolution<T>> Search(ISearchProblem<T> problemStatement, T initialState);
    }
}
