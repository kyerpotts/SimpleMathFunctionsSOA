using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authenticator
{
    internal class AuthDataLayer
    {
        private string credentialsFilePath;
        private string valTokenFilePath;
        Random rnd;

        // File paths and data files initialized and created here as they may need to be accessed immediately on server init.
        public AuthDataLayer(string credentialsFilePath, string valTokenFilePath)
        {
            // File paths need to be initialized. Data is injectedd from the business layer.
            this.credentialsFilePath = credentialsFilePath;
            this.valTokenFilePath = valTokenFilePath;
            rnd = new Random();

            // Need to check if credentials file exists before creating to ensure user data is not wiped
            if (!File.Exists(credentialsFilePath))
            {
                File.CreateText(credentialsFilePath);
            }

            // Token file will always be invalid when server starts up so will always need to be overwritten if it exists.
            File.CreateText(valTokenFilePath);
        }

        // The user credentials supplied are appended to an existing file, delineating each string with a ":" character in order to parse later
        public void RegisterNewCredentials(string username, string password)
        {
            // Credentials should only be saved to an existing file. If the file does not exist, there is some problem.
            if (File.Exists(credentialsFilePath))
            {
                using (StreamWriter sw = File.AppendText(credentialsFilePath))
                {
                    sw.WriteLine(username + ":" + password);
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        // In order to validate the login of the user, user credentials must be returned to be searched in an efficient way.
        // This method returns a dictionary using the usernames as keys with the password as the values.
        public Dictionary<string, string> getCredDict()
        {
            // A dictionary is used to build the map of user credentials. This will allow the function to easily parse and match usernames and passwords
            Dictionary<string, string> credentialsDictionary = new Dictionary<string, string>();

            // A credentials file must exist for validation to occur
            if (File.Exists(credentialsFilePath))
            {
                using (StreamReader sr = File.OpenText(credentialsFilePath))
                {
                    string readString = String.Empty;
                    while ((readString = sr.ReadLine()) != null)
                    {
                        // Strings are written with ':' as a delimiter and must be split before being returned to the dictionary.
                        string[] credTokens = readString.Split(':');
                        if (credentialsDictionary.ContainsKey(credTokens[0]))
                        {
                            credentialsDictionary[credTokens[0]] = credTokens[1];
                        }
                        else
                        {
                            credentialsDictionary.Add(credTokens[0], credTokens[1]);
                        }
                    }
                }
            }

            return credentialsDictionary;
        }

        // The temp validation token needs to be written to the file whenever a valid login occurs
        public int writeValToken()
        {
            int valToken = 0;
            // If the tokenFile does not exist, an error has occured
            if (File.Exists(valTokenFilePath))
            {
                using (StreamWriter sw = File.AppendText(valTokenFilePath))
                {
                    valToken = rnd.Next(1, 10000000);
                    sw.WriteLine(valToken.ToString());
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
            return valToken;
        }

        // Parses the valToken file and returns a list of all currently valid tokens
        public List<int> readValTokens()
        {
            List<int> valTokensList = new List<int>();
            // File must exist to be read here, if file does not exist some unknown error has occured
            if (File.Exists(valTokenFilePath))
            {
                using (StreamReader sr = File.OpenText(valTokenFilePath))
                {
                    string readString = String.Empty;
                    while ((readString = sr.ReadLine()) != null)
                    {
                        valTokensList.Add(int.Parse(readString));
                    }
                }
            }
            else
            {
                throw new FileNotFoundException();
            }

            return valTokensList;
        }

        // Clears all val tokens from the val token file
        public void clearAllValTokens()
        {
            // The file can be quickly recreated with the same path rather than opening the file and removing the contents
            File.CreateText(valTokenFilePath);
        }
    }
}
