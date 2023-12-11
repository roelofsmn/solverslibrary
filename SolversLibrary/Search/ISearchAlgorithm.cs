using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    internal interface ISearchAlgorithm<T>
    {
        SearchSolution<T> Search(IGoalDefinition<T> problemStatement, T initialState);
    }
}
