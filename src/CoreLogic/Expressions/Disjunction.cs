using System;
using System.Collections.Generic;
using CoreLogic.Classes;

namespace CoreLogic.Expressions
{
    public class Disjunction : BinaryOperation
    {
        public Disjunction(Expression left, Expression right) : base(left, right) { }

        public override double Evaluate(IDictionary<Class, double> membershipValues)
        {
            var leftValue = LeftArgument.Evaluate(membershipValues);
            var rightValue = RightArgument.Evaluate(membershipValues);

            return Math.Max(leftValue, rightValue);
        }
    }
}

