using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using SolversLibrary.Search;
using SolversLibrary.Search.Traversal;

namespace Benchmarks
{
    public struct Item : ICost
    {
        public Item(double cost)
        {
            Cost = cost;
        }
        public double Cost { get; }
    }

    public class LIFOComparer<TPriority> : IComparer<(TPriority priority, long insertion)>
    {
        private IComparer<TPriority> _priorityComparer;
        public LIFOComparer(IComparer<TPriority>? priorityComparer = null)
        {
            _priorityComparer = priorityComparer ?? Comparer<TPriority>.Default;
        }
        public int Compare((TPriority priority, long insertion) x, (TPriority priority, long insertion) y)
        {
            var prioOrder = _priorityComparer.Compare(x.priority, y.priority);
            if (prioOrder != 0)
                return prioOrder;
            else
                return Math.Sign(y.insertion - x.insertion);
        }
    }

    [MemoryDiagnoser(false)]
    public class PriorityQueueBenchmark
    {
        private Random rnd = new Random(42);
        private PriorityQueue<Item, double> _queue;
        private PriorityQueue<Item, (double, long)> _queueLifo;

        public PriorityQueueBenchmark()
        {
            _queue = new PriorityQueue<Item, double>();
            _queueLifo = new PriorityQueue<Item, (double, long)>(new LIFOComparer<double>());
            _items = new List<Item>();
        }

        [Params(100_000, 1_000_000)]
        public int N;

        private List<Item> _items;

        [GlobalSetup]
        public void Setup()
        {
            _queue.Clear();
            _queueLifo.Clear();
            _items.Clear();
            for (int i = 0; i < N; i++)
                _items.Add(new Item(rnd.Next(0, 100)));
        }

        [IterationCleanup]
        public void Cleanup()
        {
            _queue.Clear();
            _queueLifo.Clear();
        }

        [Benchmark(Baseline = true)]
        public void Queue_Push()
        {
            foreach (var item in _items)
                _queue.Enqueue(item, item.Cost);
        }

        [Benchmark]
        public void QueueLifo_Push()
        {
            int insertion = 0;
            foreach (var item in _items)
                _queueLifo.Enqueue(item, (item.Cost, insertion++));
        }

    }
}
