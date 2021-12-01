using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solvers.AssignmentProblem;

namespace SolverTests
{
    [TestClass]
    public class AssignmentProblemTest
    {
        [TestMethod, TestCategory("Hungarian")]
        public void Test_Hungarian()
        {
            //http://www.hungarianalgorithm.com/solve.php?c=16-89-61-26-33--2-46-22-74-81--22-96-6-86-81--64-32-14-16-51--50-32-64-86-46&random=1

            var c = new double[,] {
                { 16, 89, 61, 26, 33 },
                { 2, 46, 22, 74, 81 },
                { 22, 96, 6, 86, 81 },
                { 64, 32, 14, 16, 51 },
                { 50, 32, 64, 86, 46 }
            };

            var ans = AssignmentProblem.HungarianAlgorithm(c);

            double opt_sum = 0.0;
            foreach (var t in ans)
                opt_sum += c[t.Item1, t.Item2];

            Assert.AreEqual(89, opt_sum);
        }
        [TestMethod, TestCategory("Hungarian")]
        public void Test_Hungarian_MoreColumnsThanRows()
        {
            //http://www.hungarianalgorithm.com/solve.php?c=16-89-61-26-33-8--2-46-22-74-81-13--22-96-6-86-81-20--64-32-14-16-51-67--50-32-64-86-46-11&random=1

            var c = new double[,] {
                { 16, 89, 61, 26, 33, 8 },
                { 2, 46, 22, 74, 81, 13 },
                { 22, 96, 6, 86, 81, 20 },
                { 64, 32, 14, 16, 51, 67 },
                { 50, 32, 64, 86, 46, 11 }
            };

            var ans = AssignmentProblem.HungarianAlgorithm(c);

            double opt_sum = 0.0;
            foreach (var t in ans)
                opt_sum += c[t.Item1, t.Item2];

            Assert.AreEqual(64, opt_sum);
        }
        [TestMethod, TestCategory("Hungarian")]
        public void Test_Hungarian_MoreRowsThanColumns()
        {
            //http://www.hungarianalgorithm.com/solve.php?c=16-89-61-26-33--2-46-22-74-81--22-96-6-86-81--64-32-14-16-51--50-32-64-86-46--15-17-88-5-20&random=1

            var c = new double[,] {
                { 16, 89, 61, 26, 33 },
                { 2, 46, 22, 74, 81 },
                { 22, 96, 6, 86, 81 },
                { 64, 32, 14, 16, 51 },
                { 50, 32, 64, 86, 46 },
                { 15, 17, 88, 5, 20 }
            };

            var ans = AssignmentProblem.HungarianAlgorithm(c);

            double opt_sum = 0.0;
            foreach (var t in ans)
                opt_sum += c[t.Item1, t.Item2];

            Assert.AreEqual(74, opt_sum);
        }
        [TestMethod, TestCategory("Hungarian")]
        public void Test_Hungarian_WithInfinity()
        {
            //http://www.hungarianalgorithm.com/solve.php?c=16-89-61-26-33--2-46-22-74-81--22-96-6-86-81--64-32-14-9999999-9999999--50-32-64-9999999-9999999&random=1

            var c = new double[,] {
                { 16, 89, 61, 26, 33 },
                { 2, 46, 22, 74, 81 },
                { 22, 96, 6, 86, 81 },
                { 64, 32, 14, double.PositiveInfinity, double.PositiveInfinity },
                { 50, 32, 64, double.PositiveInfinity, double.PositiveInfinity }
            };

            var ans = AssignmentProblem.HungarianAlgorithm(c);

            double opt_sum = 0.0;
            foreach (var t in ans)
                opt_sum += c[t.Item1, t.Item2];

            Assert.AreEqual(155, opt_sum);
        }
        [TestMethod, TestCategory("Hungarian")]
        public void Test_Hungarian_WithInfinity_2()
        {
            //http://www.hungarianalgorithm.com/solve.php?c=16-89-61-26-33--2-46-22-74-81--22-96-6-86-81--64-32-14-9999999-9999999--50-32-64-9999999-9999999&random=1

            var c = new double[,] {
                { 16, 89, 61, double.PositiveInfinity, 33 },
                { 2, 46, 22, double.PositiveInfinity, 81 },
                { 22, 96, 6, double.PositiveInfinity, 81 },
                { 64, 32, 14, double.PositiveInfinity, double.PositiveInfinity },
                { 50, 32, 64, double.PositiveInfinity, double.PositiveInfinity }
            };

            var ans = AssignmentProblem.HungarianAlgorithm(c);

            double opt_sum = 0.0;
            foreach (var t in ans)
                opt_sum += c[t.Item1, t.Item2];

            Assert.AreEqual(73, opt_sum);
        }
    }
}
