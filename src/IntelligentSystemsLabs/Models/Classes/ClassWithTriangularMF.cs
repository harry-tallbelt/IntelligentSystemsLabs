using System;

namespace IntelligentSystemsLabs.Models.Classes
{
    /// <summary>
    /// Represents class with a membership function, given as
    /// u(x) =
    ///        0,            x < a
    ///        (x-a)/(b-a),  a <= x < b
    ///        (c-x)/(c-b),  b <= x < c
    ///        0,            c <= x
    ///
    /// or, alternatively,
    ///
    /// u(x) = max(min((x - a) / (b - a), (c - x) / (c - b)), 0)
    /// </summary>
	public class ClassWithTriangularMF : Class
	{
        public double A { get; private set; }
        public double B { get; private set; }
        public double C { get; private set; }

        public ClassWithTriangularMF(string name, double a, double b, double c) : base(name)
		{
            if (a >= b || b >= c)
            {
                throw new ArgumentException("Parameters do not satisfy a < b < c.");
            }

            A = a; B = b; C = c;
		}

        public override double CalculateMembershipValueFor(double value)
        {
            if (value < A)
            {
                return 0.0;
            }
            else if (value < B)
            {
                return (value - A) / (B - A);
            }
            else if (value < C)
            {
                return (C - value) / (C - B);
            }
            return 0.0;
        }
    }
}

