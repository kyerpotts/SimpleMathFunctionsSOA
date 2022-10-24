using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIEndpoint
{
    public class EndpointObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string APIendpoint { get; set; }
        public int NumOperands { get; set; } 
        public string OperandType { get; set; } 
    }
}
