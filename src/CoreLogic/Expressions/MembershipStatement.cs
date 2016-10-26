using System;
using System.Collections.Generic;
using System.Linq;
using CoreLogic.Classes;

namespace CoreLogic.Expressions
{
    /// <summary>
    /// The expression of that type states,
    /// that the given value lies within the stored class.
    /// </summary>
	public class MembershipStatement : Expression
	{
        public Class Class { get; private set; }
        public Parameter Parameter { get; private set; }

        public override IEnumerable<Class> ReferencedClasses
        {
            get { return new Class[] { Class }; }
        }

        public MembershipStatement(Parameter parameter, Class clazz)
		{
            if (!parameter.Classes.Contains(clazz))
            {
                throw new ArgumentException("The given class is not connected with the given parameter.");
            }

            Parameter = parameter; Class = clazz;
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

