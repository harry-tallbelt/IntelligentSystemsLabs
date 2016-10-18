using IntelligentSystemsLabs.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntelligentSystemsLabs.Models
{
	public class Task
	{
		public string Name { get; private set; }
		public ISet<Parameter> InputParameters { get; private set; }
		public ISet<Parameter> OutputParameters { get; private set; }
		public ISet<Rule> Rules { get; private set; }

		public Task (string name, ISet<Parameter> inputs, ISet<Parameter> outputs, ISet<Rule> rules)
        {
            if (!RulesAreValid(rules, outputs))
            {
                throw new ArgumentException("Either rules do not cover all of the output parameters or link the parameters not listed as output.");
            }

            Name = name;
            InputParameters = inputs;
            OutputParameters = outputs;
            Rules = rules;
		}

        private static bool RulesAreValid(ISet<Rule> rules, ISet<Parameter> outputs)
        {
            var allOutputClasses = outputs
                .Select(parameter => parameter.Classes.AsEnumerable())
                .Aggregate((left, right) => left.Concat(right));
            
            var classesWithRules = rules.Select(rule => rule.Class);

            return AreEqualSets(allOutputClasses, classesWithRules);
        }

        /// <summary>
        /// Returns true if both the sequences contain
        /// the same elements regardless of their order
        /// and with possible dublicates.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private static bool AreEqualSets<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            return !first.Except(second).Union(second.Except(first)).Any();
        }

        public class OneParameterSolution
        {
            public Func<double, double> ParameterDistribution { get; private set; }
            public double GravityCenter { get; private set; }
            
            public OneParameterSolution(IDictionary<Class,double> membershipValues)
            {
                throw new NotImplementedException();
            }
        }

        public IDictionary<Parameter,OneParameterSolution> Solve(IDictionary<Parameter, double> inputValues)
        {
            throw new NotImplementedException();
        }
	}
}

