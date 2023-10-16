using System;
using System.Collections.Generic;
using System.Linq;
using Solvers.Extensions;

namespace Solvers.AssignmentProblem
{
    public static class AssignmentProblem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="costMatrix"></param>
        /// <returns></returns>
        /// <see cref="https://en.wikipedia.org/wiki/Hungarian_algorithm"/> 
        /// <see cref="http://www.hungarianalgorithm.com/"/>
        /// <see cref="Riesen2009 - Approximate graph edit distance computation by means of bipartite graph matching"/>
        public static IEnumerable<Tuple<int, int>> HungarianAlgorithm(double[,] costMatrix)
        {
            var finiteCosts = costMatrix.Cast<double>().Where(i => !double.IsInfinity(i));
            if (!finiteCosts.Any())
                return new Tuple<int, int>[0];

            var maxVal = finiteCosts.Max();
            var infRepl = (10 + maxVal) * costMatrix.GetLength(0) * costMatrix.GetLength(1);

            var newCostMatrix = new double[costMatrix.GetLength(0), costMatrix.GetLength(1)]; ;
            for (int i = 0; i < costMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < costMatrix.GetLength(1); j++)
                {
                    if (double.IsInfinity(costMatrix[i, j]) || double.IsNaN(costMatrix[i, j]))
                        newCostMatrix[i, j] = infRepl;
                    else
                        newCostMatrix[i, j] = costMatrix[i, j];
                }
            }
            var s = Start(new State(newCostMatrix));

            return s.Marked
                .Select(m => new Tuple<int, int>(m.Item1, m.Item2)) // Select only the indices, not the mark
                .Where(m => m.Item1 < costMatrix.GetLength(0) && m.Item2 < costMatrix.GetLength(1))
                .Where(m => !(double.IsInfinity(costMatrix[m.Item1, m.Item2]) || double.IsNaN(costMatrix[m.Item1, m.Item2]))); // Filter out indices outside original array, if it was non-square
        }

        /// <summary>
        /// <see cref="_hungarian.py from scipy.optimize"/>
        /// </summary>
        private class State
        {
            public State(double[,] costMatrix)
            {
                var m = costMatrix.GetLength(0);
                var n = costMatrix.GetLength(1);

                if (m != n)
                {
                    N = Math.Max(m, n);
                    CostMatrix = new double[N, N];
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < N; j++)
                            if (i < m && j < n)
                                CostMatrix[i, j] = costMatrix[i, j];
                }
                else
                {
                    N = n;
                    CostMatrix = new double[N, N];
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < N; j++)
                            CostMatrix[i, j] = costMatrix[i, j];
                }

                CoveredRows = new bool[N];
                CoveredColumns = new bool[N];
                Marked = new List<Tuple<int, int, Marks>>();
            }
            public int N { get; }
            public bool[] CoveredRows
            { get; set; }
            public bool[] CoveredColumns
            { get; set; }
            public List<Tuple<int, int, Marks>> Marked
            { get; set; }

            public double[,] CostMatrix
            { get; private set; }

            public void ClearCovers()
            {
                CoveredRows = new bool[N];
                CoveredColumns = new bool[N];
            }

            public void SubtractRowMinima()
            {
                CostMatrix = CostMatrix.SubtractFromRows(CostMatrix.RowMinima());
            }
            public void SubtractColumnMinima()
            {
                CostMatrix = CostMatrix.SubtractFromColumns(CostMatrix.ColumnMinima());
            }
        }

        private enum Marks
        {
            Star,
            Prime
        }
        private static State Start(State state)
        {
            state.SubtractRowMinima();
            state.SubtractColumnMinima();

            var zeros = new List<Tuple<int, int>>();
            for (int i = 0; i < state.N; i++)
                for (int j = 0; j < state.N; j++)
                    if (state.CostMatrix[i, j] == 0.0 && !(state.CoveredRows[i] || state.CoveredColumns[j]))
                    {
                        state.Marked.Add(new Tuple<int, int, Marks>(i, j, Marks.Star));
                        state.CoveredRows[i] = true;
                        state.CoveredColumns[j] = true;
                    }

            state.ClearCovers();
            return Step1(state);
        }

        private static State Step1(State state)
        {
            // Mark each column with a starred zero as covered
            foreach (var t in state.Marked.Where(t => t.Item3 == Marks.Star))
                state.CoveredColumns[t.Item2] = true;

            var coveredColumns = state.CoveredColumns.Count(b => b);
            // If amount of covered columns equals total number of columns, optimal assignment was found: return
            if (coveredColumns == state.N)
                return state;
            else // continue with step 2
                return Step2(state);
        }
        private static State Step2(State state)
        {
            double e_min = double.MaxValue;

            //bool uncovered_zero_found = false;
            for (int i = 0; i < state.N; i++)
                for (int j = 0; j < state.N; j++)
                    if (state.CostMatrix[i, j] == 0.0 && !state.CoveredRows[i] && !state.CoveredColumns[j])
                    {
                        state.Marked.Add(new Tuple<int, int, Marks>(i, j, Marks.Prime));

                        //uncovered_zero_found = true;
                        if (!state.Marked.Where(t => t.Item1 == i).Any(t => t.Item3 == Marks.Star))
                            return Step3(state);
                        else
                        {
                            state.CoveredRows[i] = true;
                            var star = state.Marked.Single(t => t.Item1 == i && t.Item3 == Marks.Star);
                            state.CoveredColumns[star.Item2] = false;
                            return Step2(state);
                        }
                    }
                    else if (state.CostMatrix[i, j] < e_min && !state.CoveredRows[i] && !state.CoveredColumns[j])
                        e_min = state.CostMatrix[i, j];
            //if (!uncovered_zero_found) <-- not to be checked, since the function has already returned if this was the case

            // Step4 (from Riesen2009)
            // Add e_min to all elements that are in both the covered rows and columns
            for (int i = 0; i < state.N; i++)
                for (int j = 0; j < state.N; j++)
                    if (state.CoveredRows[i] && state.CoveredColumns[j])
                        state.CostMatrix[i, j] += e_min;
                    else if (!state.CoveredRows[i] && !state.CoveredColumns[j])
                        state.CostMatrix[i, j] -= e_min;
            return Step2(state);
        }

        private static State Step3(State state)
        {
            var S = new List<(int, int, Marks)>();
            var Z0 = state.Marked.Last();
            S.Add((Z0.Item1, Z0.Item2, Z0.Item3));
            while (state.Marked.Any(t => t.Item2 == Z0.Item2 && t.Item3 == Marks.Star))
            {
                var Z1 = state.Marked.Single(t => t.Item2 == Z0.Item2 && t.Item3 == Marks.Star);
                S.Add((Z1.Item1, Z1.Item2, Z1.Item3));
                var prime_zero = state.Marked.Single(t => t.Item1 == Z1.Item1 && t.Item3 == Marks.Prime);
                Z0 = prime_zero;
                S.Add((Z0.Item1, Z0.Item2, Z0.Item3));
            }
            foreach (var t in S)//.Where(s => s.Item3 == Marks.Star))
                state.Marked.RemoveAll(m => m.Item1 == t.Item1 && m.Item2 == t.Item2 && m.Item3 == t.Item3);
            foreach (var t in S.Where(s => s.Item3 == Marks.Prime))
                state.Marked.Add(new Tuple<int, int, Marks>(t.Item1, t.Item2, Marks.Star));
            state.Marked.RemoveAll(m => m.Item3 == Marks.Prime);
            state.ClearCovers();
            return Step1(state);
        }
    }
}
