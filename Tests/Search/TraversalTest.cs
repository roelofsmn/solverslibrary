using NSubstitute;
using SolversLibrary.Search;
using SolversLibrary.Search.Traversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Search
{
    public class TraversalTest
    {
        [Fact]
        public void BreadthFirstGraphTraversal()
        {
            // Assign
            var explorer = Substitute.For<IExploreFunction<string, string>>();
            explorer.Branch("a").Returns(new string[] { "b", "c" });
            explorer.Branch("b").Returns(new string[] { "d", "c" });
            explorer.Branch("c").Returns(new string[] { "e" });
            explorer.Branch("d").Returns(new string[] { });
            explorer.Branch("e").Returns(new string[] { });

            var strategy = new BreadthFirstTraversalStrategy<string>();

            // Act
            var traverser = new GraphTraversal<string>(
                explorer,
                strategy);
            var results = traverser.Traverse("a").ToArray();

            // Assert
            Assert.Equal(new string[] { "a", "b", "c", "d", "e" }, results);
        }

        [Fact]
        public void DepthFirstGraphTraversal()
        {
            // Assign
            var explorer = Substitute.For<IExploreFunction<string, string>>();
            explorer.Branch("a").Returns(new string[] { "b", "c" });
            explorer.Branch("b").Returns(new string[] { "d", "c" });
            explorer.Branch("c").Returns(new string[] { "e" });
            explorer.Branch("d").Returns(new string[] { });
            explorer.Branch("e").Returns(new string[] { });

            var strategy = new DepthFirstTraversalStrategy<string>();

            // Act
            var traverser = new GraphTraversal<string>(
                explorer,
                strategy);
            var results = traverser.Traverse("a").ToArray();

            // Assert
            Assert.Equal(new string[] { "a", "c", "e", "b", "d" }, results);
        }

        [Fact]
        public void BreadthFirstTreeTraversal()
        {
            // Assign
            var explorer = Substitute.For<IExploreFunction<string, string>>();
            explorer.Branch("a").Returns(new string[] { "b", "c" });
            explorer.Branch("b").Returns(new string[] { "d", "e" });
            explorer.Branch("c").Returns(new string[] { });
            explorer.Branch("d").Returns(new string[] { });
            explorer.Branch("e").Returns(new string[] { });

            var strategy = new BreadthFirstTraversalStrategy<string>();

            // Act
            var traverser = new TreeTraversal<string>(
                explorer,
                strategy);
            var results = traverser.Traverse("a").ToArray();

            // Assert
            Assert.Equal(new string[] { "a", "b", "c", "d", "e" }, results);
        }

        [Fact]
        public void DepthFirstTreeTraversal()
        {
            // Assign
            var explorer = Substitute.For<IExploreFunction<string, string>>();
            explorer.Branch("a").Returns(new string[] { "b", "c" });
            explorer.Branch("b").Returns(new string[] { "d", "e" });
            explorer.Branch("c").Returns(new string[] { });
            explorer.Branch("d").Returns(new string[] { });
            explorer.Branch("e").Returns(new string[] { });

            var strategy = new DepthFirstTraversalStrategy<string>();

            // Act
            var traverser = new TreeTraversal<string>(
                explorer,
                strategy);
            var results = traverser.Traverse("a").ToArray();

            // Assert
            Assert.Equal(new string[] { "a", "c", "b", "e", "d" }, results);
        }

        [Fact]
        public void PriorityTraversal_AddCandidateState()
        {
            var costState = Substitute.For<ICost>();
            costState.Cost.Returns(5);
            var strategy = new PriorityTraversalStrategy<ICost>();

            // Act
            strategy.AddCandidateState(costState);

            // Assert
            Assert.True(strategy.ContainsCandidates());
            Assert.True(strategy.Contains(costState));
        }
        [Fact]
        public void PriorityTraversal_NextState()
        {
            var costState = Substitute.For<ICost>();
            costState.Cost.Returns(5);
            var strategy = new PriorityTraversalStrategy<ICost>();

            // Act
            strategy.AddCandidateState(costState);
            var next = strategy.NextState();

            // Assert
            Assert.Same(costState, next);
            Assert.False(strategy.ContainsCandidates());
            Assert.False(strategy.Contains(costState));
        }
        [Fact]
        public void PriorityTraversal_ReplaceCandidateState()
        {
            var costState = Substitute.For<ICost>();
            costState.Cost.Returns(5);
            var replacement = Substitute.For<ICost>();
            replacement.Cost.Returns(2);
            var strategy = new PriorityTraversalStrategy<ICost>();

            // Act
            strategy.AddCandidateState(costState);
            strategy.ReplaceCandidateState(costState, replacement);

            // Assert
            Assert.True(strategy.ContainsCandidates());
            Assert.False(strategy.Contains(costState));
            Assert.True(strategy.Contains(replacement));
        }
        [Fact]
        public void PriorityTraversal_ReplaceCandidateState_NextState()
        {
            var costState = Substitute.For<ICost>();
            costState.Cost.Returns(5);
            var replacement = Substitute.For<ICost>();
            replacement.Cost.Returns(2);
            var strategy = new PriorityTraversalStrategy<ICost>();

            // Act
            strategy.AddCandidateState(costState);
            strategy.ReplaceCandidateState(costState, replacement);
            var next = strategy.NextState();

            // Assert
            Assert.False(strategy.ContainsCandidates());
            Assert.Same(replacement, next);
        }
    }
}
