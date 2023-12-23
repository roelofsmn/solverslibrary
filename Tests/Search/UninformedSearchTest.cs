using NSubstitute;
using NSubstitute.Routing.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolversLibrary.Search;
using SolversLibrary.Search.Algorithms;
using SolversLibrary.Search.Factories;
using SolversLibrary.Search.Traversal;

namespace Tests.Search
{
    public class UninformedSearchTest
    {
        private readonly IGoalDefinition<string> problem;
        private readonly ITraversal<string> action1;
        private readonly ITraversal<string> action2;
        private readonly ITraversal<string> action3;
        private readonly ITraversal<string> action4;
        private readonly ITraversal<string> action5;
        private readonly IBranchingFunction<string> branching;
        private readonly string initialState;
        private List<SearchSolution<string>> searchStates;

        public UninformedSearchTest()
        {
            // Assign
            problem = Substitute.For<IGoalDefinition<string>>();
            problem.IsTerminal("sibiu").Returns(false);
            problem.IsTerminal("rimnicu").Returns(false);
            problem.IsTerminal("pitesti").Returns(false);
            problem.IsTerminal("fagaras").Returns(false);
            problem.IsTerminal("bucharest").Returns(true);

            action1 = Substitute.For<ITraversal<string>>();
            action1.Traverse("sibiu").Returns("fagaras");

            action2 = Substitute.For<ITraversal<string>>();
            action2.Traverse("sibiu").Returns("rimnicu");

            action3 = Substitute.For<ITraversal<string>>();
            action3.Traverse("fagaras").Returns("bucharest");

            action4 = Substitute.For<ITraversal<string>>();
            action4.Traverse("rimnicu").Returns("pitesti");

            action5 = Substitute.For<ITraversal<string>>();
            action5.Traverse("pitesti").Returns("bucharest");

            branching = Substitute.For<IBranchingFunction<string>>();
            branching.Branch("sibiu").Returns(new ITraversal<string>[] { action1, action2 });
            branching.Branch("rimnicu").Returns(new ITraversal<string>[] { action4 });
            branching.Branch("pitesti").Returns(new ITraversal<string>[] { action5 });
            branching.Branch("fagaras").Returns(new ITraversal<string>[] { action3 });

            initialState = "sibiu";

            searchStates = new List<SearchSolution<string>>();
        }

        private void Bfs_ProgressUpdated(SearchSolution<string> obj)
        {
            searchStates.Add(obj);
        }

        [Fact]
        public void BreadthFirstGraphSearch()
        {
            // See section 3.4, page 85 of Artificial Intelligence: A Modern Approach, 3rd edition, S.J. Russell and P. Norvig
            // Assign
            var bfs = new UninformedPathSearch<string>(
                TraversalStrategies.BreadthFirst,
                Traversers.Graph,
                branching);
            bfs.ProgressUpdated += Bfs_ProgressUpdated;

            // Act
            var result = bfs.Search(problem, initialState);

            // Assert
            // Check final result
            Assert.Equal("sibiu", result.InitialState);
            Assert.Equal(new ITraversal<string>[] { action1, action3 }, result.ActionPath);
            Assert.Equal("bucharest", result.TerminalState);

            // Check search path
            Assert.Equal(4, searchStates.Count);
            Assert.Equal(new ITraversal<string>[] { }, searchStates[0].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action1 }, searchStates[1].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action2 }, searchStates[2].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action1, action3 }, searchStates[3].ActionPath);
        }

        [Fact]
        public void BreadthFirstTreeSearch()
        {
            // See section 3.4, page 85 of Artificial Intelligence: A Modern Approach, 3rd edition, S.J. Russell and P. Norvig
            // Assign
            var bfs = new UninformedPathSearch<string>(
                TraversalStrategies.BreadthFirst,
                Traversers.Tree,
                branching);
            bfs.ProgressUpdated += Bfs_ProgressUpdated;

            // Act
            var result = bfs.Search(problem, initialState);

            // Assert
            // Check final result
            Assert.Equal("sibiu", result.InitialState);
            Assert.Equal(new ITraversal<string>[] { action1, action3 }, result.ActionPath);
            Assert.Equal("bucharest", result.TerminalState);

            // Check search path
            Assert.Equal(4, searchStates.Count);
            Assert.Equal(new ITraversal<string>[] { }, searchStates[0].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action1 }, searchStates[1].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action2 }, searchStates[2].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action1, action3 }, searchStates[3].ActionPath);
        }

        [Fact]
        public void DepthFirstGraphSearchTest()
        {
            // See section 3.4, page 85 of Artificial Intelligence: A Modern Approach, 3rd edition, S.J. Russell and P. Norvig
            // Assign
            var dfs = new UninformedPathSearch<string>(
                TraversalStrategies.DepthFirst,
                Traversers.Graph,
                branching);
            dfs.ProgressUpdated += Bfs_ProgressUpdated;

            // Act
            var result = dfs.Search(problem, initialState);

            // Assert
            // Check final result
            Assert.Equal("sibiu", result.InitialState);
            Assert.Equal(new ITraversal<string>[] { action2, action4, action5 }, result.ActionPath);
            Assert.Equal("bucharest", result.TerminalState);

            // Check search path
            Assert.Equal(4, searchStates.Count);
            Assert.Equal(new ITraversal<string>[] { }, searchStates[0].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action2 }, searchStates[1].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action2, action4 }, searchStates[2].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action2, action4, action5 }, searchStates[3].ActionPath);
        }
        [Fact]
        public void DepthFirstTreeSearchTest()
        {
            // See section 3.4, page 85 of Artificial Intelligence: A Modern Approach, 3rd edition, S.J. Russell and P. Norvig
            // Assign
            var dfs = new UninformedPathSearch<string>(
                TraversalStrategies.DepthFirst,
                Traversers.Tree,
                branching);
            dfs.ProgressUpdated += Bfs_ProgressUpdated;

            // Act
            var result = dfs.Search(problem, initialState);

            // Assert
            // Check final result
            Assert.Equal("sibiu", result.InitialState);
            Assert.Equal(new ITraversal<string>[] { action2, action4, action5 }, result.ActionPath);
            Assert.Equal("bucharest", result.TerminalState);

            // Check search path
            Assert.Equal(4, searchStates.Count);
            Assert.Equal(new ITraversal<string>[] { }, searchStates[0].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action2 }, searchStates[1].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action2, action4 }, searchStates[2].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action2, action4, action5 }, searchStates[3].ActionPath);
        }
    }
}
