using CoreLogic.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreLogic
{
	public class Parameter
	{
		public string Name { get; private set; }
		public Range Range { get; private set; }
		public IEnumerable<Class> Classes { get; private set; }

		public Parameter(string name, Range range, IEnumerable<Class> classes)
		{
            if (range.IsEmpty)
            {
                throw new ArgumentException("A parameter cannot be defined on an empty range.");
            }

            if (!classes.Any())
            {
                throw new ArgumentException("A parameter should be divided into at least one class.");
            }

            if (classes.GroupBy(c => c.Name).Any(g => g.Count() > 1))
            {
                throw new ArgumentException("Some of the classes' names are not unique.");
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
        public IDictionary<Class, double> CalculateMembershipValuesFor(double value)
        {
            if (!Range.Contains(value))
            {
                throw new ArgumentException("The value is out of the parameter's range.");
            }

            return Classes.ToDictionary(clazz => clazz, clazz => clazz.CalculateMembershipValueFor(value));
        }
	}
}

