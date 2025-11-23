using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentScheduler.Models;
using AppointmentScheduler.Services;

namespace AppointmentScheduler.Repositories
{
    /// <summary>
    /// Provides data access operations for the User entity.
    /// </summary>
    public class UserRepository
    {
        /// <summary>
        /// Retrieves the user name for a given user ID.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>User name if found</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when a database error occurs or user is not found
        /// </exception>
        public string GetUserNameByUserId(int userId)
        {
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Query to get the user name by user ID.
                    string query = "SELECT userName FROM user WHERE userId = @userId";
                    using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            // Return the user name as a string.
                            return Convert.ToString(result);
                        }
                        else
                        {
                            // User not found, throw an exception.
                            throw new ApplicationException($"User with ID {userId} not found.");
                        }
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get user name by user ID"), ex);
            }
        }
    }
}
