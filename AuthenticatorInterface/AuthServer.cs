using AuthenticatorInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticatorInterface
{
    public class AuthServer
    {
        private IAuthenticatorInterface authServer;
        public AuthServer()
        {
            ChannelFactory<IAuthenticatorInterface> dataServerFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://localhost:8200/AuthService";
            dataServerFactory = new ChannelFactory<IAuthenticatorInterface>(tcp, URL);
            authServer = dataServerFactory.CreateChannel();
        }

        public string Validate(int token)
        {
            string validationReq = "";
            try
            {
                validationReq = authServer.Validate(token);
            }
            catch (FaultException<AuthenticationException> authex)
            {
                Console.WriteLine(authex.Detail.Details);
            }
            return validationReq;
        }

        public int Login(string username, string password)
        {
            try
            {
                return authServer.Login(username, password);
            }
            catch (EndpointNotFoundException epnfe)
            {
                AuthenticationException authex = new AuthenticationException();
                authex.Details = epnfe.Message;
                throw new FaultException<AuthenticationException>(authex) ;
            }
        }

        public string Register(string username, string password)
        {
            try
            {
                return authServer.Register(username, password);
            }
            catch (FaultException<AuthenticationException> authex)
            {
                Console.WriteLine(authex.Detail.Details);
                return "Registration Unsuccessful";
            }
        }
    }
}
