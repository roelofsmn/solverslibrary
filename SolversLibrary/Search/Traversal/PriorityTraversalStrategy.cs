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
        private PriorityQueue<T, double> frontier; // TODO: use this instead of custom implementation...
        //private SortedDictionary<double, LinkedList<T>> frontier; // Should we make it possible to use a stack as well???
        private Func<T, double> _costFunction;
        private IEqualityComparer<T> _equalityComparer;

        public event Action<T>? Enqueued;
        public event Action<T>? Dequeued;

        public PriorityTraversalStrategy(
            Func<T, double> costFunction,
            IEqualityComparer<T>? equalityComparer = null)
        {
            frontier = new PriorityQueue<T, double>();
            _costFunction = costFunction;
            _equalityComparer = equalityComparer ?? EqualityComparer<T>.Default;
        }
        public void AddCandidateState(T state)
        {
            frontier.Enqueue(state, _costFunction(state));
            Enqueued?.Invoke(state);
        }

        public void Clear()
        {
            frontier.Clear();
        }

        public bool Contains(T state)
        {
            return frontier.UnorderedItems.SingleOrDefault(tuple => _equalityComparer.Equals(tuple.Element, state)).Element != null;
        }

        public bool ContainsCandidates()
        {
            return frontier.Count > 0;
        }

        public T NextState()
        {
            var item = frontier.Dequeue();
            Dequeued?.Invoke(item);
            return item;
        }
    }
}
