using AuthenticatorInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace ServiceProvider.Models
{
    internal class AuthServer
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
            catch(FaultException<AuthenticationException> authex)
            {
                Console.WriteLine(authex.Detail.Details);
            }
            return validationReq;
        }
    }
}