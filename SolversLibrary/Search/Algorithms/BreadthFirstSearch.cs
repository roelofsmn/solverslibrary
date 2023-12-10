using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Algorithms
{
    public class BreadthFirstSearch<T> : ISearchAlgorithm<T>
    {
        private Queue<SearchNode<T>> _frontier;
        private HashSet<T> _explored;

        public BreadthFirstSearch(IEqualityComparer<T>? stateEquality = null)
        {
            _frontier = new Queue<SearchNode<T>>();
            _explored = new HashSet<T>(stateEquality);
        }
        public SearchSolution<T> Search(ISearchProblem<T> problemStatement, T initialState)
        {
            _frontier.Clear();
            _explored.Clear();

            if (problemStatement.IsTerminal(initialState))
                return new SearchSolution<T>(initialState, Array.Empty<ISearchAction<T>>(), initialState);

            _frontier.Enqueue(new SearchNode<T>(initialState, null, null));

            while (_frontier.Count > 0)
            {
                SearchNode<T> node = _frontier.Dequeue();
                _explored.Add(node.State);
                foreach (var action in problemStatement.Branch(node.State))
                {
                    var childState = action.Apply(node.State);
                    if (!_explored.Contains(childState))
                    {
                        var child = new SearchNode<T>(childState, node, action);
                        if (problemStatement.IsTerminal(childState))
                            return new SearchSolution<T>(
                                initialState,
                                child.GetActionsToNode(),
                                childState);
                        _frontier.Enqueue(child);
                    }
                }
            }
            throw new NoSolutionFoundException();
        }
    }
}
