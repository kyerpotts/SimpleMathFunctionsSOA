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

            AuthBusinessLayer authBusinessLayer = AuthBusinessLayer.Instance("authCredentialsFile.txt", "authTokenFile.txt");
            ServiceHost host;
            NetTcpBinding tcp = new NetTcpBinding();
            host = new ServiceHost(AuthBusinessLayer.Instance("authCredentialsFile.txt", "authTokenFile.txt"));
            host.AddServiceEndpoint(typeof(IAuthenticatorInterface), tcp, "net.tcp://0.0.0.0:8200/AuthService");
            host.Open();

            // Signal for the background tasks to be cancelled
            try
            {
                Task clearTokensTask = authBusinessLayer.ClearSavedTokens(timeFrame, cancellationTokenSource.Token);
                Console.WriteLine("System Online");
                Console.ReadLine();
                cancellationTokenSource.Cancel();
                await clearTokensTask;
            }
            catch (OperationCanceledException oce)
            {
                Console.WriteLine("Background threads successfully exited: " + oce.Message);
                Console.ReadLine();
            }
            finally
            {
                host.Close();
            }
        }
    }
}
