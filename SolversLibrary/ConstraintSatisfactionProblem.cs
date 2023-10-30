using System;
using System.Collections.Generic;
using System.Linq;

namespace Solvers
{
    public sealed class ConstraintSatisfactionProblem<Tvar, Tval>
    {
        public ConstraintSatisfactionProblem(IEqualityComparer<Tvar> variableComparer = null, IEqualityComparer<Tval> valueComparer = null)
        {
            _variableComparer = (variableComparer == null ? EqualityComparer<Tvar>.Default : variableComparer);
            _valueComparer = (valueComparer == null ? EqualityComparer<Tval>.Default : valueComparer);

            _variables = new Dictionary<Tvar, IEnumerable<Tval>>(_variableComparer);
            _constraints = new Dictionary<IEnumerable<Tvar>, BinaryConstraint>();
            _solutions = new List<Dictionary<Tvar, Tval>>();

            GetFirst = false;
            AllDiff = false;
        }
        private IEqualityComparer<Tvar> _variableComparer;
        private IEqualityComparer<Tval> _valueComparer;

        public bool AllDiff { get; set; }
        public bool GetFirst { get; set; }
        private bool _getBest;
        public Func<Tvar, Tval, bool> DomainSelector { private get; set; }
        /// <summary>
        /// Constraint on whole solution that returns true when satisfied
        /// </summary>
        public Func<Dictionary<Tvar, Tval>, bool> SolutionConstraint { private get; set; }

        /// <summary>
        /// Score of a solution, higher is better. Makes the CSP only return solutions with score equal to the maximum.
        /// </summary>
        public Func<Dictionary<Tvar, Tval>, int> SolutionScore { private get; set; }

        public void SetVariableActions(Action<Tvar, Tval> OnVariableSet, Action<Tvar, Tval> OnVariableUndo)
        {
            if (OnVariableSet == null || OnVariableUndo == null)
                throw new ArgumentNullException("VariableActions must both be set");
            _onVariableSet = OnVariableSet;
            _onVariableUndo = OnVariableUndo;
        }
        private Action<Tvar, Tval> _onVariableSet;
        private Action<Tvar, Tval> _onVariableUndo;

        private List<Dictionary<Tvar, Tval>> _solutions;
        private Dictionary<Tvar, IEnumerable<Tval>> _variables;

        private Dictionary<IEnumerable<Tvar>, BinaryConstraint> _constraints;

        private Dictionary<Tvar, Tval> _currentSolution;

        public void AddVariable(Tvar variable, IEnumerable<Tval> domain)
        {
            _variables.Add(variable, domain);
        }

        public void AddConstraint(BinaryConstraint constraint, IEnumerable<Tvar> variables)
        {
            _constraints.Add(variables, constraint);
        }

        public List<Dictionary<Tvar, Tval>> Solve(IDictionary<Tvar, Tval> startSolution = null)
        {
            if (startSolution == null)
                _currentSolution = new Dictionary<Tvar, Tval>(_variableComparer);
            else
                _currentSolution = new Dictionary<Tvar, Tval>(startSolution, _variableComparer);
            // TODO: Check the start solution is actually valid with the provided constraints and domain function
            // TODO: Check the variables in startSolution are in the variables collection

            if (AllDiff)
                AddConstraint((var1, var2, val1, val2) =>
                {
                    return !_valueComparer.Equals(val1, val2);
                },
                _variables.Keys);

            if (DomainSelector != null)
                ResolveDomains();

            _getBest = SolutionScore != null;

            var remainingVariables = new Dictionary<Tvar, List<Tval>>(_variableComparer);
            foreach (var kv in _variables)
                if (!_currentSolution.ContainsKey(kv.Key)) // Remaining variables not in current (=start) solution
                    remainingVariables.Add(kv.Key, new List<Tval>(kv.Value));

            // Renew the domains with the start solution information
            foreach (var kv in _currentSolution)
                remainingVariables = ForwardChecking(kv.Key, kv.Value, remainingVariables);

            BackTrack(remainingVariables);
            
            return _solutions;
        }

        private void ResolveDomains()
        {
            foreach (var var in _variables.Keys.ToArray())
            {
                _variables[var] = _variables[var].Where(val => { return DomainSelector(var, val); });
            }
        }
        private bool ShouldAbandonCurrentSolution()
        {
            if (SolutionConstraint == null)
                return false;

            return !SolutionConstraint(_currentSolution);
        }
        private void KeepBestSolutions()
        {
            if (_getBest)
            {
                var newScore = SolutionScore(_currentSolution);
                if (newScore > _maxScore)
                {
                    _solutions.Clear();
                    _solutions.Add(_currentSolution.ToDictionary(kv => { return kv.Key; }, kv => { return kv.Value; }, _variableComparer));
                    _maxScore = newScore;
                }
                else if (newScore == _maxScore && !(GetFirst && _solutions.Any()))
                    _solutions.Add(_currentSolution.ToDictionary(kv => { return kv.Key; }, kv => { return kv.Value; }, _variableComparer));
            }
            else
                _solutions.Add(_currentSolution.ToDictionary(kv => { return kv.Key; }, kv => { return kv.Value; }, _variableComparer));
        }
        private int _maxScore = int.MinValue;

        private void BackTrack(Dictionary<Tvar, List<Tval>> remainingVariables)
        {
            if (GetFirst && !_getBest && _solutions.Any()) // If only get a first solution without requiring a best, return with current solution
                return;
            if (ShouldAbandonCurrentSolution()) // First check if current solution is valid
                return;
            if (remainingVariables.Count == 0) // If solution is complete, add it to solution set
            {
                KeepBestSolutions();
                return;
            }
            if (remainingVariables.Any(v => { return v.Value.Count == 0; })) // A domain is empty : abandon solution
                return;

            var currentVariable = SelectVariableMRV(remainingVariables);

            foreach (var value in remainingVariables[currentVariable])
            {
                // new branch
                _currentSolution.Add(currentVariable, value); // Because of forward checking, value is valid per definition
                _onVariableSet?.Invoke(currentVariable, value); // Perform user defined action, if set

                BackTrack(ForwardChecking(currentVariable, value, remainingVariables));

                _currentSolution.Remove(currentVariable);
                _onVariableUndo?.Invoke(currentVariable, value); // Perform user defined undo action, if set
            }
        }

        private Dictionary<Tvar, List<Tval>> ForwardChecking(Tvar variable, Tval value, Dictionary<Tvar, List<Tval>> remainingVariables)
        {
            var newVariableDomains = new Dictionary<Tvar, List<Tval>>(_variableComparer);

            foreach (var otherVariable in remainingVariables.Keys.Where(r => { return !_variableComparer.Equals(r, variable); }))
            {
                newVariableDomains.Add(otherVariable, new List<Tval>()); // Add a domain entry (with empty domain)
                // Get all constraints involving current variables
                var applicableConstraints = _constraints.Where(constraint =>
                {
                    return constraint.Key.Contains(variable, _variableComparer) &&
                            constraint.Key.Contains(otherVariable, _variableComparer);
                })
                .Select(kv => kv.Value);
                // Check the values of otherVariable with the constraints. Only add to domain when all constraints are met
                foreach (var otherValue in remainingVariables[otherVariable]) // Check the valid values for this variable
                {
                    // Yes: a double negation. But it reads: if all constraints are true, valid is true (should execute faster than .All, though)
                    var valid = !applicableConstraints.Any(c => { return !c(variable, otherVariable, value, otherValue); });
                    if (valid)
                        newVariableDomains[otherVariable].Add(otherValue);
                }
            }
            //foreach (var constraint in _constraints.Where(constraint => { return constraint.Key.Contains(variable, _variableComparer); })) // Loop through all constraints involving current variable
            //{
            //    foreach (var otherVariable in constraint.Key.Where(o => { return !_variableComparer.Equals(o, variable) && remainingVariables.ContainsKey(o); })) // Consider only those other variables that are not assigned and are not the current variable
            //    {
            //        if (!newVariableDomains.ContainsKey(otherVariable))
                        
            //        foreach (var otherValue in remainingVariables[otherVariable]) // Check the valid values for this variable
            //        {
            //            var valid = constraint.Value(variable, otherVariable, value, otherValue);

            //            if (valid && !newVariableDomains[otherVariable].Contains(otherValue, _valueComparer))
            //                newVariableDomains[otherVariable].Add(otherValue); // Add value to domain if valid
            //            else if (!valid && newVariableDomains[otherVariable].Contains(otherValue, _valueComparer))
            //                newVariableDomains[otherVariable].Remove(otherValue); // Remove value from domain if invalid
            //        }
            //    }
            //}
            //foreach (var kv in remainingVariables.Where(item => { return !newVariableDomains.ContainsKey(item.Key) && !_variableComparer.Equals(item.Key, variable); }))
            //    newVariableDomains.Add(kv.Key, kv.Value);

            return newVariableDomains;
        }

        private Tvar SelectVariableMRV(Dictionary<Tvar, List<Tval>> remainingVariables)
        {
            var minDomainLength = remainingVariables.Min(o => { return o.Value.Count(); });
            return remainingVariables.First(o => { return o.Value.Count() == minDomainLength; }).Key; // TODO: Replace First by taking variable with least constraints...
        }

        public delegate bool BinaryConstraint(Tvar var1, Tvar var2, Tval val1, Tval val2);
    }

}
