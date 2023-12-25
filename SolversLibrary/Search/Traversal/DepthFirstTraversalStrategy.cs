using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Traversal
{
    public class DepthFirstTraversalStrategy<T> : ITraversalStrategy<T>
    {
        private Stack<T> frontier;
        public DepthFirstTraversalStrategy()
        {
            frontier = new Stack<T>();
        }

        public event Action<T>? Enqueued;
        public event Action<T>? Dequeued;

        public void AddCandidateState(T state)
        {
            frontier.Push(state);
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
            T item = frontier.Pop();
            Dequeued?.Invoke(item);
            return item;
        }
    }
}
