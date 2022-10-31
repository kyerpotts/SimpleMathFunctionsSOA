using APIEndpoint;
using AuthenticatorInterface;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePublisher
{
    internal class MenuOptions
    {
        AuthServer authServer;
        RestClient client;

        public MenuOptions()
        {
            authServer = new AuthServer();
            client = new RestClient("https://localhost:44341/api");
        }

        public void DisplayMenu()
        {
            Console.WriteLine("Please select from the following options:");
            Console.WriteLine("1. Register new account.");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Publish a service");
            Console.WriteLine("4. Unpublish a service");
            Console.WriteLine("5. Quit");
        }

        public void RegisterNewAccount()
        {
            Console.WriteLine("Please enter a username.");
            string username = Console.ReadLine();
            Console.WriteLine("Please enter a password.");
            string password = Console.ReadLine();

            Console.WriteLine(authServer.Register(username, password));
        }

        public int Login()
        {
            Console.WriteLine("Please enter a username.");
            string username = Console.ReadLine();
            Console.WriteLine("Please enter a password.");
            string password = Console.ReadLine();
            int token = authServer.Login(username, password);
            return token;
        }

        // TODO: create publish and unpublish requests
        public void Publish(int token)
        {
            EndpointObject newEndpoint = new EndpointObject();
            Console.WriteLine("Please enter the name of the endpoint.");
            newEndpoint.Name = Console.ReadLine();
            Console.WriteLine("Please enter the endpoint Description");
            newEndpoint.Description = Console.ReadLine();
            Console.WriteLine("Please enter the endpoint address");
            newEndpoint.APIendpoint = Console.ReadLine();
            Console.WriteLine("Please enter the number of operands");
            int numOps;
            while (!(int.TryParse(Console.ReadLine(), out numOps)))
            {
                Console.WriteLine("Please enter the number of operands as an integer");
            }
            newEndpoint.NumOperands = numOps;
            Console.WriteLine("Please enter the type of operands");
            newEndpoint.OperandType = Console.ReadLine();
            RestRequest pubRequest = new RestRequest("registry/publish/", Method.Post).AddHeader("Authorization", " Basic " + token.ToString()).AddJsonBody(newEndpoint);

            RestResponse response = client.Execute(pubRequest);
            ReturnStatus reStat;
            if (response.IsSuccessful)
            {
                reStat = JsonConvert.DeserializeObject<ReturnStatus>(response.Content);
            }
            else
            {
                // To deserialize a bad response, an anonymous type is used to decode the response format and then retrieve the Json response message
                var badResponseMsg = new { Message = "" };
                badResponseMsg = JsonConvert.DeserializeAnonymousType(response.Content, badResponseMsg);
                reStat = JsonConvert.DeserializeObject<ReturnStatus>(badResponseMsg.Message);
            }
            Console.WriteLine(reStat.Status + " : " + reStat.Reason);
        }

        public void Unpublish(int token)
        {
            Console.WriteLine("Please enter the name of the endpoint to delete.");
            string deletedEndpoint = Console.ReadLine();
            RestRequest unPubRequest = new RestRequest("registry/unpublish/{jEndpointName}", Method.Delete).AddHeader("Authorization", " Basic " + token.ToString());
            unPubRequest.AddUrlSegment("jEndpointName", deletedEndpoint);

            RestResponse response = client.Execute(unPubRequest);
            ReturnStatus reStat;
            if (response.IsSuccessful)
            {
                reStat = JsonConvert.DeserializeObject<ReturnStatus>(response.Content);
            }
            else
            {
                // To deserialize a bad response, an anonymous type is used to decode the response format and then retrieve the Json response message
                var badResponseMsg = new { Message = "" };
                badResponseMsg = JsonConvert.DeserializeAnonymousType(response.Content, badResponseMsg);
                reStat = JsonConvert.DeserializeObject<ReturnStatus>(badResponseMsg.Message);
            }
            Console.WriteLine(reStat.Status + " : " + reStat.Reason);
        }
    }
}
