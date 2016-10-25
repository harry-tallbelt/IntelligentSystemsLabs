using CoreLogic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLogic
{
	public class Task
	{
		public string Name { get; private set; }
		public IEnumerable<Parameter> InputParameters { get; private set; }
		public IEnumerable<Parameter> OutputParameters { get; private set; }
		public IEnumerable<Rule> Rules { get; private set; }

		public Task (string name, IEnumerable<Parameter> inputs, IEnumerable<Parameter> outputs, IEnumerable<Rule> rules)
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

        private static bool RulesAreValid(IEnumerable<Rule> rules, IEnumerable<Parameter> outputs)
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
        private static bool AreEqualSets<T>(IEnumerable<T> first, IEnumerable<T> second) =>
            !first.Except(second).Union(second.Except(first)).Any();

        public IEnumerable<OutputParameterSolution> Solve(IDictionary<Parameter, double> inputValues)
        {
            if (InputParameters.Except(inputValues.Keys).Any())
            {
                throw new ArgumentException("Input values do not represent all the task's input parameters.");
            }

            var inputMembershipValues = InputParameters
                .SelectMany(param => param.CalculateMembershipValuesFor(inputValues[param]))  // obtains a sequence of KV-pairs instead of dictionaries
                .ToDictionary(pair => pair.Key, pair => pair.Value);    // merges all the pairs in one dictionaty; throws exception on matching keys (which should never happen)

            var outputMembershipValues = Rules
                .GroupBy(rule => rule.Class)
                .ToDictionary(group => group.Key,
                              group => group.Select(rule => rule.Expression.Evaluate(inputMembershipValues)).Max());  // joins the rules of the same target class via max (fuzzy disjunction)

            return OutputParameters.Select(param => new OutputParameterSolution(param, outputMembershipValues));
        }

        public class OutputParameterSolution
        {
            public Parameter Parameter { get; private set; }
            public Func<double, double> ParameterDistribution { get; private set; }
            public double GravityCenter { get; private set; }

            public OutputParameterSolution(Parameter parameter, IDictionary<Class, double> membershipValues)
            {
                if (membershipValues.Keys.Except(parameter.Classes).Any())
                {
                    throw new ArgumentException("The given classes list does not contain all the parameter's necessary classes.");
                }
                
                Parameter = parameter;
                ParameterDistribution = BuildFuzzyDistribution(membershipValues);
                GravityCenter = ComputeGravityCenter(ParameterDistribution, parameter.Range);
            }

            private static Func<double, double> BuildFuzzyDistribution(IDictionary<Class, double> membershipValues) =>
                x => membershipValues
                .Select(pair => Math.Min(pair.Key.CalculateMembershipValueFor(x), pair.Value))
                .Max();

            private static double ComputeGravityCenter(Func<double, double> f, Range range) =>
                Integrate(x => x * f(x), range) / Integrate(f, range);

            // TODO: Integration precision should probably be a class' field,
            // as it might be useful to (in/de)crease it depending on the Parameter.
            // On the other hand, will we actually need it?
            private static readonly double DEFAULT_INTEGRATION_PRECISION = 0.05;

            private static double Integrate(Func<double, double> f, Range range) =>
                Integrate(f, range, DEFAULT_INTEGRATION_PRECISION);

            private static double Integrate(Func<double, double> f, Range range, double precision)
            {
                var res = 0.0;

                for (double x = range.LowerBoundary; x < range.UpperBoundary; x += precision)
                {
                    res += f(x) * precision;
                }

                return res;
            }
        }
    }
}

