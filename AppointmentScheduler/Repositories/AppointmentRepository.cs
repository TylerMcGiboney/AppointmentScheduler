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
    public class AppointmentRepository
    {
        public List<Appointment> GetAppointments()
        {
            List<Appointment> appointments = new List<Appointment>();
            try
            {
                string query = "SELECT * FROM appointment";
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
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
                            appointments.Add(appointment);
                        }
                    }
                }
                return appointments;
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get Appointments"), ex);
            }
        }

        public int AddAppointment(Appointment appointment)
        {
            try
            {
                using(MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
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
                        cmd.Parameters.AddWithValue("@url", appointment.Url);
                        cmd.Parameters.AddWithValue("@start", appointment.Start);
                        cmd.Parameters.AddWithValue("@end", appointment.End);
                        cmd.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@createdBy", App.CurrentUser.UserName);
                        cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new ApplicationException("Unable to add new appointment.");
                        }
                        long newId = cmd.LastInsertedId;
                        return (int)newId;
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Add Appointment"), ex);
            }
        }

        public bool EditAppointment(Appointment appointment)
        {
            try
            {
                using(MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
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
                        cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);
                        cmd.Parameters.AddWithValue("@appointmentId", appointment.AppointmentId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if(rowsAffected == 0)
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Edit Appointment"), ex);
            }
        }

        public bool DeleteAppointmentById(int appointmentId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM appointment WHERE appointmentId = @appointmentId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@appointmentId", appointmentId);
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
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Delete Appointment"), ex);
            }
        }

    }
}
