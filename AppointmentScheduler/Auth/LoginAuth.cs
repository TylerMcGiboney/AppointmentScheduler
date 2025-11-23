using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentScheduler.Services;
using AppointmentScheduler.Models;
using MySql.Data.MySqlClient;

namespace AppointmentScheduler.Auth
{
    public class LoginAuth
    {

        /// <summary>
        /// Authenticate user by username and password
        /// </summary>
        /// <param name="username">Username provided by login form</param>
        /// <param name="password">Raw password entered in login form</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public User Authenticate(string username, string password)
        {
            //Query checks for matching username and password in the database
            //The password is stored in plain text per project requirements
            try
            {
                string query = @"SELECT userId, userName, active, createDate, createdBy, lastUpdate, lastUpdateBy
                               FROM user
                               WHERE userName = @username AND password = @password";


                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                //Map DB columns to User object
                                //Password is not included for security reasons
                                User authenticatedUser = new User();
                                authenticatedUser.UserId = reader.GetInt32("userId");
                                authenticatedUser.UserName = reader.GetString("userName");
                                authenticatedUser.Password = null; // Do not expose password
                                authenticatedUser.IsActive = reader.GetBoolean("active");
                                authenticatedUser.CreateDate = reader.GetDateTime("createDate");
                                authenticatedUser.CreatedBy = reader.GetString("createdBy");
                                authenticatedUser.LastUpdate = reader.GetDateTime("lastUpdate");
                                authenticatedUser.LastUpdateBy = reader.GetString("lastUpdateBy");

                                return authenticatedUser;
                            }
                            else
                            {
                                return null; // Authentication failed
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Handle database-related exceptions
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Authenticate user"), ex);
            }
        }


    }

}
