using APIEndpoint;
using AuthenticatorInterface;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<EndpointObject> endpointList;
        private object listSync;

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
            endpointList = new ObservableCollection<EndpointObject>();
            listSync = new object();

            // Required to allow the listview to be updated with changes from different threads
            BindingOperations.EnableCollectionSynchronization(endpointList, listSync);
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
            spInputControls.Dispatcher.Invoke(() => spInputControls.Visibility = Visibility.Hidden);
        }

        private void UnhideInterfaceComponents()
        {
            // Dispatcher is invoked for each UI element that will be hidden/unhidden to ensure that the operations are thread-safe
            btnGetAllServices.Dispatcher.Invoke(() => btnGetAllServices.Visibility = Visibility.Visible);
            btnSearchService.Dispatcher.Invoke(() => btnSearchService.Visibility = Visibility.Visible);
            tbSearchService.Dispatcher.Invoke(() => tbSearchService.Visibility = Visibility.Visible);
            lvServicesList.Dispatcher.Invoke(() => lvServicesList.Visibility = Visibility.Visible);
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
        private async Task<ObservableCollection<EndpointObject>> SearchServices(string searchTerm, RestClient regClient, ObservableCollection<EndpointObject> ePList)
        {
            // Populating the rest request with the endpoint and authorization information
            if (!searchTerm.Equals(""))
            {
                RestRequest searchRequest = new RestRequest("search/{searchterm}", Method.Delete).AddHeader("Authorization", " Basic " + token.ToString());
                searchRequest.AddUrlSegment("searchterm", searchTerm);
                // The request needs to be executed asyncronously
                RestResponse resp = await regClient.ExecuteGetAsync(searchRequest);

                // The list of endpoints needs to be deserialized if the request is successful
                if (resp.IsSuccessful)
                {
                    ObservableCollection<EndpointObject> tempList = JsonConvert.DeserializeObject<ObservableCollection<EndpointObject>>(resp.Content);
                    ePList = await AddEPItems(tempList, ePList);
                }
                // If unsuccessful, and error message is generated to let the user know and then continues
                else
                {
                    var badResponseMsg = new { Message = "" };
                    badResponseMsg = JsonConvert.DeserializeAnonymousType(resp.Content, badResponseMsg);
                    ReturnStatus reStat = JsonConvert.DeserializeObject<ReturnStatus>(badResponseMsg.Message);
                    MessageBox.Show(reStat.Status + ": " + reStat.Reason, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return ePList;
        }

        private async Task<ObservableCollection<EndpointObject>> GetAllServices(RestClient regClient, ObservableCollection<EndpointObject> ePList)
        {
            // Populating the rest request with the endpoint and authorization information
            RestRequest getAllRequest = new RestRequest("allservices/", Method.Get).AddHeader("Authorization", " Basic " + token.ToString());
            // The request needs to be executed asyncronously
            RestResponse resp = await regClient.ExecuteGetAsync(getAllRequest);

            // The list of endpoints needs to be deserialized if the request is successful
            if (resp.IsSuccessful)
            {
                ObservableCollection<EndpointObject> tempList = JsonConvert.DeserializeObject<ObservableCollection<EndpointObject>>(resp.Content);
                return await AddEPItems(tempList, ePList);
            }
            // If unsuccessful, and error message is generated to let the user know and then continues
            else
            {
                var badResponseMsg = new { Message = "" };
                badResponseMsg = JsonConvert.DeserializeAnonymousType(resp.Content, badResponseMsg);
                ReturnStatus reStat = JsonConvert.DeserializeObject<ReturnStatus>(badResponseMsg.Message);
                MessageBox.Show(reStat.Status + ": " + reStat.Reason, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return ePList;
            }
        }

        private async Task<ObservableCollection<EndpointObject>> AddEPItems(ObservableCollection<EndpointObject> tempList, ObservableCollection<EndpointObject> ePList)
        {
            return await Task.Run(() =>
            {
                lock (listSync)
                {
                    foreach (EndpointObject tempEP in tempList)
                    {
                        bool addEP = true;

                        foreach (EndpointObject eP in ePList)
                        {
                            if (tempEP.APIendpoint.ToLower().Equals(eP.APIendpoint))
                            {
                                addEP = false;
                            }
                        }
                        if (addEP)
                        {
                            ePList.Add(tempList[0]);
                        }
                    }
                }
                return ePList;
            });
        }

        // Generates the requisite textbox fields from the information contained within the endpoint object
        private void PopulateFieldsFromSelection(EndpointObject selectedEndpoint)
        {
            // Clear the stack panel of any previous UI elements
            spInputControls.Children.Clear();

            // Create new input elements
            for (int i = 0; i < selectedEndpoint.NumOperands; i++)
            {
                TextBox tb = new TextBox() { Text = "Operand " + (i + 1), Width = 100, Height = 26, FontSize = 16, Margin = new Thickness(12, 12, 12, 12) };
                spInputControls.Children.Add(tb);
            }
            spInputControls.Visibility = Visibility.Visible;
            btnTest.Visibility = Visibility.Visible;
            labTestOutput.Visibility = Visibility.Visible;
            labTestOutputlab.Visibility = Visibility.Visible;
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
        private async void btnSearchService_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = "";
            tbSearchService.Dispatcher.Invoke(() =>
            {
                searchTerm = tbSearchService.Text;
            });

            endpointList = await SearchServices(searchTerm, regClient, endpointList);
        }

        private async void btnGetAllServices_Click(object sender, RoutedEventArgs e)
        {
            endpointList = await GetAllServices(regClient, endpointList);
        }

        private void lvServicesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateFieldsFromSelection((EndpointObject)e.AddedItems[0]);
        }

        private async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            EndpointObject endpoint = (EndpointObject)lvServicesList.SelectedItem;

            // Populating the rest request with the endpoint and authorization information
            RestRequest testRequest = new RestRequest(endpoint.APIendpoint, Method.Get).AddHeader("Authorization", " Basic " + token.ToString());
            bool checkOperands = spInputControls.Children.GetEnumerator().MoveNext();
            while (checkOperands)
            {
                string tbVal = "";
                // Retrieve the text from the texbox
                spInputControls.Dispatcher.Invoke(() =>
                {
                    TextBox tb = (TextBox)spInputControls.Children.GetEnumerator().Current;
                    tbVal = tb.Text;
                });
                // Check if the text is null
                if (tbVal.Equals(""))
                {
                    checkOperands = false;
                }
                else

                testRequest.AddJsonBody();

            }
            // The request needs to be executed asyncronously
            RestResponse resp = await regClient.ExecuteGetAsync(getAllRequest);

            // The list of endpoints needs to be deserialized if the request is successful
            if (resp.IsSuccessful)
            {
                ObservableCollection<EndpointObject> tempList = JsonConvert.DeserializeObject<ObservableCollection<EndpointObject>>(resp.Content);
                return await AddEPItems(tempList, ePList);
            }
            // If unsuccessful, and error message is generated to let the user know and then continues
            else
            {
                var badResponseMsg = new { Message = "" };
                badResponseMsg = JsonConvert.DeserializeAnonymousType(resp.Content, badResponseMsg);
                ReturnStatus reStat = JsonConvert.DeserializeObject<ReturnStatus>(badResponseMsg.Message);
                MessageBox.Show(reStat.Status + ": " + reStat.Reason, "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return ePList;
            }

        }
    }
}
