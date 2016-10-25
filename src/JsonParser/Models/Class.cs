using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonParser.Models
{
    internal class Class
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Dictionary<string, double> Parameters { get; set; }
    }
}
