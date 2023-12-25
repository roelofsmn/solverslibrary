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
            var explorer = Substitute.For<IBranchingFunction<string>>();

            var ab = Substitute.For<ITraversal<string>>();
            ab.Traverse("a").Returns("b");

            var ac = Substitute.For<ITraversal<string>>();
            ac.Traverse("a").Returns("c");

            var bd = Substitute.For<ITraversal<string>>();
            bd.Traverse("b").Returns("d");

            var bc = Substitute.For<ITraversal<string>>();
            bc.Traverse("b").Returns("c");

            var ce = Substitute.For<ITraversal<string>>();
            ce.Traverse("c").Returns("e");

            explorer.Branch("a").Returns(new ITraversal<string>[] { ab, ac });
            explorer.Branch("b").Returns(new ITraversal<string>[] { bd, bc });
            explorer.Branch("c").Returns(new ITraversal<string>[] { ce });
            explorer.Branch("d").Returns(new ITraversal<string>[] { });
            explorer.Branch("e").Returns(new ITraversal<string>[] { });

            var strategy = new BreadthFirstTraversalStrategy<string>();

            // Act
            var traverser = new GraphTraverser<string>(
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
            var explorer = Substitute.For<IBranchingFunction<string>>();

            var ab = Substitute.For<ITraversal<string>>();
            ab.Traverse("a").Returns("b");

            var ac = Substitute.For<ITraversal<string>>();
            ac.Traverse("a").Returns("c");

            var bd = Substitute.For<ITraversal<string>>();
            bd.Traverse("b").Returns("d");

            var bc = Substitute.For<ITraversal<string>>();
            bc.Traverse("b").Returns("c");

            var ce = Substitute.For<ITraversal<string>>();
            ce.Traverse("c").Returns("e");

            explorer.Branch("a").Returns(new ITraversal<string>[] { ab, ac });
            explorer.Branch("b").Returns(new ITraversal<string>[] { bd, bc });
            explorer.Branch("c").Returns(new ITraversal<string>[] { ce });
            explorer.Branch("d").Returns(new ITraversal<string>[] { });
            explorer.Branch("e").Returns(new ITraversal<string>[] { });

            var strategy = new DepthFirstTraversalStrategy<string>();

            // Act
            var traverser = new GraphTraverser<string>(
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
            var explorer = Substitute.For<IBranchingFunction<string>>();

            var ab = Substitute.For<ITraversal<string>>();
            ab.Traverse("a").Returns("b");

            var ac = Substitute.For<ITraversal<string>>();
            ac.Traverse("a").Returns("c");

            var bd = Substitute.For<ITraversal<string>>();
            bd.Traverse("b").Returns("d");

            var be = Substitute.For<ITraversal<string>>();
            be.Traverse("b").Returns("e");

            explorer.Branch("a").Returns(new ITraversal<string>[] { ab, ac });
            explorer.Branch("b").Returns(new ITraversal<string>[] { bd, be });
            explorer.Branch("c").Returns(new ITraversal<string>[] { });
            explorer.Branch("d").Returns(new ITraversal<string>[] { });
            explorer.Branch("e").Returns(new ITraversal<string>[] { });

            var strategy = new BreadthFirstTraversalStrategy<string>();

            // Act
            var traverser = new TreeTraverser<string>(
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
            var explorer = Substitute.For<IBranchingFunction<string>>();

            var ab = Substitute.For<ITraversal<string>>();
            ab.Traverse("a").Returns("b");

            var ac = Substitute.For<ITraversal<string>>();
            ac.Traverse("a").Returns("c");

            var bd = Substitute.For<ITraversal<string>>();
            bd.Traverse("b").Returns("d");

            var be = Substitute.For<ITraversal<string>>();
            be.Traverse("b").Returns("e");

            explorer.Branch("a").Returns(new ITraversal<string>[] { ab, ac });
            explorer.Branch("b").Returns(new ITraversal<string>[] { bd, be });
            explorer.Branch("c").Returns(new ITraversal<string>[] { });
            explorer.Branch("d").Returns(new ITraversal<string>[] { });
            explorer.Branch("e").Returns(new ITraversal<string>[] { });

            var strategy = new DepthFirstTraversalStrategy<string>();

            // Act
            var traverser = new TreeTraverser<string>(
                explorer,
                strategy);
            var results = traverser.Traverse("a").ToArray();

            // Assert
            Assert.Equal(new string[] { "a", "c", "b", "e", "d" }, results);
        }

        [Fact]
        public void PriorityTraversal_AddCandidateState()
        {
            var costState = "a";
            var strategy = new PriorityTraversalStrategy<string>(x => 5);

            // Act
            strategy.AddCandidateState(costState);

            // Assert
            Assert.True(strategy.ContainsCandidates());
            Assert.True(strategy.Contains(costState));
        }
        [Fact]
        public void PriorityTraversal_NextState()
        {
            var costState = "a";
            var strategy = new PriorityTraversalStrategy<string>(x => 5);

            // Act
            strategy.AddCandidateState(costState);
            var next = strategy.NextState();

            // Assert
            Assert.Same(costState, next);
            Assert.False(strategy.ContainsCandidates());
            Assert.False(strategy.Contains(costState));
        }
        //[Fact]
        //public void PriorityTraversal_ReplaceCandidateState()
        //{
        //    var costState = "a";
        //    var replacement = "b";
        //    var strategy = new PriorityTraversalStrategy<string>(x => x == "a" ? 5 : 2);

        //    // Act
        //    strategy.AddCandidateState(costState);
        //    strategy.ReplaceCandidateState(costState, replacement);

        //    // Assert
        //    Assert.True(strategy.ContainsCandidates());
        //    Assert.False(strategy.Contains(costState));
        //    Assert.True(strategy.Contains(replacement));
        //}
        //[Fact]
        //public void PriorityTraversal_ReplaceCandidateState_NextState()
        //{
        //    var costState = "a";
        //    var replacement = "b";
        //    var strategy = new PriorityTraversalStrategy<string>(x => x == "a" ? 5 : 2);

        //    // Act
        //    strategy.AddCandidateState(costState);
        //    strategy.ReplaceCandidateState(costState, replacement);
        //    var next = strategy.NextState();

        //    // Assert
        //    Assert.False(strategy.ContainsCandidates());
        //    Assert.Same(replacement, next);
        //}
    }
}
