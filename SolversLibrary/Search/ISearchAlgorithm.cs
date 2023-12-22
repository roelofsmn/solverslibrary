using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    public interface ISearchAlgorithm<T> : IAlgorithm<SearchSolution<T>>
    {
        SearchSolution<T> Search(IGoalDefinition<T> problemStatement, T initialState);

        public IGoalDefinition<T> Goal { get; set; }
        public T InitialState { get; set; }
    }

    public interface ISearchProblem<T>
    {
        IGoalDefinition<T> Goal { get; }
        T InitialState { get; }
    }
}
