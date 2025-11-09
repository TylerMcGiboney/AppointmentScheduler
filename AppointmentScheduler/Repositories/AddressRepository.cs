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
    public class AddressRepository
    {
        public int AddAddress(Address address)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO address 
                                 (address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdate, lastUpdateBy)
                                 VALUES (@address, @address2, @cityId, @postalCode, @phone, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@address", address.Address1);
                        cmd.Parameters.AddWithValue("@address2", address.Address2);
                        cmd.Parameters.AddWithValue("@cityId", address.CityId);
                        cmd.Parameters.AddWithValue("@postalCode", address.Zip);
                        cmd.Parameters.AddWithValue("@phone", address.PhoneNumber);
                        cmd.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@createdBy", App.CurrentUser.UserName);
                        cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new ApplicationException("Unable to add new address.");
                        }
                        long newId = cmd.LastInsertedId;
                        return (int)newId;
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Add address"), ex);
            }
        }

        public bool EditAddress(Address address)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE address SET 
                                 address = @address, 
                                 address2 = @address2, 
                                 cityId = @cityId, 
                                 postalCode = @postalCode, 
                                 phone = @phone, 
                                 lastUpdate = @lastUpdate, 
                                 lastUpdateBy = @lastUpdateBy
                                 WHERE addressId = @addressId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@address", address.Address1);
                        cmd.Parameters.AddWithValue("@address2", address.Address2);
                        cmd.Parameters.AddWithValue("@cityId", address.CityId);
                        cmd.Parameters.AddWithValue("@postalCode", address.Zip);
                        cmd.Parameters.AddWithValue("@phone", address.PhoneNumber);
                        cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);
                        cmd.Parameters.AddWithValue("@addressId", address.AddressId);
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
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Edit address"), ex);
            }
        }
    }
}
