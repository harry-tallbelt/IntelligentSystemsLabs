using System;

namespace IntelligentSystemsLabs.Models.Classes
{
    /// <summary>
    /// Represents
    /// u(x) = sig(x; a, b) - sig(x; c, d)
    ///
    /// where
    ///
    /// sig(x; a, c) = 1 / (1 + exp( -a(x - c) ))
    /// </summary>
	public class ClassWithSigmoidDifferenceMF : Class
	{
        public double A1 { get; private set; }
        public double A2 { get; private set; }
        public double C1 { get; private set; }
        public double C2 { get; private set; }

        public ClassWithSigmoidDifferenceMF(string name, double a1, double a2, double c1, double c2) : base(name)
		{
            if (a1 >= a2 || c1 <= 0.0 || c2 <= 0.0)
            {
                throw new ArgumentException();
            }

            A1 = a1; C1 = c1;
            A2 = a2; C2 = c2;
		}

        private static double Sig(double value, double a, double c) => 1.0 / (1.0 + Math.Exp(-a * (value - c)));

        public override double CalculateMembershipValueFor(double value)
        {
            return Math.Abs(Sig(value, A1, C1) - Sig(value, A2, C2));
        }
    }
}

