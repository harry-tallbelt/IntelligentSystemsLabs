using System;
using System.Collections.Generic;

namespace IntelligentSystemsLabs.Models
{
	public class Task
	{
		public string Name { get; set; }
		public List<Parameter> InputParameters { get; set; }
		public List<Parameter> OutputParameters { get; set; }
		public List<Rule> Rules { get; set; }

		public Task ()
		{
		}
	}
}

