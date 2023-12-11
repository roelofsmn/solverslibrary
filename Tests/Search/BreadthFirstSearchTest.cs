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
    public class BreadthFirstSearchTest
    {
        [Fact]
        public void BreadthFirstSearch()
        {
            // See section 3.4, page 85 of Artificial Intelligence: A Modern Approach, 3rd edition, S.J. Russell and P. Norvig
            // Assign
            var problem = Substitute.For<IGoalDefinition<string>>();
            problem.IsTerminal("sibiu").Returns(false);
            problem.IsTerminal("rimnicu").Returns(false);
            problem.IsTerminal("pitesti").Returns(false);
            problem.IsTerminal("fagaras").Returns(false);
            problem.IsTerminal("bucharest").Returns(true);

            var action1 = Substitute.For<ITraversal<string>>();
            action1.Traverse("sibiu").Returns("fagaras");

            var action2 = Substitute.For<ITraversal<string>>();
            action2.Traverse("sibiu").Returns("rimnicu");

            var action3 = Substitute.For<ITraversal<string>>();
            action3.Traverse("fagaras").Returns("bucharest");

            var action4 = Substitute.For<ITraversal<string>>();
            action4.Traverse("rimnicu").Returns("pitesti");

            var action5 = Substitute.For<ITraversal<string>>();
            action5.Traverse("pitesti").Returns("bucharest");

            var branching = Substitute.For<IBranchingFunction<string>>();
            branching.Branch("sibiu").Returns(new ITraversal<string>[] { action1, action2 });
            branching.Branch("rimnicu").Returns(new ITraversal<string>[] { action4 });
            branching.Branch("pitesti").Returns(new ITraversal<string>[] { action5 });
            branching.Branch("fagaras").Returns(new ITraversal<string>[] { action3 });

            var initialState = "sibiu";

            // Act
            var bfs = new UninformedPathSearch<string>(
                TraversalStrategies.BreadthFirst,
                Traversers.Graph,
                branching);
            var result = bfs.Search(problem, initialState);

            // Assert
            Assert.Equal("sibiu", result.InitialState);
            Assert.Equal(new ITraversal<string>[] { action1, action3 }, result.ActionPath);
            Assert.Equal("bucharest", result.TerminalState);
        }
    }
}
