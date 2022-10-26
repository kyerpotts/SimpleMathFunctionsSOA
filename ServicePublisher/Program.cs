using AuthenticatorInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicePublisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IAuthenticatorInterface authenticationServer;
            ChannelFactory<IAuthenticatorInterface> authenticationServerFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://0.0.0.0:9000/AuthenticationService";
            authenticationServerFactory = new ChannelFactory<IAuthenticatorInterface>(tcp, URL);
            authenticationServer = authenticationServerFactory.CreateChannel();

            try
            {
                Console.WriteLine(authenticationServer.Validate(12345));
            }
            catch(FaultException<AuthenticationException> authEx)
            {
                Console.WriteLine(authEx.Message);
            }
        }
    }
}
