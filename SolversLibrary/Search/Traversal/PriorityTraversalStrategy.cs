using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Traversal
{
    /// <summary>
    /// Cost function gives a cost based on current state, e.g. a heuristic cost to the goal.
    /// </summary>
    /// <typeparam name="T">Type of state.</typeparam>
    /// <param name="state">The state to evaluate the cost for.</param>
    /// <returns>(Estimated) Cost of state.</returns>
    public delegate double CostFunction<T>(T state);

    /// <summary>
    /// Cost of state resulting from applying an action to a state.
    /// </summary>
    /// <typeparam name="T">Type of state.</typeparam>
    /// <param name="state">The state on which the action is applied.</param>
    /// <param name="action">The action applied.</param>
    /// <returns>Cost of the state resulting from applying action to state.</returns>
    public delegate double ActionCostFunction<T>(T state, ITraversal<T> action);

    public class PriorityTraversalStrategy<T> : ITraversalStrategy<T>
    {
        //private PriorityQueue<T, double> frontier; // TODO: use this instead of custom implementation...
        private SortedDictionary<double, LinkedList<T>> frontier; // Should we make it possible to use a stack as well???
        private CostFunction<T> _costFunction;
        private double minCost;
        private IEqualityComparer<T>? _equalityComparer;
        public PriorityTraversalStrategy(
            CostFunction<T> costFunction,
            IEqualityComparer<T>? equalityComparer = null)
        {
            frontier = new SortedDictionary<double, LinkedList<T>>();
            _costFunction = costFunction;
            minCost = double.PositiveInfinity;
            _equalityComparer = equalityComparer;
        }
        public void AddCandidateState(T state)
        {
            var cost = _costFunction(state);
            if (frontier.ContainsKey(cost))
                frontier[cost].AddFirst(state);
            else
            {
                frontier.Add(cost, new LinkedList<T>(new T[] { state }));
                if (cost < minCost)
                    minCost = cost;
            }
        }

        public void Clear()
        {
            frontier.Clear();
        }

        public bool Contains(T state)
        {
            var cost = _costFunction(state);
            return frontier.ContainsKey(cost) && frontier[cost].Contains(state, _equalityComparer);
        }

        public bool ContainsCandidates()
        {
            return frontier.Count > 0;
        }

        public T NextState()
        {
            var minStates = frontier[minCost];
            if (minStates.Count == 1)
            {
                frontier.Remove(minCost);
                minCost = frontier.Any() ? frontier.Keys.Min() : double.PositiveInfinity;
                return minStates.Single();
            }
            else if (minStates.Count > 1)
            {
                return minStates.First!.Value;
            }
            else
                throw new InvalidOperationException();
        }

        public void ReplaceCandidateState(T current, T replacement)
        {
            var cost = _costFunction(current);
            if (frontier[cost].Count == 1)
                frontier.Remove(cost);
            else
                frontier[cost].Remove(current);

            AddCandidateState(replacement);
        }

        public T? Find(T state)
        {
            return frontier.Values.SelectMany(list => list).SingleOrDefault(t => _equalityComparer?.Equals(state, t) ?? t.Equals(state));
        }
    }
}
