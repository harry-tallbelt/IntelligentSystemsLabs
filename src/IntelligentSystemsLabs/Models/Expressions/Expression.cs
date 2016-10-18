using System.Collections.Generic;
using IntelligentSystemsLabs.Models.Classes;

namespace IntelligentSystemsLabs.Models.Expressions
{
	public abstract class Expression
	{
        public abstract double Evaluate(Dictionary<Class,double> membershipValues);
	}
}

