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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int token { get; set; }
        internal AuthServer authServer;
        private RestClient regClient;
        private RestClient servClient;

        public MainWindow()
        {
            InitializeComponent();
            authServer = new AuthServer();
        }
    }
}
