using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Registry.Models
{
    // Methods need to be called without expensive instantiation overhead when executing API services
    internal static class RegistryDataLayer
    {
        public static void WriteEndPoint(string filePath, string jEndpoint)
        {
            if (File.Exists(filePath))
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.Write(jEndpoint);
                }
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}