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
using AppointmentScheduler.Models;
using AppointmentScheduler.Services;
using AppointmentScheduler.Auth;
using AppointmentScheduler.Resources;

namespace AppointmentScheduler.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {

            LocalizationService localizationService = new LocalizationService();
            localizationService.SetLanguageBasedOnLocation();
            InitializeComponent();


        }

        private void signInButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextBox.Text.Trim();
            string password = passwordBox.Password.Trim();

            LoginAuth authenticatedUser = new LoginAuth();

            User user = authenticatedUser.Authenticate(username, password);

            if (user != null)
            {
                // Successful login
                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                errorMessageTextBlock.Visibility = Visibility.Visible;
                errorMessageTextBlock.Text = Strings.Login_InvalidCredentials;

            }




        }

        

        
    }
}
