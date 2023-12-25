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
    public class AstarSearchTest
    {
        private readonly IGoalDefinition<string> problem;
        private readonly ITraversal<string> action1;
        private readonly ITraversal<string> action2;
        private readonly ITraversal<string> action3;
        private readonly ITraversal<string> action4;
        private readonly ITraversal<string> action5;
        private readonly IBranchingFunction<string> branching;
        private readonly IHeuristic<string> heuristic;
        private readonly string initialState;
        private List<PathSearchState<string>> searchStates;

        public AstarSearchTest()
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

            heuristic = Substitute.For<IHeuristic<string>>();
            heuristic.Heuristic("sibiu").Returns(253);
            heuristic.Heuristic("rimnicu").Returns(193);
            heuristic.Heuristic("fagaras").Returns(176);
            heuristic.Heuristic("pitesti").Returns(100);
            heuristic.Heuristic("bucharest").Returns(0);

            initialState = "sibiu";

            searchStates = new List<PathSearchState<string>>();
        }

        [Fact]
        public void AstarSearch()
        {
            // See section 3.4, page 85 of Artificial Intelligence: A Modern Approach, 3rd edition, S.J. Russell and P. Norvig
            // Assign

            var pathBranch = new PathSearchBranchingFunction<string>(branching, (state, cost, traversal) => cost + traversal.Cost(state) ?? 0);
            var astar = new AstarSearch<PathSearchState<string>>(
                SolversLibrary.Search.Factories.Traversers.Graph,
                pathBranch,
                (pathNode) => pathNode.Cost,
                (pathNode) => heuristic.Heuristic(pathNode.State)
                );
            astar.Explored += Astar_ProgressUpdated;

            // Act
            var pathGoal = new PathSearchGoal<string>(problem);
            var pathInitialState = new PathSearchState<string>(initialState, null, null, 0.0);
            var result = astar.Search(pathGoal, pathInitialState);

            // Assert
            // Check final result
            Assert.Equal(new ITraversal<string>[] { action2, action4, action5 }, result.GetActionsToNode().ToArray());
            Assert.Equal(278, result.Cost);
            Assert.Equal("bucharest", result.State);

            // Check search path
            Assert.Equal(5, searchStates.Count);
            Assert.Equal(new ITraversal<string>[] { }, searchStates[0].GetActionsToNode().ToArray());
            Assert.Equal(0, searchStates[0].Cost); // heuristic cost

            // at this point, rimnicu has 193 + 80 = 273, and fagaras has 176 + 99 = 275
            Assert.Equal(new ITraversal<string>[] { action2 }, searchStates[1].GetActionsToNode().ToArray()); // because it has lowest total estimated cost
            Assert.Equal(80, searchStates[1].Cost);
            // adds pitesti with total cost 177 + 100 = 277

            Assert.Equal(new ITraversal<string>[] { action1 }, searchStates[2].GetActionsToNode().ToArray()); // because it has lowest cost
            Assert.Equal(99, searchStates[2].Cost);
            // adds bucharest with total cost 310

            // we first explore the lower cost path (action2)
            Assert.Equal(new ITraversal<string>[] { action2, action4 }, searchStates[3].GetActionsToNode().ToArray());
            Assert.Equal(177, searchStates[3].Cost);
            // now, the path (action2, action4, action5) is added to the frontier, with a cost of 278, and is expanded next.

            Assert.Equal(new ITraversal<string>[] { action2, action4, action5 }, searchStates[4].GetActionsToNode().ToArray()); // arrives at bucharest
            Assert.Equal(278, searchStates[4].Cost);
        }

        private void Astar_ProgressUpdated(PathSearchState<string> obj)
        {
            searchStates.Add(obj);
        }
    }
}
