using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonParser.Models
{
    internal class Expression
    {
        public string type { get; set; }
        public string var_name { get; set; }
        public string class_name { get; set; }
        public Expression arg { get; set; }
        public Expression left { get; set; }
        public Expression right { get; set; }
    }
}
