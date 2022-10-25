using APIEndpoint;
using Newtonsoft.Json;
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
        // Writes an endpoint to the file in Json
        public static void WriteEndPoint(string filePath, string jEndpoint)
        {
            // If the file exists the current endpoints need to be read and deserialized before adding to them
            if (File.Exists(filePath))
            {
                // Read all of the endpoints from the file
                List<EndpointObject> endpoints = ReadEndpoints(filePath);
                // The new endpoint needs to be added to the list
                endpoints.Add(JsonConvert.DeserializeObject<EndpointObject>(jEndpoint));
                // Once the new endpoint has been added, the entire file is rewritten.
                // This will not scale well but should be fine for a small number of endpoints
                File.WriteAllText(filePath, JsonConvert.SerializeObject(endpoints));
            }
            else
            {
                // If the file does not exist it needs to be created, and a new list of endpoints initialized
                File.Create(filePath);
                List<EndpointObject> endpoints = new List<EndpointObject>();
                // The new endpoint is added to the fresh list and then written to the file.
                endpoints.Add(JsonConvert.DeserializeObject<EndpointObject>(jEndpoint));
                File.WriteAllText(filePath, JsonConvert.SerializeObject(endpoints));
            }
        }

        public static List<EndpointObject> ReadEndpoints(string filePath)
        {
            // The file must exist for information to be read. If it doesn't exist, some error has occured
            if (File.Exists(filePath))
            {
                // The file needs to be readinto a string so it can be deserialized
                string readFile = File.ReadAllText(filePath);
                // The string is deserialized and returned as a list of endpoints
                List<EndpointObject> endpoints = JsonConvert.DeserializeObject<List<EndpointObject>>(readFile);
                return endpoints;
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public static void DeleteAllEndpoints(string filePath)
        {

        }

        public static void WriteAllEndpoints(string filePath, List<EndpointObject> endpoints)
        {

        }
    }
}