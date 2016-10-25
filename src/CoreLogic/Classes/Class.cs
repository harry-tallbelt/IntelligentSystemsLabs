using System;

namespace IntelligentSystemsLabs.Models.Classes
{
	public abstract class Class
	{
        public string Name { get; private set; }

        public Class(string name)
		{
            Name = name;
		}

        public abstract double CalculateMembershipValueFor(double value);
    }
}

