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

        public event Action<T>? Enqueued;
        public event Action<T>? Dequeued;

        public void AddCandidateState(T state)
        {
            frontier.Enqueue(state);
            Enqueued?.Invoke(state);
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
            T item = frontier.Dequeue();
            Dequeued?.Invoke(item);
            return item;
        }
    }
}
