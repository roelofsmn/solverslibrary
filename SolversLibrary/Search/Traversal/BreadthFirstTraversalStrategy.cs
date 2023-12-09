using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Traversal
{
    public class BreadthFirstTraversalStrategy<T> : ITraversalStrategy<T>
    {
        private Queue<T> frontier;
        public BreadthFirstTraversalStrategy()
        {
            frontier = new Queue<T>();
        }
        public void AddCandidateState(T state)
        {
            frontier.Enqueue(state);
        }

        public void Clear()
        {
            frontier.Clear();
        }

        public bool Contains(T state)
        {
            return frontier.Contains(state);
        }

        public bool ContainsCandidates()
        {
            return frontier.Count > 0;
        }

        public T NextState()
        {
            return frontier.Dequeue();
        }

        public void ReplaceCandidateState(T current, T replacement)
        {
            throw new NotImplementedException();
        }
    }
}
