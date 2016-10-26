using System;

namespace CoreLogic
{
	public struct Range
	{
        public double LowerBoundary { get; private set; }
        public double UpperBoundary { get; private set; }

        public bool IsEmpty { get { return LowerBoundary > UpperBoundary; } }

        public Range (double from, double to)
		{
            LowerBoundary = from; UpperBoundary = to;
		}

        public bool Contains(double x) => LowerBoundary <= x && x <= UpperBoundary;
	}
}

