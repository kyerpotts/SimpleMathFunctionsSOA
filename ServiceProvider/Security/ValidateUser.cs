using AuthenticatorInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace ServiceProvider.Security
{
    public class ValidateUser
    {
        IAuthenticatorInterface authenticationServer;

        public ValidateUser()
        {
            ChannelFactory<IAuthenticatorInterface> authenticationServerFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            string URL = "net.tcp://0.0.0.0:8100/AuthenticationService";
            authenticationServerFactory = new ChannelFactory<IAuthenticatorInterface>(tcp, URL);
            authenticationServer = authenticationServerFactory.CreateChannel();
        }
        public string ValidateUserByAuthServer(int token)
        {
            return authenticationServer.Validate(token);
        }
    }
}