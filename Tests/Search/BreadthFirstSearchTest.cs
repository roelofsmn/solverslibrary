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
        public void BreadthFirstSearch()
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

            var action2 = Substitute.For<ISearchAction<string>>();
            action2.Apply("sibiu").Returns("rimnicu");

            var action3 = Substitute.For<ISearchAction<string>>();
            action3.Apply("fagaras").Returns("bucharest");

            var action4 = Substitute.For<ISearchAction<string>>();
            action4.Apply("rimnicu").Returns("pitesti");

            var action5 = Substitute.For<ISearchAction<string>>();
            action5.Apply("pitesti").Returns("bucharest");

            problem.Branch("sibiu").Returns(new ISearchAction<string>[] { action1, action2 });
            problem.Branch("rimnicu").Returns(new ISearchAction<string>[] { action4 });
            problem.Branch("pitesti").Returns(new ISearchAction<string>[] { action5 });
            problem.Branch("fagaras").Returns(new ISearchAction<string>[] { action3 });

            var initialState = "sibiu";

            // Act
            var bfs = new BreadthFirstSearch<string>();
            var result = bfs.Search(problem, initialState);

            // Assert
            Assert.Equal("sibiu", result.InitialState);
            Assert.Equal(new ISearchAction<string>[] { action1, action3 }, result.ActionPath);
            Assert.Equal("bucharest", result.TerminalState);
        }

        [Fact]
        public void BreadthFirstSearch_CommonNodeOnPath()
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

            var action2 = Substitute.For<ISearchAction<string>>();
            action2.Apply("sibiu").Returns("rimnicu");

            var action3 = Substitute.For<ISearchAction<string>>();
            action3.Apply("fagaras").Returns("pitesti");

            var action4 = Substitute.For<ISearchAction<string>>();
            action4.Apply("rimnicu").Returns("pitesti");

            var action5 = Substitute.For<ISearchAction<string>>();
            action5.Apply("pitesti").Returns("bucharest");

            problem.Branch("sibiu").Returns(new ISearchAction<string>[] { action1, action2 });
            problem.Branch("rimnicu").Returns(new ISearchAction<string>[] { action4 });
            problem.Branch("pitesti").Returns(new ISearchAction<string>[] { action5 });
            problem.Branch("fagaras").Returns(new ISearchAction<string>[] { action3 });

            var initialState = "sibiu";

            // Act
            var bfs = new BreadthFirstSearch<string>();
            var results = bfs.Search(problem, initialState);

            // Assert
            Assert.Equal("sibiu", results.InitialState);
            Assert.Equal(new ISearchAction<string>[] { action1, action3, action5 }, results.ActionPath);
            Assert.Equal("bucharest", results.TerminalState);

            //Assert.Equal("sibiu", results.InitialState);
            //Assert.Equal(new ISearchAction<string>[] { action2, action4, action5 }, results.ActionPath);
            //Assert.Equal("bucharest", results.TerminalState);
        }
    }
}
