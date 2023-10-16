using System;
using SolversLibrary.PI;
using System.Collections.Generic;

namespace Tests
{
    public class DatatableTest
    {
        [Fact]
        [Trait("Category", "Datatable")]
        public void Init_Empty()
        {
            var dt = new DataTable();
            Assert.Equal<int>(0, dt.NumberOfColumns);
            Assert.Equal<int>(0, dt.NumberOfRows);
        }

        [Fact]
        [Trait("Category", "Datatable")]
        public void TestNumberRows_InvalidRowNumber()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var dt = new DataTable(new Dictionary<string, double[]>
                {
                    { "x", new double[] { 1, 2, 3 } },
                    { "y", new double[] { 1, 2 } }
                });
            });
        }

        [Fact]
        [Trait("Category", "Datatable")]
        public void TestNumberRows_EmptyData()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var dt = new DataTable(new Dictionary<string, double[]>
                {
                });
            });
        }

        [Fact]
        [Trait("Category", "Datatable")]
        public void Init_Data()
        {
            var dt = new DataTable(new Dictionary<string, double[]>
            {
                { "x", new double[] { 1, 2, 3 } },
                { "y", new double[] { 4, 5, 6 } }
            });

            Assert.Equal<int>(2, dt.NumberOfColumns);
            Assert.Equal<int>(3, dt.NumberOfRows);
        }
    }
}
