using MathNet.Numerics.Distributions;
using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Algorithms
{
    public class AstarSearch<T> : IHeuristicSearchAlgorithm<T>
    {
        private PriorityTraversalStrategy<CostSearchNode<T>> strategy;
        private HashSet<T> _explored;
        private SearchNodeStateComparer<T> searchNodeComparer;

        public AstarSearch(IEqualityComparer<T>? stateEquality = null)
        {
            strategy = new PriorityTraversalStrategy<CostSearchNode<T>>();
            _explored = new HashSet<T>(stateEquality);
            searchNodeComparer = new SearchNodeStateComparer<T>();
        }
        public CostSearchSolution<T> Search(IHeuristicSearchProblem<T> problemStatement, T initialState)
        {
            strategy.Clear();
            _explored.Clear();

            if (problemStatement.IsTerminal(initialState))
                return new CostSearchSolution<T>(initialState, Array.Empty<ISearchAction<T>>(), initialState, 0.0);

            strategy.AddCandidateState(new CostSearchNode<T>(initialState, null, null, 0.0));

            while (strategy.ContainsCandidates())
            {
                CostSearchNode<T> node = strategy.NextState();
                if (problemStatement.IsTerminal(node.State))
                    return new CostSearchSolution<T>(
                        initialState,
                        node.GetActionsToNode(),
                        node.State,
                        node.Cost);

                _explored.Add(node.State);
                foreach (var action in problemStatement.Branch(node.State))
                {
                    var childState = action.Apply(node.State);
                    var actionCost = action.Cost(node.State);
                    var child = new CostSearchNode<T>(childState, node, action, node.Cost + actionCost);
                    CostSearchNode<T>? matchingSearchNode = strategy.Find(child, searchNodeComparer);
                    if (!_explored.Contains(childState) && matchingSearchNode == null)
                        strategy.AddCandidateState(child);
                    else if (matchingSearchNode != null 
                        && matchingSearchNode.Cost + problemStatement.Heuristic(matchingSearchNode.State) > child.Cost + problemStatement.Heuristic(child.State))
                        strategy.ReplaceCandidateState(matchingSearchNode, child);
                }
            }
            throw new NoSolutionFoundException();
        }
    }
}
