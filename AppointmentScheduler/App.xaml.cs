using AppointmentScheduler.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AppointmentScheduler.Models;
using AppointmentScheduler.Views;
using System.Globalization;

namespace AppointmentScheduler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ICurrentUserService CurrentUser { get; } = new CurrentUserService();

        public static void SetCurrentUser(User user)
        {
            CurrentUser.Set(user);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var systemCulture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            string cultureCode;
            if (systemCulture == "es")
            {
                cultureCode = "es-ES";
            }
            else
            {
                cultureCode = "en-US";
            }

            LocalizationService localizationService = new LocalizationService();
            localizationService.ChangeLanguage(cultureCode);

            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}
