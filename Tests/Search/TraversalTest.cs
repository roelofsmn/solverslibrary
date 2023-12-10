using NSubstitute;
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
    }
}
