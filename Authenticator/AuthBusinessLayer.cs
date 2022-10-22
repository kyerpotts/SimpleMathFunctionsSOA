using AuthenticatorInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Authenticator
{
    internal class AuthBusinessLayer : IAuthenticatorInterface
    {
        public async void clearSavedTokens(int timespan)
        {
            throw new NotImplementedException();
        }

        public int Login(string name, string password)
        {
            throw new FaultException<>():
        }

        public string Register(string name, string password)
        {
            throw new NotImplementedException();
        }

        public string Validate(int token)
        {
            throw new NotImplementedException();
        }
    }
}
