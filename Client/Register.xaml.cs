using AuthenticatorInterface;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        AuthServer authServer;
        public Register(AuthServer authServer)
        {
            InitializeComponent();
            this.authServer = authServer;
        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string username = null;
            string password = null;
            tbUsername.Dispatcher.Invoke(new Action(() => username = tbUsername.Text));
            tbPassword.Dispatcher.Invoke(new Action(() => password = tbPassword.Text));
            await RegisterUser(username, password);
        }

        private async Task RegisterUser(string username, string password)
        {
            if (username == "" || password == "")
            {
                MessageBox.Show("Username and password fields cannot be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string returnVal = await Task.Run(() =>
                {
                    return authServer.Register(username, password);
                });
                MessageBox.Show(returnVal, "Registration", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();

            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
