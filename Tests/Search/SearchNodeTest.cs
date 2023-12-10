using NSubstitute;
using SolversLibrary.Search;
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
            var root = Substitute.For<SearchNode<string>>("root", null, null);
            var action1 = Substitute.For<ISearchAction<string>>();
            var first = Substitute.For<SearchNode<string>>("first", root, action1);
            var action2 = Substitute.For<ISearchAction<string>>();
            var second = Substitute.For<SearchNode<string>>("second", first, action2);

            // Act
            var results = second.GetActionsToNode();

            // Assert
            Assert.Equal(2, results.Length);
            Assert.Same(action1, results[0]);
            Assert.Same(action2, results[1]);
        }
    }
}
