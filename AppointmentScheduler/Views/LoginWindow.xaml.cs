using System;
using System.Windows;
using AppointmentScheduler.Models;
using AppointmentScheduler.Services;
using AppointmentScheduler.Auth;
using AppointmentScheduler.Resources;

namespace AppointmentScheduler.Views
{
    /// <summary>
    /// Code-behind for the login window.
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            // Set UI language based on the user's timezone region
            LocalizationService localizationService = new LocalizationService();
            localizationService.SetLanguageBasedOnLocation();

            InitializeComponent();

            // Display the local system timezone name in the UI
            var tz = TimeZoneInfo.Local;
            locationLabel.Text = tz.StandardName;
        }

        private void signInButton_Click(object sender, RoutedEventArgs e)
        {
            // Read user input values
            string username = usernameTextBox.Text.Trim();
            string password = passwordBox.Password.Trim();

            // Create authentication helper
            LoginAuth authenticatedUser = new LoginAuth();

            // Attempt to authenticate the user
            User user = authenticatedUser.Authenticate(username, password);

            if (user != null)
            {
                // Login success: store logged-in user globally
                App.SetCurrentUser(user);

                // Record login timestamp in history file
                LoginHistory.RecordLogin();

                // Open the main window and close the login screen
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                // Show error message for invalid login
                errorMessageTextBlock.Visibility = Visibility.Visible;
                errorMessageTextBlock.Text = Strings.Login_InvalidCredentials;
            }
        }
    }
}
