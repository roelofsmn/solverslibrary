using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    public class SearchSolution<T>
    {
        public SearchSolution(T initialState, ISearchAction<T>[] actionPath, T terminalState)
        {
            InitialState = initialState;
            ActionPath = actionPath;
            TerminalState = terminalState;
        }
        public T InitialState { get; init; }
        public ISearchAction<T>[] ActionPath { get; init; }
        public T TerminalState { get; init; }
    }

    public class CostSearchSolution<T> : SearchSolution<T>, ICost
    {
        public CostSearchSolution(T initialState, ISearchAction<T>[] actionPath, T terminalState, double totalCost) : base(initialState, actionPath, terminalState)
        {
            _totalCost = totalCost;
        }

        private double _totalCost;
        public double Cost => _totalCost;
    }
}
