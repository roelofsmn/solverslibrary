using NSubstitute;
using SolversLibrary.Search.Algorithms;
using SolversLibrary.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Search
{
    public class AstarSearchTest
    {
        [Fact]
        public void AstarSearch()
        {
            // See section 3.4, page 85 of Artificial Intelligence: A Modern Approach, 3rd edition, S.J. Russell and P. Norvig
            // Assign
            var problem = Substitute.For<IHeuristicSearchProblem<string>>();
            problem.IsTerminal("sibiu").Returns(false);
            problem.IsTerminal("rimnicu").Returns(false);
            problem.IsTerminal("pitesti").Returns(false);
            problem.IsTerminal("fagaras").Returns(false);
            problem.IsTerminal("bucharest").Returns(true);

            var action1 = Substitute.For<ICostSearchAction<string>>();
            action1.Apply("sibiu").Returns("fagaras");
            action1.Cost("sibiu").Returns(99);

            var action2 = Substitute.For<ICostSearchAction<string>>();
            action2.Apply("sibiu").Returns("rimnicu");
            action2.Cost("sibiu").Returns(80);

            var action3 = Substitute.For<ICostSearchAction<string>>();
            action3.Apply("fagaras").Returns("bucharest");
            action3.Cost("fagaras").Returns(211);

            var action4 = Substitute.For<ICostSearchAction<string>>();
            action4.Apply("rimnicu").Returns("pitesti");
            action4.Cost("rimnicu").Returns(97);

            var action5 = Substitute.For<ICostSearchAction<string>>();
            action5.Apply("pitesti").Returns("bucharest");
            action5.Cost("pitesti").Returns(101);

            problem.Branch("sibiu").Returns(new ICostSearchAction<string>[] { action1, action2 });
            problem.Branch("rimnicu").Returns(new ICostSearchAction<string>[] { action4 });
            problem.Branch("pitesti").Returns(new ICostSearchAction<string>[] { action5 });
            problem.Branch("fagaras").Returns(new ICostSearchAction<string>[] { action3 });

            problem.Heuristic("sibiu").Returns(253);
            problem.Heuristic("rimnicu").Returns(193);
            problem.Heuristic("fagaras").Returns(176);
            problem.Heuristic("pitesti").Returns(100);
            problem.Heuristic("bucharest").Returns(0);

            var initialState = "sibiu";

            // Act
            var bfs = new AstarSearch<string>();
            var result = bfs.Search(problem, initialState);

            // Assert
            Assert.Equal("sibiu", result.InitialState);
            Assert.Equal(new ICostSearchAction<string>[] { action2, action4, action5 }, result.ActionPath);
            Assert.Equal(278, result.Cost);
            Assert.Equal("bucharest", result.TerminalState);

        }
    }
}
