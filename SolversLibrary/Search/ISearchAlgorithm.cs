using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    internal interface ISearchAlgorithm<T>
    {
        SearchSolution<T> Search(ISearchProblem<T> problemStatement, T initialState);
    }

    internal interface ICostSearchAlgorithm<T>
    {
        CostSearchSolution<T> Search(ICostSearchProblem<T> problemStatement, T initialState);
    }
    internal interface IHeuristicSearchAlgorithm<T>
    {
        CostSearchSolution<T> Search(IHeuristicSearchProblem<T> problemStatement, T initialState);
    }

}
