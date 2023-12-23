using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    internal class LinkedListStack<T>
    {
        private LinkedList<T> _stack;

        public LinkedListStack()
        {
            _stack = new LinkedList<T>();
        }

        public void Push(T item)
        {
            _stack.AddLast(item);
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }

    [MemoryDiagnoser(false)]
    public class StackBenchmark
    {
        private int item = 42;
        private Queue<int> _queue;
        private Stack<int> _stack;
        private LinkedListStack<int> _llStack;

        [Params(10000, 100000, 1000000)]
        public int N;

        [IterationSetup]
        public void Setup()
        {
            _queue = new Queue<int>();
            _stack = new Stack<int>();
            _llStack = new LinkedListStack<int>();
        }

        [IterationCleanup]
        public void Cleanup()
        {
            _queue.Clear();
            _stack.Clear();
            _llStack.Clear();
        }

        [Benchmark]
        public void Queue_Push()
        {
            for (int i = 0; i < N; i++)
                _queue.Enqueue(i);
        }

        [Benchmark]
        public void Stack_Push()
        {
            for (int i = 0; i < N; i++)
                _stack.Push(i);
        }

        [Benchmark]
        public void LinkedListStack_Push()
        {
            for (int i = 0; i < N; i++)
                _llStack.Push(i);
        }

    }
}
