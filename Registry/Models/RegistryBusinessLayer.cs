using APIEndpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Registry.Models
{
    internal static class RegistryBusinessLayer
    {
        public static void WriteAPIEndpointToFile(string filePath, string jEndpoint)
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

        public static void DeleteEndpoint(string filePath)
        {
            List<EndpointObject> endpoints = RegistryDataLayer.ReadEndpoints(filePath);
        }
    }
}