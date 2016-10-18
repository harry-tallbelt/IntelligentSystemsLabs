using System;
using System.Collections.Generic;
using IntelligentSystemsLabs.Models.Classes;

namespace IntelligentSystemsLabs.Models.Expressions
{
    public class Disjunction : BinaryOperation
    {
        public Disjunction(Expression left, Expression right) : base(left, right) { }

        public override double Evaluate(Dictionary<Class, double> membershipValues)
        {
            var leftValue = LeftArgument.Evaluate(membershipValues);
            var rightValue = RightArgument.Evaluate(membershipValues);

            return Math.Max(leftValue, rightValue);
        }
    }
}

