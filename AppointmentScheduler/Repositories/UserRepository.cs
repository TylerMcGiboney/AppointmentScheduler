using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentScheduler.Models;
using AppointmentScheduler.Services;

namespace AppointmentScheduler.Repositories
{
    public class UserRepository
    {
        public string GetUserNameByUserId(int userId)
        {
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT userName FROM user WHERE userId = @userId";
                    using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return Convert.ToString(result);
                        }
                        else
                        {
                            throw new ApplicationException($"User with ID {userId} not found.");
                        }
                    }
                }
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get user name by user ID"), ex);
            }
        }
    }
}
