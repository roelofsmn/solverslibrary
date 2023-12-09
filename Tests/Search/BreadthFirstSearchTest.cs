using NSubstitute;
using NSubstitute.Routing.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolversLibrary.Search;
using SolversLibrary.Search.Algorithms;

namespace Tests.Search
{
    public class BreadthFirstSearchTest
    {
        [Fact]
        public void GetActionsToNode()
        {
            // Assign
            var root = Substitute.For<SearchNode<string>>("root", null, null, 0.0);
            var action1 = Substitute.For<ISearchAction<string>>();
            var first = Substitute.For<SearchNode<string>>("first", root, action1, 1.0);
            var action2 = Substitute.For<ISearchAction<string>>();
            var second = Substitute.For<SearchNode<string>>("second", first, action2, 2.0);

            // Act
            var results = BreadthFirstSearch<string>.GetActionsToNode(second);

            // Assert
            Assert.Equal(2, results.Length);
            Assert.Same(action1, results[0]);
            Assert.Same(action2, results[1]);
        }

        [Fact]
        public void Search()
        {
            // See section 3.4, page 85 of Artificial Intelligence: A Modern Approach, 3rd edition, S.J. Russell and P. Norvig
            // Assign
            var problem = Substitute.For<ISearchProblem<string>>();
            problem.IsTerminal("sibiu").Returns(false);
            problem.IsTerminal("rimnicu").Returns(false);
            problem.IsTerminal("pitesti").Returns(false);
            problem.IsTerminal("fagaras").Returns(false);
            problem.IsTerminal("bucharest").Returns(true);

            var action1 = Substitute.For<ISearchAction<string>>();
            action1.Apply("sibiu").Returns("fagaras");
            action1.Cost("sibiu").Returns(99);

            var action2 = Substitute.For<ISearchAction<string>>();
            action2.Apply("sibiu").Returns("rimnicu");
            action2.Cost("sibiu").Returns(80);

            var action3 = Substitute.For<ISearchAction<string>>();
            action3.Apply("fagaras").Returns("bucharest");
            action3.Cost("fagaras").Returns(211);

            var action4 = Substitute.For<ISearchAction<string>>();
            action4.Apply("rimnicu").Returns("pitesti");
            action4.Cost("rimnicu").Returns(97);

            var action5 = Substitute.For<ISearchAction<string>>();
            action5.Apply("pitesti").Returns("bucharest");
            action5.Cost("pitesti").Returns(101);

            problem.Branch("sibiu").Returns(new ISearchAction<string>[] { action1, action2 });
            problem.Branch("rimnicu").Returns(new ISearchAction<string>[] { action4 });
            problem.Branch("pitesti").Returns(new ISearchAction<string>[] { action5 });
            problem.Branch("fagaras").Returns(new ISearchAction<string>[] { action3 });

            var initialState = "sibiu";

            // Act
            var bfs = new BreadthFirstSearch<string>();
            var results = bfs.Search(problem, initialState).ToArray();

            // Assert
            // Since BFS essentially only finds the shortest path, we don't find the optimal solution.
            // We do, however, find the target state in the least amount of steps.
            Assert.Single(results);
            var solution = results.Single();
            Assert.Equal("sibiu", solution.InitialState);
            Assert.Equal(new ISearchAction<string>[] { action1, action3 }, solution.ActionPath);
            Assert.Equal(310, solution.TotalCost);
            Assert.Equal("bucharest", solution.TerminalState);
        }
    }
}
