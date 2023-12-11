using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    public class SearchSolution<T>
    {
        public SearchSolution(T initialState, ITraversal<T>[] actionPath, T terminalState, double? cost = null)
        {
            InitialState = initialState;
            ActionPath = actionPath;
            TerminalState = terminalState;
            Cost = cost;
        }
        public T InitialState { get; init; }
        public ITraversal<T>[] ActionPath { get; init; }
        public T TerminalState { get; init; }
        public double? Cost { get; }
    }
}
