using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    public struct SearchSolution<T>
    {
        public SearchSolution(T initialState, ISearchAction<T>[] actionPath, double totalCost, T terminalState)
        {
            InitialState = initialState;
            ActionPath = actionPath;
            TotalCost = totalCost;
            TerminalState = terminalState;
        }
        public readonly T InitialState { get; init; }
        public readonly ISearchAction<T>[] ActionPath { get; init; }
        public readonly double TotalCost { get; init; }
        public readonly T TerminalState { get; init; }
    }
}
