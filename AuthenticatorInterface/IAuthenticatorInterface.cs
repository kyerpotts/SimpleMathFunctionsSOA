using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AuthenticatorInterface
{
    [ServiceContract]
    public interface IAuthenticatorInterface
    {
        [OperationContract]
        [FaultContract(typeof(AuthenticationException))]
        string Register(string name, string password);

        [OperationContract]
        [FaultContract(typeof(AuthenticationException))]
        int Login(string name, string password);

        [OperationContract]
        [FaultContract(typeof(AuthenticationException))]
        string Validate(int token);
    }
}
