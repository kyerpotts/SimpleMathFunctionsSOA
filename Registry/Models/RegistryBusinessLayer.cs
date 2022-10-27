using APIEndpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Registry.Models
{
    internal static class RegistryBusinessLayer
    {
        public static void WriteAPIEndpointToFile(string filePath, EndpointObject jEndpoint)
        {
            RegistryDataLayer.WriteEndPoint(filePath, jEndpoint);
        }

        public static List<EndpointObject> FindEndpoints(string filePath, string searchterm)
        {
            List<EndpointObject> matchedEndpoints = new List<EndpointObject>();
            List<EndpointObject> endpoints = RegistryDataLayer.ReadEndpoints(filePath);
            foreach (EndpointObject endpoint in endpoints)
            {
                if (endpoint.Description.IndexOf(searchterm, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    matchedEndpoints.Add(endpoint);
                }
            }

            return matchedEndpoints;
        }

        public static List<EndpointObject> GetAllEndpoints(string filePath)
        {
            List<EndpointObject> endpoints = RegistryDataLayer.ReadEndpoints(filePath);

            return endpoints;
        }

        // Deletes a specific endpoint given a searchterm. The searchterm should correspond to the name of the endpoint
        public static bool DeleteEndpoint(string filePath, string searchTerm)
        {
            List<EndpointObject> endpoints = RegistryDataLayer.ReadEndpoints(filePath);
            foreach(EndpointObject endpoint in endpoints)
            {
                // Matches the name of the endpoint against the searchterm to identify if the file contains the relevant endpoint
                if(endpoint.Name.Equals(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    endpoints.Remove(endpoint);
                    RegistryDataLayer.WriteAllEndpoints(filePath, endpoints);
                    // Endpoint has been found and successfully deleted
                    return true;
                }
            }
            // Endoint was not found, deletion unsuccessful
            return false;
        }
    }
}