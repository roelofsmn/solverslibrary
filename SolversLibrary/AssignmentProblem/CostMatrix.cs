using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solvers.AssignmentProblem
{
    public struct CostMatrix<T>
    {

        double[,] _costs;
        Func<T, T, double> _substitutionCost;
        Func<T, double> _insertionCost, _deletionCost;
        IList<T> _items1, _items2;

        public CostMatrix(IList<T> e1, IList<T> e2, EditOptions<T> options)
        {
            _costs = new double[e1.Count() + 2, e2.Count() + 2];
            _substitutionCost = options.SubstitutionCost;
            _insertionCost = options.InsertionCost;
            _deletionCost = options.DeletionCost;
            _items1 = e1;
            _items2 = e2;
            ComputeCosts();
        }

        private void ComputeCosts()
        {
            int m = _costs.GetLength(0);
            int n = _costs.GetLength(1);
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                {
                    if (i < m - 2 && j < n - 2)
                        _costs[i, j] = _substitutionCost(_items1[i], _items2[j]);
                    else if (i >= m - 2 && j >= n - 2)
                        _costs[i, j] = double.PositiveInfinity;
                    else if (i == m - 2) // insertions of g2 items
                        _costs[i, j] = _insertionCost(_items2[j]);
                    else if (i == m - 1) // deletions of g2 items
                        _costs[i, j] = _deletionCost(_items2[j]);
                    else if (j == n - 2) // insertions of g1 items
                        _costs[i, j] = _insertionCost(_items1[i]);
                    else if (j == n - 1) // deletions of g1 items
                        _costs[i, j] = _deletionCost(_items1[i]);
                }
        }

        public double[,] C
        {
            get { return _costs; }
        }
        public double this[int i, int j]
        {
            get { return _costs[i, j]; }
        }

        public double[,] this[int i, int j, int m, int n]
        {
            get
            {
                var result = new double[m, n];
                for (int x = i; x < i + m; x++)
                    for (int y = j; y < j + n; y++)
                        result[x - i, y - j] = _costs[x, y];
                return result;
            }
        }
    }
}
