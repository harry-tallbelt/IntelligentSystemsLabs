using System;
using System.Collections.Generic;
using IntelligentSystemsLabs.Models.Classes;

namespace IntelligentSystemsLabs.Models.Expressions
{
	public class Negation : Expression
	{
        public Expression Argument { get; private set; }

		public Negation(Expression argument)
        {
            Argument = argument;
        }

        public override double Evaluate(Dictionary<Class, double> membershipValues)
        {
            return 1.0 - Argument.Evaluate(membershipValues);
        }
    }
}

