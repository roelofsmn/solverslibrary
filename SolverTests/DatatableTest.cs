using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolversLibrary.PI;
using System.Collections.Generic;

namespace SolverTests
{
    [TestClass]
    public class DatatableTest
    {
        [TestMethod, TestCategory("Datatable")]
        public void Init_Empty()
        {
            var dt = new DataTable();
            Assert.AreEqual<int>(0, dt.NumberOfColumns);
            Assert.AreEqual<int>(0, dt.NumberOfRows);
        }

        [TestMethod, TestCategory("Datatable")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestNumberRows_InvalidRowNumber()
        {
            var dt = new DataTable(new Dictionary<string, double[]>
            {
                { "x", new double[] { 1, 2, 3 } },
                { "y", new double[] { 1, 2 } }
            });
        }

        [TestMethod, TestCategory("Datatable")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestNumberRows_EmptyData()
        {
            var dt = new DataTable(new Dictionary<string, double[]>
            {
            });
        }

        [TestMethod, TestCategory("Datatable")]
        public void Init_Data()
        {
            var dt = new DataTable(new Dictionary<string, double[]>
            {
                { "x", new double[] { 1, 2, 3 } },
                { "y", new double[] { 4, 5, 6 } }
            });

            Assert.AreEqual<int>(2, dt.NumberOfColumns);
            Assert.AreEqual<int>(3, dt.NumberOfRows);
        }
    }
}
