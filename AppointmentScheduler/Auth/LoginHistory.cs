using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AppointmentScheduler.Auth
{
    /// <summary>
    /// Records user login history to a log file.
    /// Path to log file is: AppointmentScheduler\bin\Debug\login_history.txt"
    /// </summary>
    public class LoginHistory
    {
        public static void RecordLogin()
        {
            // Get current user & time of login
            int userId = AppointmentScheduler.App.CurrentUser.UserId;
            string userName = AppointmentScheduler.App.CurrentUser.UserName;
            DateTime loginTime = DateTime.UtcNow;

            // Log the login event to a file
            string logFilePath = "login_history.txt";
            string logEntry = $"{loginTime:u} - User '{userName}' logged in. UserID = {userId} {Environment.NewLine}";

            // Append the log entry to the file
            File.AppendAllText(logFilePath, logEntry);
        }
    }
}
