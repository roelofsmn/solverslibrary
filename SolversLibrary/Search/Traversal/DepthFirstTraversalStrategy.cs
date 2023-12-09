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
        public void AddCandidateState(T state)
        {
            frontier.Push(state);
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
            return frontier.Pop();
        }

        public void ReplaceCandidateState(T current, T replacement)
        {
            throw new NotImplementedException();
        }
    }
}
