using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;


namespace AppointmentScheduler.Services
{
    /// <summary>
    /// Service for managing database connections.
    /// </summary>
    public class DatabaseService
    {
        /// <summary>
        /// Gets a new MySQL database connection using the connection string from the configuration file.
        /// </summary>
        /// <returns>Retuns a new MySQL connection</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static MySqlConnection GetConnection()
        {
            // Retrieve the connection string from the configuration file
            var connectionStringSettings = ConfigurationManager.ConnectionStrings["localdb"];
            
            
            if(connectionStringSettings == null)
            {
                // The connection string is not found in the configuration file
                throw new InvalidOperationException("The connection string to the database can not be found.");
            }

            // Get the actual connection string
            string connectionString = connectionStringSettings.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                // The connection string is empty
                throw new InvalidOperationException("The connection string: 'localdb' is empty.");
            }
            // Create and return a new MySQL connection
            return new MySqlConnection(connectionString);
        }

    }
}
