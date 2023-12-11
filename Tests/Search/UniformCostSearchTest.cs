using NSubstitute;
using SolversLibrary.Search.Algorithms;
using SolversLibrary.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolversLibrary.Search.Traversal;

namespace Tests.Search
{
    public class UniformCostSearchTest
    {
        [Fact]
        public void UniformCostSearch()
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
            action1.Cost("sibiu").Returns(99);

            var action2 = Substitute.For<ITraversal<string>>();
            action2.Traverse("sibiu").Returns("rimnicu");
            action2.Cost("sibiu").Returns(80);

            var action3 = Substitute.For<ITraversal<string>>();
            action3.Traverse("fagaras").Returns("bucharest");
            action3.Cost("fagaras").Returns(211);

            var action4 = Substitute.For<ITraversal<string>>();
            action4.Traverse("rimnicu").Returns("pitesti");
            action4.Cost("rimnicu").Returns(97);

            var action5 = Substitute.For<ITraversal<string>>();
            action5.Traverse("pitesti").Returns("bucharest");
            action5.Cost("pitesti").Returns(101);

            var branching = Substitute.For<IBranchingFunction<string>>();
            branching.Branch("sibiu").Returns(new ITraversal<string>[] { action1, action2 });
            branching.Branch("rimnicu").Returns(new ITraversal<string>[] { action4 });
            branching.Branch("pitesti").Returns(new ITraversal<string>[] { action5 });
            branching.Branch("fagaras").Returns(new ITraversal<string>[] { action3 });

            var initialState = "sibiu";

            // Act
            var bfs = new UniformCostSearch<string>(
                SolversLibrary.Search.Factories.Traversers.Graph,
                branching);
            var result = bfs.Search(problem, initialState);

            // Assert
            Assert.Equal("sibiu", result.InitialState);
            Assert.Equal(new ITraversal<string>[] { action2, action4, action5 }, result.ActionPath);
            Assert.Equal(278, result.Cost);
            Assert.Equal("bucharest", result.TerminalState);

        }
    }
}
