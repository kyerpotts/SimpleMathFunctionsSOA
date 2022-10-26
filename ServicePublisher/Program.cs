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
            AuthServer authServer = new AuthServer();
            //IAuthenticatorInterface authenticationServer;
            //ChannelFactory<IAuthenticatorInterface> authenticationServerFactory;
            //NetTcpBinding tcp = new NetTcpBinding();
            //string URL = "net.tcp://0.0.0.0:8200/AuthService";
            //authenticationServerFactory = new ChannelFactory<IAuthenticatorInterface>(tcp, URL);
            //authenticationServer = authenticationServerFactory.CreateChannel();

            try
            {
                //authenticationServer.Register("user", "user");
                //int token = authenticationServer.Login("user", "user");

                Console.WriteLine(authServer.Validate(12345));
                Console.ReadLine();
            }
            catch(FaultException<AuthenticationException> authEx)
            {
                Console.WriteLine(authEx.Message);
            }
        }
    }
}
