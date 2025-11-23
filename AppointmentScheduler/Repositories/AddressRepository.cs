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
    /// Provides data access operations for the Address entity.
    /// Handles insert and update operations against the MySQL "address" table.
    /// </summary>
    public class AddressRepository
    {

        /// <summary>
        /// Adds a new address record to the database and returns the newly created address ID.
        /// </summary>
        /// <param name="address">Address object containing values to insert.</param>
        /// <returns>
        /// The newly generated <c>addressId</c> for the inserted record.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the insert fails or a database error occurs
        /// </exception>
        public int AddAddress(Address address)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    //Insert a new address row and let the DB generate the primary key.
                    string query = @"INSERT INTO address 
                             (address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdate, lastUpdateBy)
                             VALUES (@address, @address2, @cityId, @postalCode, @phone, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        //Mapping Address properties to the parameters in the insert statement.
                        //Address2 is optional so we provide a string with a single space if it is null.
                        cmd.Parameters.AddWithValue("@address", address.Address1);
                        cmd.Parameters.AddWithValue("@address2",address.Address2 ?? " ");
                        cmd.Parameters.AddWithValue("@cityId", address.CityId);
                        cmd.Parameters.AddWithValue("@postalCode", address.Zip);
                        cmd.Parameters.AddWithValue("@phone", address.PhoneNumber);

                        //Audit fields - setting to current UTC time and current user.
                        cmd.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@createdBy", App.CurrentUser.UserName);
                        cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            // Insert failed, throw an exception.
                            throw new ApplicationException("Unable to add new address.");
                        }

                        //Get the auto-generated addressId of the newly inserted record.
                        long newId = cmd.LastInsertedId;
                        return (int)newId;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Add address"), ex);
            }
        }

        /// <summary>
        /// Updates an existing address record in the database.
        /// </summary>
        /// <param name="address">The address object containing the updated values</param>
        /// <returns>
        /// true if at least one row was affected; otherwise, false.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the update fails or a database error occurs
        /// </exception>
        public bool EditAddress(Address address)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    //Update the existing address record identified by addressId.
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
                        cmd.Parameters.AddWithValue("@address2", address.Address2 ?? "");
                        cmd.Parameters.AddWithValue("@cityId", address.CityId);
                        cmd.Parameters.AddWithValue("@postalCode", address.Zip);
                        cmd.Parameters.AddWithValue("@phone", address.PhoneNumber);
                        cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);
                        cmd.Parameters.AddWithValue("@addressId", address.AddressId);

                        //If at least one row was affected, the update was successful.
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Edit address"), ex);
            }
        }
    }
}
