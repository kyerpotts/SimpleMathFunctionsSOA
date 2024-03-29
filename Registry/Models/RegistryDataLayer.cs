﻿using APIEndpoint;
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
        public static void WriteEndPoint(string filePath, EndpointObject jEndpoint)
        {
            // If the file exists the current endpoints need to be read and deserialized before adding to them
            if (File.Exists(filePath))
            {
                // Read all of the endpoints from the file
                List<EndpointObject> endpoints = ReadEndpoints(filePath);

                // If the file is empty the List will be null, so instantiate the list
                if (endpoints == null)
                {
                    endpoints = new List<EndpointObject>();
                }
                // The new endpoint needs to be added to the list
                endpoints.Add(jEndpoint);
                // Once the new endpoint has been added, the entire file is rewritten.
                // This will not scale well but should be fine for a small number of endpoints
                File.WriteAllText(filePath, JsonConvert.SerializeObject(endpoints));
            }
            else
            {
                List<EndpointObject> endpoints = new List<EndpointObject>();
                // The new endpoint is added to the fresh list and then written to the file.
                endpoints.Add(jEndpoint);
                //endpoints.Add(JsonConvert.DeserializeObject<EndpointObject>(jEndpoint));
                WriteAllEndpoints(filePath, endpoints);
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

        // When all endpoints are deleted, the file must still contain json data for the List<EndpointObject>
        // In order to initialize it when other read/write operations are called
        // This function may be redundant
        public static void DeleteAllEndpoints(string filePath)
        {
                // An empty list of endpoint objects is instantiated to be written to the file
                List<EndpointObject> emptyEndpointList = new List<EndpointObject>();
                // The empty list of endpoint objects is written to the file to
                WriteAllEndpoints(filePath, emptyEndpointList);
        }

        public static void WriteAllEndpoints(string filePath, List<EndpointObject> endpoints)
        {
            // File is overwritten with a blank file to ensure no data will be corrupted
            File.Create(filePath).Dispose();
            // List of EndpointObjects is serialized into json and written to the text file
            File.WriteAllText(filePath, JsonConvert.SerializeObject(endpoints));
        }
    }
}