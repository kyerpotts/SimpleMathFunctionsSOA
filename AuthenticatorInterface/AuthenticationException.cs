using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticatorInterface
{
    [DataContract]
    public class AuthenticationException
    {
        private string details;
        [DataMember]

        public string Details
        {
            get { return details; }
            set { details = value; }
        }
    }
}
