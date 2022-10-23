using AuthenticatorInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Authenticator
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, InstanceContextMode = InstanceContextMode.Single)]
    internal class AuthBusinessLayer : IAuthenticatorInterface
    {
        AuthDataLayer dataLayer;
        Mutex mutexLock;
        Mutex writeMutex;
        private static object instanceMutex = new object();
        int currentReaders = 0;

        // Creates a singleton to be used for Authentication. This ensures that a new business layer object is not created whenever the server receives a call
        private static volatile AuthBusinessLayer instance = null;

        public static AuthBusinessLayer Create(string credentialsFilePath, string valTokenPath)
        {
            lock (instanceMutex)
            {
                if (instance == null)
                {
                    instance = new AuthBusinessLayer(credentialsFilePath, valTokenPath);

                    return instance;
                }
                else
                {
                    throw new InvalidOperationException("Instance has already been created. Please use RetrieveInstance function");
                }

            }
        }

        public static AuthBusinessLayer RetrieveInstance()
        {
            lock (instanceMutex)
            {
                if (instance != null)
                {
                    return instance;
                }
                else
                {
                    throw new InvalidOperationException("Instance has not been created. Please use Create function");
                }
            }
        }

        // Standard constructor made private to prevent instantiation outside of the singleton
        private AuthBusinessLayer(string credentialsFilePath, string valTokenPath)
        {
            dataLayer = new AuthDataLayer(credentialsFilePath, valTokenPath);

            mutexLock = new Mutex();
            writeMutex = new Mutex();
        }

        // Uses the given timespan in minutes to determine when the validation tokens file needs to be cleared
        public async Task ClearSavedTokens(int timespanMinutes, CancellationToken cancellationToken)
        {
            while (true)
            {
                Task delayTask = Task.Delay(TimeSpan.FromMinutes(timespanMinutes), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                await delayTask;
                Task clearTokensTask = Task.Run(() =>
                {
                    if (writeMutex.WaitOne())
                    {
                        try
                        {
                            dataLayer.clearAllValTokens();
                        }
                        finally
                        {
                            writeMutex.ReleaseMutex();
                        }
                    }
                });
                await clearTokensTask;
            }
        }

        public int Login(string username, string password)
        {
            int loginToken = 0;
            try
            {
                // A dictionary is used to build the map of user credentials. This will allow the function to easily parse and match usernames and passwords
                Dictionary<string, string> credentialsDictionary = dataLayer.getCredDict();


                // If a match is found for login, the temporary validation token is saved to the file and the token is then returned to the caller
                if (matchCredentials(username, password, credentialsDictionary))
                {
                    writeMutex.WaitOne();
                    loginToken = dataLayer.writeValToken();
                    writeMutex.ReleaseMutex();
                }
            }
            catch (FileNotFoundException fnfe)
            {
                AuthenticationException AuthEx = new AuthenticationException();
                AuthEx.Details = fnfe.Message;
                throw new FaultException<AuthenticationException>(AuthEx);
            }

            // Returns either a valid token (greater than 0) or an invalid token (0) depending on the outcome of the validation
            return loginToken;

        }

        public string Register(string name, string password)
        {
            try
            {
                writeMutex.WaitOne();
                dataLayer.RegisterNewCredentials(name, password);
                writeMutex.ReleaseMutex();
                return "Successfully Registered";
            }
            catch (FileNotFoundException fnfe)
            {
                AuthenticationException AuthEx = new AuthenticationException();
                AuthEx.Details = fnfe.Message;
                throw new FaultException<AuthenticationException>(AuthEx);
            }
        }

        public string Validate(int token)
        {
            try
            {
                // Tokenlist is shared amongst threads and therefor needs to be syncronized properly
                // These threads are sync'd with a generic implementation of the readers/writers problem solution
                mutexLock.WaitOne();
                currentReaders++;
                if (currentReaders == 1)
                {
                    writeMutex.WaitOne();
                    mutexLock.ReleaseMutex();
                }
                List<int> tokenList = dataLayer.readValTokens();
                mutexLock.WaitOne();
                currentReaders--;
                if (currentReaders == 0)
                {
                    writeMutex.ReleaseMutex();
                    mutexLock.ReleaseMutex();
                }

                // Each item in the list is checked against the provided token to see if there's a match
                foreach (int item in tokenList)
                {
                    if (token == item)
                    {
                        return "validated";
                    }
                }
                return "not validated";

            }
            catch (FileNotFoundException fnfe)
            {
                AuthenticationException AuthEx = new AuthenticationException();
                AuthEx.Details = fnfe.Message;
                throw new FaultException<AuthenticationException>(AuthEx);
            }
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
