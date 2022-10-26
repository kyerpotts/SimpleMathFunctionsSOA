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

            try
            {
                authServer.Register("user", "user");
                int token = authServer.Login("user", "user");

                Console.WriteLine(authServer.Validate(token));
                Console.ReadLine();
            }
            catch(FaultException<AuthenticationException> authEx)
            {
                Console.WriteLine(authEx.Message);
            }
        }
    }
}
