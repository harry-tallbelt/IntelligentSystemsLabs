using System.Collections.Generic;
using CoreLogic.Classes;

namespace CoreLogic.Expressions
{
	public abstract class Expression
	{
        public abstract double Evaluate(IDictionary<Class,double> membershipValues);
	}
}

