using System;

namespace IntelligentSystemsLabs.Models.Classes
{
    /// <summary>
    /// Represents class with the membership function given as
    /// u(x) =
    ///        0,              x < a
    ///        (x-a)/(b-a),    a <= x < b
    ///        1,              b <= x < c
    ///        (d-x)/(d-c),    c <= x < d
    ///        0,              d <= x
    ///
    /// or, artelnatively,
    ///
    /// u(x) = max(min((x - a) / (b - a), 1, (d - x) / (d - c)), 0)
    ///
    /// </summary>
	public class ClassWithTrapezoidalMF : Class
    {
        public double A { get; private set; }
        public double B { get; private set; }
        public double C { get; private set; }
        public double D { get; private set; }

        public ClassWithTrapezoidalMF(string name, double a, double b, double c, double d) : base(name)
		{
            if (a >= b || b >= c || c >= d)
            {
                throw new ArgumentException("The parameters do not satisfy a < b < c < d.");
            }

            A = a; B = b; C = c; D = d;
		}

        public override double CalculateMembershipValueFor(double value)
        {
            if (value < A)
            {
                return 0.0; ;
            }
            else if (value < B)
            {
                return (value - A) / (B - A);
            }
            else if (value < C)
            {
                return 1.0;
            }
            else if (value < D)
            {
                return (D - value) / (D - C);
            }
            return 0.0;
        }
    }
}

