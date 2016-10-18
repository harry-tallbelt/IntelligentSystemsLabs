using System;

namespace IntelligentSystemsLabs.Models.Classes
{
    /// <summary>
    /// Represents class with the membership function given as
    /// u(x) = 1 / (1 + abs( (x-c)/a )^2b )
    /// </summary>
	public class ClassWithGeneralisedBellMF : Class
	{
        public double A { get; private set; }
        public double B { get; private set; }
        public double C { get; private set; }

        public ClassWithGeneralisedBellMF(string name, double a, double b, double c) : base(name)
		{
            A = a; B = b; C = c;
		}

        public override double CalculateMembershipValueFor(double value)
        {
            return 1.0 / (1.0 + Math.Abs(Math.Pow((value - C) / A, 2 * B)));
        }
    }
}

