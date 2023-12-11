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
    public class SearchNodeTest
    {
        [Fact]
        public void GetActionsToNode()
        {
            // Assign
            var root = Substitute.For<SearchNode<string>>("root", null, null, 0.0);
            var action1 = Substitute.For<ITraversal<string>>();
            var first = Substitute.For<SearchNode<string>>("first", root, action1, 0.0);
            var action2 = Substitute.For<ITraversal<string>>();
            var second = Substitute.For<SearchNode<string>>("second", first, action2, 0.0);

            // Act
            var results = second.GetActionsToNode();

            // Assert
            Assert.Equal(2, results.Length);
            Assert.Same(action1, results[0]);
            Assert.Same(action2, results[1]);
        }
    }
}
