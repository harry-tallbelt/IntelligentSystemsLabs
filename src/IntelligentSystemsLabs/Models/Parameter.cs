using System;
using System.Collections.Generic;

namespace IntelligentSystemsLabs.Models
{
	public class Parameter
	{
		public string Name { get; set; }
		public Range Range { get; set; }
		public List<Class> Classes { get; set; }

		public Parameter ()
		{
		}
	}
}

