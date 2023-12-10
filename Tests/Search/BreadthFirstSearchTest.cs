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
            var root = Substitute.For<SearchNode<string>>("root", null, null);
            var action1 = Substitute.For<ISearchAction<string>>();
            var first = Substitute.For<SearchNode<string>>("first", root, action1);
            var action2 = Substitute.For<ISearchAction<string>>();
            var second = Substitute.For<SearchNode<string>>("second", first, action2);

            // Act
            var results = BreadthFirstSearch<string>.GetActionsToNode(second);

            // Assert
            Assert.Equal(2, results.Length);
            Assert.Same(action1, results[0]);
            Assert.Same(action2, results[1]);
        }

        [Fact]
        public void Search_Traversal()
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
        public void Search_Traversal_CommonNodeOnPath()
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

        [Fact]
        public void UniformCostSearch()
        {
            // See section 3.4, page 85 of Artificial Intelligence: A Modern Approach, 3rd edition, S.J. Russell and P. Norvig
            // Assign
            var problem = Substitute.For<ICostSearchProblem<string>>();
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

            var initialState = "sibiu";

            // Act
            var bfs = new UniformCostSearch<string>();
            var result = bfs.Search(problem, initialState);

            // Assert
            Assert.Equal("sibiu", result.InitialState);
            Assert.Equal(new ISearchAction<string>[] { action2, action4, action5 }, result.ActionPath);
            Assert.Equal(278, result.Cost);
            Assert.Equal("bucharest", result.TerminalState);

        }
    }
}
