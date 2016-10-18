using System;
using System.Collections.Generic;
using IntelligentSystemsLabs.Models.Classes;

namespace IntelligentSystemsLabs.Models.Expressions
{
    /// <summary>
    /// The expression of that type states,
    /// that the given value lies within the stored class.
    /// </summary>
	public class MembershipStatement : Expression
	{
        public Class Class { get; private set; }

		public MembershipStatement(Class clazz)
		{
            Class = clazz;
		}

        public override double Evaluate(IDictionary<Class, double> membershipValues)
        {
            if (!membershipValues.ContainsKey(Class))
            {
                throw new ArgumentException($"{nameof(membershipValues)} dictionary does not contain class {Class.Name}");
            }

            return membershipValues[Class];
        }
    }
}

