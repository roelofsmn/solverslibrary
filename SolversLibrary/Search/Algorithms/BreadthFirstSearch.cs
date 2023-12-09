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
        public IEnumerable<SearchSolution<T>> Search(ISearchProblem<T> problemStatement, T initialState)
        {
            _frontier.Clear();
            _explored.Clear();

            if (problemStatement.IsTerminal(initialState))
                yield return new SearchSolution<T>(initialState, Array.Empty<ISearchAction<T>>(), 0.0, initialState);

            _frontier.Enqueue(new SearchNode<T>(initialState, null, null, 0.0));

            while (_frontier.Count > 0)
            {
                SearchNode<T> node = _frontier.Dequeue();
                _explored.Add(node.State);
                foreach (var action in problemStatement.Branch(node.State))
                {
                    var childState = action.Apply(node.State);
                    var actionCost = action.Cost(node.State);
                    if (!_explored.Contains(childState))
                    {
                        var child = new SearchNode<T>(childState, node, action, node.PathCost + actionCost);
                        if (problemStatement.IsTerminal(childState))
                            yield return new SearchSolution<T>(
                                initialState,
                                GetActionsToNode(child),
                                child.PathCost,
                                childState);
                        _frontier.Enqueue(child);
                    }
                }
            }
        }

        internal static ISearchAction<T>[] GetActionsToNode(SearchNode<T> node)
        {
            LinkedList<ISearchAction<T>> actions = new LinkedList<ISearchAction<T>>();
            var currentNode = node;
            while (currentNode.Parent != null)
            {
                actions.AddFirst(currentNode.Action ?? throw new NullReferenceException("Search node action is null, while parent isn't."));
                currentNode = currentNode.Parent;
            }
            return actions.ToArray();
        }
    }
}
