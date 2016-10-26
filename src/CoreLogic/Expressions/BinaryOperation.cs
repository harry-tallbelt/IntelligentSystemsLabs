using System;
using System.Linq;
using CoreLogic.Classes;
using System.Collections.Generic;

namespace CoreLogic.Expressions
{
	public abstract class BinaryOperation : Expression
	{
        public Expression LeftArgument { get; private set; }
        public Expression RightArgument { get; private set; }

        public override IEnumerable<Class> ReferencedClasses
        {
            get { return LeftArgument.ReferencedClasses.Union(RightArgument.ReferencedClasses); }
        }

        public BinaryOperation(Expression left, Expression right)
		{
            LeftArgument = left;
            RightArgument = right;
		}
	}
}

