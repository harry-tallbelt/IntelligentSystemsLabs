using System;
using System.Collections.Generic;
using CoreLogic.Classes;

namespace CoreLogic.Expressions
{
	public class Negation : Expression
	{
        public Expression Argument { get; private set; }

        public override IEnumerable<Class> ReferencedClasses
        {
            get { return Argument.ReferencedClasses; }
        }

        public Negation(Expression argument)
        {
            Argument = argument;
        }

        public override double Evaluate(IDictionary<Class, double> membershipValues)
        {
            return 1.0 - Argument.Evaluate(membershipValues);
        }
    }
}

