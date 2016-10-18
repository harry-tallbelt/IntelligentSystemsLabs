using IntelligentSystemsLabs.Models.Classes;
using System;
using System.Collections.Generic;

namespace IntelligentSystemsLabs.Models
{
	public class Parameter
	{
		public string Name { get; private set; }
		public Range Range { get; private set; }
		public ISet<Class> Classes { get; private set; }

		public Parameter(string name, Range range, ISet<Class> classes)
		{
            if (range.IsEmpty)
            {
                throw new ArgumentException("A parameter cannot be defined on an empty range.");
            }

            if (classes.Count == 0)
            {
                throw new ArgumentException("A parameter should be divided into at least one class.");
            }
            
            Name = name; Range = range; Classes = classes;
		}

        /// <summary>
        /// Calculates membership values of a given value
        /// to each of classes.
        /// </summary>
        /// <param name="value">
        /// An exact value of the parameter. Note that
        /// the value should be in the parameter's range.
        /// </param>
        /// <returns>
        /// A map of the parameter's classes to their membership values.
        /// </returns>
        public Dictionary<Class, double> CalculateMembershipValuesFor(double value)
        {
            if (!Range.Contains(value))
            {
                throw new ArgumentException("The value is out of the parameter's range.");
            }

            var res = new Dictionary<Class, double>();

            foreach (var clazz in Classes)
            {
                res.Add(clazz, clazz.CalculateMembershipValueFor(value));
            }

            return res;
        }
	}
}

