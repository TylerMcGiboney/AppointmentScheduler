using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentScheduler.Models;
using AppointmentScheduler.Services;
using MySql.Data.MySqlClient;

namespace AppointmentScheduler.Repositories
{
    /// <summary>
    /// Provides data access operations for the Appointment entity.
    /// </summary>
    public class AppointmentRepository
    {
        /// <summary>
        /// Retrieves all appointments from the database.
        /// </summary>
        /// <returns>A list of all appointments</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when a database error occurs
        /// </exception>
        public List<Appointment> GetAppointments()
        {
            List<Appointment> appointments = new List<Appointment>();
            try
            {
                //Selects all fields from the appointment table.
                string query = "SELECT * FROM appointment";
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Map database fields to Appointment object properties
                                Appointment appointment = new Appointment
                                {
                                    AppointmentId = reader.GetInt32("appointmentId"),
                                    CustomerId = reader.GetInt32("customerId"),
                                    UserId = reader.GetInt32("userId"),
                                    Title = reader.GetString("title"),
                                    Description = reader.GetString("description"),
                                    Location = reader.GetString("location"),
                                    Contact = reader.GetString("contact"),
                                    Type = reader.GetString("type"),
                                    Url = reader.GetString("url"),
                                    Start = reader.GetDateTime("start"),
                                    End = reader.GetDateTime("end"),
                                    CreateDate = reader.GetDateTime("createDate"),
                                    CreatedBy = reader.GetString("createdBy"),
                                    LastUpdate = reader.GetDateTime("lastUpdate"),
                                    LastUpdateBy = reader.GetString("lastUpdateBy")
                                };
                                // Add the appointment to the list
                                appointments.Add(appointment);
                            }
                        }
                    }
                }
                // Return the list of appointments
                return appointments;
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get Appointments"), ex);
            }
        }

        /// <summary>
        /// Retrieves appointments for a specific user by their user ID.
        /// </summary>
        /// <param name="userId">UserId is used to find all appointments where the IDs match </param>
        /// <returns>A list of appointments for a specific user</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when a database error occurs
        /// </exception>
        public List<Appointment> GetAppointmentsByUserId(int userId)
        {
            List<Appointment> appointments = new List<Appointment>();
            try
            {
                //Selects all fields from the appointment table where the userId matches the provided parameter.
                string query = "SELECT * FROM appointment WHERE userId = @userId";
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Map database fields to Appointment object properties
                                Appointment appointment = new Appointment
                                {
                                    AppointmentId = reader.GetInt32("appointmentId"),
                                    CustomerId = reader.GetInt32("customerId"),
                                    UserId = reader.GetInt32("userId"),
                                    Title = reader.GetString("title"),
                                    Description = reader.GetString("description"),
                                    Location = reader.GetString("location"),
                                    Contact = reader.GetString("contact"),
                                    Type = reader.GetString("type"),
                                    Url = reader.GetString("url"),
                                    Start = reader.GetDateTime("start"),
                                    End = reader.GetDateTime("end"),
                                    CreateDate = reader.GetDateTime("createDate"),
                                    CreatedBy = reader.GetString("createdBy"),
                                    LastUpdate = reader.GetDateTime("lastUpdate"),
                                    LastUpdateBy = reader.GetString("lastUpdateBy")
                                };
                                // Add the appointment to the list
                                appointments.Add(appointment);
                            }
                        }
                    }
                }
                // Return the list of appointments
                return appointments;
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get Appointments By User ID"), ex);
            }
        }

        /// <summary>
        /// Retrieves appointments for a specific customer by their customer ID.
        /// </summary>
        /// <param name="customerId">CustomerId is used to select appointments where Ids match</param>
        /// <returns>A list of appointments for a certain customer</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when a database error occurs
        /// </exception>
        public List<Appointment> GetAppointmentsByCustomerId(int customerId)
        {
            List<Appointment> appointments = new List<Appointment>();
            try
            {
                //Selects all fields from the appointment table where the customerId matches the provided parameter.
                string query = "SELECT * FROM appointment WHERE customerId = @customerId";
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerId", customerId);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Map database fields to Appointment object properties
                                Appointment appointment = new Appointment
                                {
                                    AppointmentId = reader.GetInt32("appointmentId"),
                                    CustomerId = reader.GetInt32("customerId"),
                                    UserId = reader.GetInt32("userId"),
                                    Title = reader.GetString("title"),
                                    Description = reader.GetString("description"),
                                    Location = reader.GetString("location"),
                                    Contact = reader.GetString("contact"),
                                    Type = reader.GetString("type"),
                                    Url = reader.GetString("url"),
                                    Start = reader.GetDateTime("start"),
                                    End = reader.GetDateTime("end"),
                                    CreateDate = reader.GetDateTime("createDate"),
                                    CreatedBy = reader.GetString("createdBy"),
                                    LastUpdate = reader.GetDateTime("lastUpdate"),
                                    LastUpdateBy = reader.GetString("lastUpdateBy")
                                };
                                //  Add the appointment to the list
                                appointments.Add(appointment);
                            }
                        }
                    }
                }
                // Return the list of appointments
                return appointments;
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get Appointments By Customer ID"), ex);
            }
        }

        /// <summary>
        /// Retrieves appointments for a specific day.
        /// </summary>
        /// <param name="date">Date value used to query for appointments</param>
        /// <returns>List of appointments on selected date</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when a database error occurs
        /// </exception>
        public List<Appointment> GetAppointmentsByDay(DateTime date)
        {
            List<Appointment> appointmentsByDay = new List<Appointment>();

            try
            {
                //Selects all fields from the appointment table where the date part of the start field matches the provided date.
                string query = "SELECT * FROM appointment WHERE DATE(start) = @date";
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        // Only the date part is used in the comparison.
                        cmd.Parameters.AddWithValue("@date", date.Date);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Map database fields to Appointment object properties
                                Appointment appointment = new Appointment
                                {
                                    AppointmentId = reader.GetInt32("appointmentId"),
                                    CustomerId = reader.GetInt32("customerId"),
                                    UserId = reader.GetInt32("userId"),
                                    Title = reader.GetString("title"),
                                    Description = reader.GetString("description"),
                                    Location = reader.GetString("location"),
                                    Contact = reader.GetString("contact"),
                                    Type = reader.GetString("type"),
                                    Url = reader.GetString("url"),
                                    Start = reader.GetDateTime("start"),
                                    End = reader.GetDateTime("end"),
                                    CreateDate = reader.GetDateTime("createDate"),
                                    CreatedBy = reader.GetString("createdBy"),
                                    LastUpdate = reader.GetDateTime("lastUpdate"),
                                    LastUpdateBy = reader.GetString("lastUpdateBy")
                                };
                                // Add the appointment to the list
                                appointmentsByDay.Add(appointment);
                            }
                        }
                    }
                }
                // Return the list of appointments for the specified day
                return appointmentsByDay;
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get appointments by day"),ex);
            }
        }

        /// <summary>
        /// Adds a new appointment to the database and returns the newly created appointment ID.
        /// </summary>
        /// <param name="appointment"Appointment object with values to insert</param>
        /// <returns>Newly created appointment ID</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the insert fails or a database error occurs
        /// </exception>
        public int AddAppointment(Appointment appointment)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    //Insert a new appointment row and let the DB generate the primary key.
                    string query = @"INSERT INTO appointment 
                                         (customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy, lastUpdate, lastUpdateBy)
                                         VALUES (@customerId, @userId, @title, @description, @location, @contact, @type, @url, @start, @end, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerId", appointment.CustomerId);
                        cmd.Parameters.AddWithValue("@userId", appointment.UserId);
                        cmd.Parameters.AddWithValue("@title", appointment.Title);
                        cmd.Parameters.AddWithValue("@description", appointment.Description);
                        cmd.Parameters.AddWithValue("@location", appointment.Location);
                        cmd.Parameters.AddWithValue("@contact", appointment.Contact);
                        cmd.Parameters.AddWithValue("@type", appointment.Type);
                        cmd.Parameters.AddWithValue("@url", appointment.Url ?? " ");
                        cmd.Parameters.AddWithValue("@start", appointment.Start);
                        cmd.Parameters.AddWithValue("@end", appointment.End);

                        // Audit fields - setting to current UTC time and current user.
                        cmd.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@createdBy", App.CurrentUser.UserName);
                        cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            // Insert failed, throw an exception.
                            throw new ApplicationException("Unable to add new appointment.");
                        }
                        // Retrieve and return the newly generated appointment ID.
                        long newId = cmd.LastInsertedId;
                        return (int)newId;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Add Appointment"), ex);
            }
        }

        /// <summary>
        /// Edits an existing appointment in the database.
        /// </summary>
        /// <param name="appointment">Appointment object with values to update</param>
        /// <returns>True if succesful, else false</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the update fails or a database error occurs
        /// </exception>
        public bool EditAppointment(Appointment appointment)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Update an existing appointment record where appointmentIds match.
                    string query = @"UPDATE appointment SET 
                                         customerId = @customerId,
                                         userId = @userId,
                                         title = @title,
                                         description = @description,
                                         location = @location,
                                         contact = @contact,
                                         type = @type,
                                         url = @url,
                                         start = @start,
                                         end = @end,
                                         lastUpdate = @lastUpdate,
                                         lastUpdateBy = @lastUpdateBy
                                         WHERE appointmentId = @appointmentId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerId", appointment.CustomerId);
                        cmd.Parameters.AddWithValue("@userId", appointment.UserId);
                        cmd.Parameters.AddWithValue("@title", appointment.Title);
                        cmd.Parameters.AddWithValue("@description", appointment.Description);
                        cmd.Parameters.AddWithValue("@location", appointment.Location);
                        cmd.Parameters.AddWithValue("@contact", appointment.Contact);
                        cmd.Parameters.AddWithValue("@type", appointment.Type);
                        cmd.Parameters.AddWithValue("@url", appointment.Url);
                        cmd.Parameters.AddWithValue("@start", appointment.Start);
                        cmd.Parameters.AddWithValue("@end", appointment.End);
                        // Audit fields - setting to current UTC time and current user. Not changing createDate and createdBy.
                        cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);
                        cmd.Parameters.AddWithValue("@appointmentId", appointment.AppointmentId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Edit Appointment"), ex);
            }
        }

        /// <summary>
        /// Deletes an appointment from the database by its appointment ID.
        /// </summary>
        /// <param name="appointmentId">The id value used to query and delete appoiontment record</param>
        /// <returns><True if deleted, false if not/returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the delete fails or a database error occurs
        /// </exception>
        public bool DeleteAppointmentById(int appointmentId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Delete the appointment record where appointmentId matches the provided parameter.
                    string query = "DELETE FROM appointment WHERE appointmentId = @appointmentId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@appointmentId", appointmentId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            // Delete failed, return false.
                            return false;
                        }
                        // Delete succeeded, return true.
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Delete Appointment"), ex);
            }
        }

    }
}
