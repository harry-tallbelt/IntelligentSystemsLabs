using System;

namespace IntelligentSystemsLabs.Models.Classes
{
    /// <summary>
    /// Represents class with the membership function given as
    /// u(x) = exp( -1/2 * ((x-c)/sigma)^2 )
    /// </summary>
	public class ClassWithGaussianMF : Class
	{
        public double C { get; private set; }
        public double Sigma { get; private set; }

        public ClassWithGaussianMF(string name, double c, double sigma) : base(name)
		{
            C = c; Sigma = sigma;
		}

        public override double CalculateMembershipValueFor(double value)
        {
            var t = (value - C) / Sigma;
            return Math.Exp(-t * t / 2.0);
        }
    }
}

