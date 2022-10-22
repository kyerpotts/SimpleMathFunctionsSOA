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
        private string validationFilePath;
        private string tokenFilePath;
        private int validationTokenNum;

        // File paths and data files initialized and created here as they may need to be accessed immediately on server init.
        public AuthDataLayer()
        {
            // Saves the authentication files to "MyDocuments" for demo purposes
            validationFilePath = Path.Combine(Environment.SpecialFolder.MyDocuments + @"\" + "authValidationFile.txt");
            tokenFilePath = Path.Combine(Environment.SpecialFolder.MyDocuments + @"\" + "authTokenFile.txt");

            // validationToken will always begin at 0. 0 will be used as an invalid token, meaning that the credentials supplied were not authenticated
            validationTokenNum = 0;

            // Need to check if credentials file exists before creating to ensure user data is not wiped
            if (!File.Exists(validationFilePath))
            {
                File.CreateText(validationFilePath);
            }

            // Token file will always be invalid when server starts up so will always need to be overwritten if it exists.
            File.CreateText(tokenFilePath);
        }

        // The user credentials supplied are appended to an existing file, delineating each string with a ":" character in order to parse later
        public void registerNewCredentials(string username, string password)
        {
            // Credentials should only be saved to an existing file. If the file does not exist, there is some problem.
            if (File.Exists(validationFilePath))
            {
                using (StreamWriter sw = File.AppendText(validationFilePath))
                {
                    sw.WriteLine(username + ":" + password);
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }


        public int ValidateCredentials(string username, string password)
        {

        }
    }
}
