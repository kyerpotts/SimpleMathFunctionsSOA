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
        private int validationTokenNum;

        // File paths and data files initialized and created here as they may need to be accessed immediately on server init.
        public AuthDataLayer()
        {
            // Saves the authentication files to "MyDocuments" for demo purposes
            credentialsFilePath = Path.Combine(Environment.SpecialFolder.MyDocuments + @"\" + "authCredentialsFile.txt");
            valTokenFilePath = Path.Combine(Environment.SpecialFolder.MyDocuments + @"\" + "authTokenFile.txt");

            // validationToken will always begin at 0. 0 will be used as an invalid token, meaning that the credentials supplied were not authenticated
            validationTokenNum = 0;

            // Need to check if credentials file exists before creating to ensure user data is not wiped
            if (!File.Exists(valTokenFilePath))
            {
                File.CreateText(valTokenFilePath);
            }

            // Token file will always be invalid when server starts up so will always need to be overwritten if it exists.
            File.CreateText(valTokenFilePath);
        }

        // The user credentials supplied are appended to an existing file, delineating each string with a ":" character in order to parse later
        public void registerNewCredentials(string username, string password)
        {
            // Credentials should only be saved to an existing file. If the file does not exist, there is some problem.
            if (File.Exists(credentialsFilePath))
            {
                using (StreamWriter sw = File.AppendText(credentialsFilePath))
                {
                    sw.WriteLine(username + ":" + password + Environment.NewLine);
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public Dictionary<string, string> getCredDict()
        {
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
            }

            return credentialsDictionary;
        }

        // The temp validation token needs to be written to the file whenever a valid login occurs
        public void writeValToken(int valToken)
        {
            // If the tokenFile does not exist, an error has occured
            if (File.Exists(valTokenFilePath))
            {
                using (StreamWriter sw = File.AppendText(valTokenFilePath))
                {
                    sw.WriteLine(valToken.ToString() + Environment.NewLine);
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        // Parses the valToken file and returns a list of all currently valid tokens
        public List<int> readValTokens()
        {
            List<int> valTokensList = new List<int>();
            // 
            if (File.Exists(valTokenFilePath))
            {
                using (StreamReader sr = File.OpenText(valTokenFilePath))
                {
                    string readString = String.Empty;
                    while ((readString = sr.ReadLine()) != null)
                    {

                    }
                }
            }
            else
            {
                throw new FileNotFoundException();
            }

            return valTokensList;
        }
    }
}
