using AuthenticatorInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Authenticator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Authentication Server is starting");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            AuthBusinessLayer authBusinessLayer = AuthBusinessLayer.Create("authCredentialsFile.txt",
                                                                           "authTokenFile.txt");
            // A timeframe must be provided for the interval clearing of tokens
            Console.WriteLine("Please provide a timeframe for session tokens to be cleared in minutes:");
            bool timeFrameProvided = false;
            // A cancellation token is used to end the async operation of the backround thread that runs in a loop
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            // Loops until a valid integer timeframe has been provided by the user
            int timeFrame = 0;
            while (timeFrameProvided == false)
            {
                if (int.TryParse(Console.ReadLine(), out timeFrame))
                {
                    timeFrameProvided = true;
                }
            }
            Task clearTokensTask = authBusinessLayer.ClearSavedTokens(timeFrame, cancellationTokenSource.Token);

            host = new ServiceHost(authBusinessLayer);
            host.AddServiceEndpoint(typeof(IAuthenticatorInterface), tcp, "net.tcp://0.0.0.0:8100/AuthenticationService");
            host.Open();
            Console.WriteLine("System Online");
            Console.ReadLine();

            // Signal for the background tasks to be cancelled
            cancellationTokenSource.Cancel();
            try
            {
                await clearTokensTask;
            }
            catch (OperationCanceledException oce)
            {
                Console.WriteLine("Background threads successfully exited: " + oce.Message);
                Console.ReadLine();
            }

            host.Close();
        }
    }
}
