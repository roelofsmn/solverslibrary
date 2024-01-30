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

            var ba = Substitute.For<ITraversal<string>>();
            ba.Traverse("b").Returns("a");

            var bd = Substitute.For<ITraversal<string>>();
            bd.Traverse("b").Returns("d");

            var bc = Substitute.For<ITraversal<string>>();
            bc.Traverse("b").Returns("c");

            var ca = Substitute.For<ITraversal<string>>();
            ca.Traverse("c").Returns("a");

            var cb = Substitute.For<ITraversal<string>>();
            cb.Traverse("c").Returns("b");

            var cd = Substitute.For<ITraversal<string>>();
            cd.Traverse("c").Returns("d");

            var db = Substitute.For<ITraversal<string>>();
            db.Traverse("d").Returns("b");

            var dc = Substitute.For<ITraversal<string>>();
            dc.Traverse("d").Returns("c");

            explorer.Branch("a").Returns(new ITraversal<string>[] { ab, ac });
            explorer.Branch("b").Returns(new ITraversal<string>[] { ba, bd, bc });
            explorer.Branch("c").Returns(new ITraversal<string>[] { ca, cb, cd });
            explorer.Branch("d").Returns(new ITraversal<string>[] { db, dc });

            var strategy = new BreadthFirstTraversalStrategy<string>();

            // Act
            var traverser = new GraphTraverser<string>(
                explorer,
                strategy);
            var results = traverser.Traverse("a").ToArray();

            // Assert
            Assert.Equal(new string[] { "a", "b", "c", "d" }, results);
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

            var ba = Substitute.For<ITraversal<string>>();
            ba.Traverse("b").Returns("a");

            var bd = Substitute.For<ITraversal<string>>();
            bd.Traverse("b").Returns("d");

            var bc = Substitute.For<ITraversal<string>>();
            bc.Traverse("b").Returns("c");

            var ca = Substitute.For<ITraversal<string>>();
            ca.Traverse("c").Returns("a");

            var cb = Substitute.For<ITraversal<string>>();
            cb.Traverse("c").Returns("b");

            var cd = Substitute.For<ITraversal<string>>();
            cd.Traverse("c").Returns("d");

            var db = Substitute.For<ITraversal<string>>();
            db.Traverse("d").Returns("b");

            var dc = Substitute.For<ITraversal<string>>();
            dc.Traverse("d").Returns("c");

            explorer.Branch("a").Returns(new ITraversal<string>[] { ab, ac });
            explorer.Branch("b").Returns(new ITraversal<string>[] { ba, bd, bc });
            explorer.Branch("c").Returns(new ITraversal<string>[] { ca, cb, cd });
            explorer.Branch("d").Returns(new ITraversal<string>[] { db, dc });

            var strategy = new DepthFirstTraversalStrategy<string>();

            // Act
            var traverser = new GraphTraverser<string>(
                explorer,
                strategy);
            var results = traverser.Traverse("a").ToArray();

            // Assert
            Assert.Equal(new string[] { "a", "c", "d", "b" }, results);
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

        [Fact]
        public void PriorityGraphTraversal()
        {
            // Assign
            var explorer = Substitute.For<IBranchingFunction<string>>();

            var ab = Substitute.For<ITraversal<string>>();
            ab.Traverse("a").Returns("b");

            var ac = Substitute.For<ITraversal<string>>();
            ac.Traverse("a").Returns("c");

            var ba = Substitute.For<ITraversal<string>>();
            ba.Traverse("b").Returns("a");

            var bd = Substitute.For<ITraversal<string>>();
            bd.Traverse("b").Returns("d");

            var bc = Substitute.For<ITraversal<string>>();
            bc.Traverse("b").Returns("c");

            var ca = Substitute.For<ITraversal<string>>();
            ca.Traverse("c").Returns("a");

            var cb = Substitute.For<ITraversal<string>>();
            cb.Traverse("c").Returns("b");

            var cd = Substitute.For<ITraversal<string>>();
            cd.Traverse("c").Returns("d");

            var db = Substitute.For<ITraversal<string>>();
            db.Traverse("d").Returns("b");

            var dc = Substitute.For<ITraversal<string>>();
            dc.Traverse("d").Returns("c");

            explorer.Branch("a").Returns(new ITraversal<string>[] { ab, ac });
            explorer.Branch("b").Returns(new ITraversal<string>[] { ba, bd, bc });
            explorer.Branch("c").Returns(new ITraversal<string>[] { ca, cb, cd });
            explorer.Branch("d").Returns(new ITraversal<string>[] { db, dc });

            Func<string, double> costFunction = (state) =>
            {
                switch (state)
                {
                    case "a":
                        return 0;
                    case "b":
                        return 3;
                    case "c":
                        return 1;
                    case "d":
                        return 2;
                    default:
                        return 0;
                }
            };

            var strategy = new PriorityTraversalStrategy<string>(costFunction);

            // Act
            var traverser = new GraphTraverser<string>(
                explorer,
                strategy);
            var results = traverser.Traverse("a").ToArray();

            // Assert
            Assert.Equal(new string[] { "a", "c", "d", "b" }, results);
        }
    }
}
