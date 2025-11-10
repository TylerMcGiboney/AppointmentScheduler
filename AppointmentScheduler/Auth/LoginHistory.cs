using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AppointmentScheduler.Auth
{
    public class LoginHistory
    {
        public static void RecordLogin()
        {
            string userName = AppointmentScheduler.App.CurrentUser.UserName;
            DateTime loginTime = DateTime.UtcNow;

            string logFilePath = "login_history.txt";
            string logEntry = $"{loginTime:u} - User '{userName}' logged in. {Environment.NewLine}";

            File.AppendAllText(logFilePath, logEntry);
        }
    }
}
