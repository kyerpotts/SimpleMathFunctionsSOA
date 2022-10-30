using APIEndpoint;
using AuthenticatorInterface;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int token;
        internal AuthServer authServer;
        private RestClient regClient;
        private RestClient servClient;
        private List<EndpointObject> endpointList;

        public MainWindow()
        {
            InitializeComponent();
            authServer = new AuthServer();
            // init RestClients with server URL's
            // Seperate clients must be generated for each endpoint host
            authServer = new AuthServer();
            regClient = new RestClient("https://localhost:44341/api/registry/");
            servClient = new RestClient("https://localhost:44309/api/services/");
            token = 0;
            endpointList = new List<EndpointObject>();
            endpointList.Add(new EndpointObject { Name = "api", Description = "api", APIendpoint = "api", NumOperands = 2, OperandType = "integer" });
            lvServicesList.ItemsSource = endpointList;

            // Hide interface components until the user has successfully logged on
            HideInterfaceComponents();
        }

        // These methods are required to show and hide various UI elements depending on the state of the program based on the state of account verification
        private void HideInterfaceComponents()
        {
            // Dispatcher is invoked for each UI element that will be hidden/unhidden to ensure that the operations are thread-safe
            btnGetAllServices.Dispatcher.Invoke(() => btnGetAllServices.Visibility = Visibility.Hidden);
            btnSearchService.Dispatcher.Invoke(() => btnSearchService.Visibility = Visibility.Hidden);
            btnTest.Dispatcher.Invoke(() => btnTest.Visibility = Visibility.Hidden);
            tbSearchService.Dispatcher.Invoke(() => tbSearchService.Visibility = Visibility.Hidden);
            lvServicesList.Dispatcher.Invoke(() => lvServicesList.Visibility = Visibility.Hidden);
            labTestOutputlab.Dispatcher.Invoke(() => labTestOutputlab.Visibility = Visibility.Hidden);
            labTestOutput.Dispatcher.Invoke(() => labTestOutput.Visibility = Visibility.Hidden);
        }

        private void UnhideInterfaceComponents()
        {
            // Dispatcher is invoked for each UI element that will be hidden/unhidden to ensure that the operations are thread-safe
            btnGetAllServices.Dispatcher.Invoke(() => btnGetAllServices.Visibility = Visibility.Visible);
            btnSearchService.Dispatcher.Invoke(() => btnSearchService.Visibility = Visibility.Visible);
            btnTest.Dispatcher.Invoke(() => btnTest.Visibility = Visibility.Visible);
            tbSearchService.Dispatcher.Invoke(() => tbSearchService.Visibility = Visibility.Visible);
            lvServicesList.Dispatcher.Invoke(() => lvServicesList.Visibility = Visibility.Visible);
            labTestOutputlab.Dispatcher.Invoke(() => labTestOutputlab.Visibility = Visibility.Visible);
            labTestOutput.Dispatcher.Invoke(() => labTestOutput.Visibility = Visibility.Hidden);
        }

        // Async operation to validate a users login credentials and provide an authentication token.
        public async Task userLogin()
        {
            string username = "", password = "";
            // UI elements must be managed by dispatcher invocations to ensure thread-safety
            tbUsername.Dispatcher.Invoke(() => username = tbUsername.Text);
            tbPassword.Dispatcher.Invoke(() => password = tbPassword.Text);
            try
            {
                // Call the authentication servers login functions
                token = await Task.Run(() => authServer.Login(username, password));
                if (token != 0)
                {
                    MessageBox.Show("Login Successful", "Login", MessageBoxButton.OK, MessageBoxImage.Information);
                    UnhideInterfaceComponents();
                }
                else
                {
                    MessageBox.Show("Login Unsuccessful", "Login", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (FaultException<AuthenticationException> authex)
            {
                MessageBox.Show(authex.Detail.Details, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<List<EndpointObject>> GetAllServices(RestClient regClient, List<EndpointObject> epList)
        {
            // Populating the rest request with the endpoint and authorization information
            RestRequest pubRequest = new RestRequest("allservices/", Method.Get).AddHeader("Authorization", " Basic " + token.ToString());
            // The request needs to be executed asyncronously
            RestResponse resp = await regClient.GetAsync(pubRequest);

            // The list of endpoints needs to be deserialized if hte request is successful
            if (resp.IsSuccessful)
            {
                epList = JsonConvert.DeserializeObject<List<EndpointObject>>(resp.Content);
                return epList;
            }
            // If unsuccessful, and error message is generated to let the user know and then continues
            else
            {
                var badResponseMsg = new { Message = "" };
                badResponseMsg = JsonConvert.DeserializeAnonymousType(resp.Content, badResponseMsg);
                ReturnStatus reStat = JsonConvert.DeserializeObject<ReturnStatus>(badResponseMsg.Message);
                MessageBox.Show(reStat.Status + ": " + reStat.Reason, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return epList;
            }
        }

        // This method is not async as any new UI element must be opened from within the UI thread.
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Register regWindow = new Register(authServer);
            regWindow.Show();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            await userLogin();
        }

        private async void btnGetAllServices_Click(object sender, RoutedEventArgs e)
        {
            endpointList = await GetAllServices(regClient, endpointList);
            lvServicesList.Dispatcher.Invoke(() => lvServicesList.Items.Refresh());
        }
    }
}
