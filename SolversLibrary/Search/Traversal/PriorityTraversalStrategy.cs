using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Traversal
{
    public class PriorityTraversalStrategy<T> : ITraversalStrategy<T> where T : ICost
    {
        private SortedDictionary<double, LinkedList<T>> frontier; // Should we make it possible to use a stack as well???
        private double minCost;
        public PriorityTraversalStrategy()
        {
            frontier = new SortedDictionary<double, LinkedList<T>>();
            minCost = double.PositiveInfinity;
        }
        public void AddCandidateState(T state)
        {
            if (frontier.ContainsKey(state.Cost))
                frontier[state.Cost].AddLast(state);
            else
            {
                frontier.Add(state.Cost, new LinkedList<T>(new T[] { state }));
                if (state.Cost < minCost)
                    minCost = state.Cost;
            }
        }

        public void Clear()
        {
            frontier.Clear();
        }

        public bool Contains(T state)
        {
            return frontier.ContainsKey(state.Cost) && frontier[state.Cost].Contains(state);
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
            if (frontier[current.Cost].Count == 1)
                frontier.Remove(current.Cost);
            else
                frontier[current.Cost].Remove(current);

            AddCandidateState(replacement);
        }

        public T? Find(T state, IEqualityComparer<T> comparer)
        {
            return frontier.Values.SelectMany(list => list).SingleOrDefault(t => comparer.Equals(state, t));
        }
    }
}
