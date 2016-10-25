using System;

namespace IntelligentSystemsLabs.Models.Expressions
{
	public abstract class BinaryOperation : Expression
	{
        public Expression LeftArgument { get; private set; }
        public Expression RightArgument { get; private set; }

        public BinaryOperation(Expression left, Expression right)
		{
            LeftArgument = left;
            RightArgument = right;
		}
	}
}

