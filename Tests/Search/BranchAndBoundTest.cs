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
    public class BranchAndBoundTest
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

        public BranchAndBoundTest()
        {
            problem = Substitute.For<IGoalDefinition<string>>();
            problem.IsTerminal("sibiu").Returns(false);
            problem.IsTerminal("rimnicu").Returns(false);
            problem.IsTerminal("pitesti").Returns(false);
            problem.IsTerminal("fagaras").Returns(false);
            problem.IsTerminal("bucharest").Returns(true);

            action1 = Substitute.For<ITraversal<string>>();
            action1.Traverse("sibiu").Returns("fagaras");
            action1.Cost("sibiu").Returns(99);

            action2 = Substitute.For<ITraversal<string>>();
            action2.Traverse("sibiu").Returns("rimnicu");
            action2.Cost("sibiu").Returns(80);

            action3 = Substitute.For<ITraversal<string>>();
            action3.Traverse("fagaras").Returns("bucharest");
            action3.Cost("fagaras").Returns(211);

            action4 = Substitute.For<ITraversal<string>>();
            action4.Traverse("rimnicu").Returns("pitesti");
            action4.Cost("rimnicu").Returns(97);

            action5 = Substitute.For<ITraversal<string>>();
            action5.Traverse("pitesti").Returns("bucharest");
            action5.Cost("pitesti").Returns(101);

            branching = Substitute.For<IBranchingFunction<string>>();
            branching.Branch("sibiu").Returns(new ITraversal<string>[] { action1, action2 });
            branching.Branch("rimnicu").Returns(new ITraversal<string>[] { action4 });
            branching.Branch("pitesti").Returns(new ITraversal<string>[] { action5 });
            branching.Branch("fagaras").Returns(new ITraversal<string>[] { action3 });

            initialState = "sibiu";

            searchStates = new List<SearchSolution<string>>();
        }

        private void BnB_ProgressUpdated(SearchSolution<string> obj)
        {
            searchStates.Add(obj);
        }

        [Fact]
        public void BranchAndBound_DepthFirst_Search()
        {
            // See section 3.4, page 85 of Artificial Intelligence: A Modern Approach, 3rd edition, S.J. Russell and P. Norvig
            // Assign
            var bnb = new BranchAndBound<string>(
                SolversLibrary.Search.Factories.Traversers.Graph,
                SolversLibrary.Search.Factories.TraversalStrategies.DepthFirst,
                branching,
                (state, cost, traversal) => cost + traversal?.Cost(state) ?? 0.0);
            bnb.ProgressUpdated += BnB_ProgressUpdated;

            // Act
            var result = bnb.Search(problem, initialState);

            // Assert
            // Check final result
            Assert.Equal("sibiu", result.InitialState);
            Assert.Equal(new ITraversal<string>[] { action2, action4, action5 }, result.ActionPath);
            Assert.Equal(278, result.Cost);
            Assert.Equal("bucharest", result.TerminalState);

            // Check search path
            Assert.Equal(6, searchStates.Count);

            Assert.Equal(new ITraversal<string>[] { }, searchStates[0].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action2 }, searchStates[1].ActionPath);
            Assert.Equal(new ITraversal<string>[] { action2, action4 }, searchStates[2].ActionPath);
            // Next we expand the full path:
            Assert.Equal(new ITraversal<string>[] { action2, action4, action5 }, searchStates[3].ActionPath);
            // we set best value to 278, but continue the search (we don't know this is the best path already...)

            Assert.Equal(new ITraversal<string>[] { action1 }, searchStates[4].ActionPath);
            // we expand the full path:
            Assert.Equal(new ITraversal<string>[] { action1, action3 }, searchStates[5].ActionPath);
            // It is terminal, but not better
            // Now we're done... as there are no more states to expand
        }

    }
}
