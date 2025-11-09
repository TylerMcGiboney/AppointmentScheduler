using AppointmentScheduler.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using AppointmentScheduler.Models;

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
    }
}
