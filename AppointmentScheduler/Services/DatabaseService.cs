using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;


namespace AppointmentScheduler.Services
{
    public class DatabaseService
    {
        public static MySqlConnection GetConnection()
        {
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["localdb"];
            
            if(connectionStringSettings == null)
            {
                throw new InvalidOperationException("The connection string to the database can not be found.");
            }

            string connectionString = connectionStringSettings.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("The connection string: 'localdb' is empty.");
            }
            return new MySqlConnection(connectionString);
        }

    }
}
