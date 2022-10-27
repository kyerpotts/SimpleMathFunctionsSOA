using AuthenticatorInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServicePublisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AuthServer authServer = new AuthServer();
            MenuOptions menuOptions = new MenuOptions();
            int token = 0;

            bool contProg = true;

            try
            {
                while (contProg)
                {
                    menuOptions.DisplayMenu();
                    int menuSelection;
                    if (int.TryParse(Console.ReadLine(), out menuSelection))
                    {
                        switch (menuSelection)
                        {
                            case 1:
                                menuOptions.RegisterNewAccount();
                                break;
                            case 2:
                                token = menuOptions.Login();
                                break;
                            case 3:
                                menuOptions.Publish(token);
                                break;
                            case 4:
                                menuOptions.Unpublish(token);
                                break;
                            case 5:
                                contProg = false;
                                break;
                            default:
                                Console.WriteLine("Invalid menu selection, please try again.");
                                break;
                        }

                    }

                }
                Console.WriteLine("Exiting the program. Press enter to close.");
                Console.ReadLine();
            }
            catch (FaultException<AuthenticationException> authEx)
            {
                Console.WriteLine(authEx.Message);
                Console.WriteLine("Exiting the program. Press enter to close.");
                Console.ReadLine();
            }
        }
    }
}
