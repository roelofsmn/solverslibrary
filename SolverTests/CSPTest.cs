using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Solvers;

namespace SolverTests
{
    [TestClass]
    public class CSPTest
    {
        private ConstraintSatisfactionProblem<string, string> GetCSP()
        {
            var problem = new ConstraintSatisfactionProblem<string, string>();

            var colors = new List<string>() { "Red", "Green", "Blue" };

            problem.AddVariable("WA", colors.AsEnumerable());
            problem.AddVariable("NT", colors.AsEnumerable());
            problem.AddVariable("Q", colors.AsEnumerable());
            problem.AddVariable("NSW", colors.AsEnumerable());
            problem.AddVariable("V", colors.AsEnumerable());
            problem.AddVariable("SA", colors.AsEnumerable());
            //problem.AddVariable("T", colors.AsEnumerable());

            problem.AddConstraint((var1, var2, val1, val2) => { return val1 != val2; }, new string[] { "WA", "NT" });
            problem.AddConstraint((var1, var2, val1, val2) => { return val1 != val2; }, new string[] { "WA", "SA" });
            problem.AddConstraint((var1, var2, val1, val2) => { return val1 != val2; }, new string[] { "SA", "NT" });
            problem.AddConstraint((var1, var2, val1, val2) => { return val1 != val2; }, new string[] { "Q", "NT" });
            problem.AddConstraint((var1, var2, val1, val2) => { return val1 != val2; }, new string[] { "SA", "Q" });
            problem.AddConstraint((var1, var2, val1, val2) => { return val1 != val2; }, new string[] { "Q", "NSW" });
            problem.AddConstraint((var1, var2, val1, val2) => { return val1 != val2; }, new string[] { "SA", "NSW" });
            problem.AddConstraint((var1, var2, val1, val2) => { return val1 != val2; }, new string[] { "SA", "V" });
            problem.AddConstraint((var1, var2, val1, val2) => { return val1 != val2; }, new string[] { "V", "NSW" });

            return problem;
        }
        private void CheckSolutionIsCorrect(Dictionary<string, string> solution)
        {
            Assert.AreEqual(solution["WA"], solution["Q"]);
            Assert.AreEqual(solution["WA"], solution["V"]);
            Assert.AreEqual(solution["NT"], solution["NSW"]);
            Assert.AreNotEqual(solution["WA"], solution["NT"]);
            Assert.AreNotEqual(solution["WA"], solution["SA"]);
        }

        [TestCategory("CSP"), TestMethod]
        public void TestCSP()
        {
            var problem = GetCSP();
            var solutions = problem.Solve();

            Assert.AreEqual(6, solutions.Count);

            foreach (var solution in solutions)
            {
                CheckSolutionIsCorrect(solution);
            }
        }
        [TestCategory("CSP"), TestMethod]
        public void TestCSP_First()
        {
            var problem = GetCSP();
            problem.GetFirst = true;
            var solutions = problem.Solve();

            Assert.AreEqual(1, solutions.Count);

            CheckSolutionIsCorrect(solutions.Single());
        }
        [TestCategory("CSP"), TestMethod]
        public void TestCSP_Best()
        {
            var problem = GetCSP();
            problem.SolutionScore = (sol => { return sol.Count(kv => kv.Value == "Red"); });
            var solutions = problem.Solve();

            Assert.AreEqual(2, solutions.Count);

            foreach (var solution in solutions)
            {
                CheckSolutionIsCorrect(solution);
                Assert.AreEqual(solution["WA"], "Red");
            }
        }
        [TestCategory("CSP"), TestMethod]
        public void TestCSP_BestAndFirst()
        {
            var problem = GetCSP();
            problem.SolutionScore = (sol => { return sol.Count(kv => kv.Value == "Red"); });
            problem.GetFirst = true;
            var solutions = problem.Solve();

            Assert.AreEqual(1, solutions.Count);
            CheckSolutionIsCorrect(solutions.Single());
            Assert.AreEqual(solutions.Single()["WA"], "Red");
        }

        [TestCategory("CSP"), TestMethod]
        public void TestCSP_AllDiff()
        {
            var problem = GetCSP();
            problem.AllDiff = true;
            var solutions = problem.Solve();

            Assert.AreEqual(0, solutions.Count);
        }
        [TestCategory("CSP"), TestMethod]
        public void TestCSP_VariableActions()
        {
            var problem = GetCSP();
            int maxCount = 0;
            var setvars = new Dictionary<string, string>();
            problem.SetVariableActions((var, val) => { setvars.Add(var, val); maxCount = Math.Max(maxCount, setvars.Count); },
                (var, val) => { setvars.Remove(var); });
            var solutions = problem.Solve();

            Assert.AreEqual(6, maxCount);
            Assert.IsTrue(!setvars.Any());
        }
        [TestCategory("CSP"), TestMethod]
        public void TestCSP_WithStart()
        {
            var problem = GetCSP();
            var solutions = problem.Solve(new Dictionary<string, string>() { { "WA", "Red" } });

            Assert.AreEqual(2, solutions.Count);

            foreach (var solution in solutions)
            {
                CheckSolutionIsCorrect(solution);
            }
        }
    }
}
