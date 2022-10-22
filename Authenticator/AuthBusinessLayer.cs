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
        public int ValidateLogin(string username, string password)
        {
            int loginToken = 0;
            // A dictionary is used to build the map of user credentials. This will allow the function to easily parse and match usernames and passwords
            Dictionary<string, string> credentialsDictionary = new Dictionary<string, string>();

            // A validation file must exist for validation to occur
            if (File.Exists(credentialsFilePath))
            {
                using (StreamReader sr = File.OpenText(credentialsFilePath))
                {
                    string readString = String.Empty;
                    while ((readString = sr.ReadLine()) != null)
                    {
                        string[] credTokens = readString.Split(':');
                        credentialsDictionary.Add(credTokens[0], credTokens[1]);
                    }
                }

                // If a match is found for login, the temporary validation token is saved to the file and the token is then returned to the caller
                if (this.matchCredentials(username, password, credentialsDictionary))
                {
                    loginToken = validationTokenNum++;
                    writeValToken(loginToken);
                }
            }
            else
            {
                throw new FileNotFoundException();
            }

            // Returns either a valid token (greater than 0) or an invalid token (0) depending on the outcome of the validation
            return loginToken;
        }

        // The validation token needs to be written to the token file each time login is authenticated.

        // Supplied credentials must be matched against the registered credential values, if supplied credentials don't match, authentication must be invalid
        private bool matchCredentials(string username, string password, Dictionary<string, string> credDictionary)
        {
            bool foundMatch = false;
            string matchPassword;

            // The credentials dict must be checked to ensure the user has registered by username, if not login credentials are invalid
            if (credDictionary.TryGetValue(username, out matchPassword))
            {
                // If the username exists in the dictionary, the given password and the stored password must match in order to produce a valid result
                if (password.Equals(matchPassword))
                {
                    foundMatch = true;
                }
            }

            // If foundMatch returns true, the password supplied was valid and the user can be authenticated.
            return foundMatch;
        }
    }
}
