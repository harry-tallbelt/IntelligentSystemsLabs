using System;
using System.Linq;
using CoreLogic.Classes;
using CoreLogic.Expressions;

namespace CoreLogic
{
    /// <summary>
    /// Represents a rule, which is a relation
    /// between a class of an output parameter 
    /// and a fuzzy-logic expression, used to
    /// calculate class's membership value.
    /// </summary>
	public class Rule
    {
        public Parameter Parameter { get; private set; }
        public Class Class { get; private set; }
        public Expression Expression { get; private set; }

        public Rule (Parameter parameter, Class clazz, Expression expression)
        {
            if (!parameter.Classes.Contains(clazz))
            {
                throw new ArgumentException("The given class is not connected with the given parameter.");
            }

            Parameter = parameter; Class = clazz; Expression = expression;
		}
	}
}

