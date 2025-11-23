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
    /// Provides data access operations for the Customer entity.
    /// </summary>
    public class CustomerRepository
    {
        /// <summary>
        /// Adds a new customer to the database and returns the newly created customer ID.
        /// </summary>
        /// <param name="customer">Customer object with values to insert</param>
        /// <returns>Newly created customer ID</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the insert fails or a database error occurs
        /// </exception>
        public int AddCustomer(Customer customer)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Insert a new customer row and let the DB generate the primary key.
                    string query = @"INSERT INTO customer 
                                     (customerName, addressId, active, createDate, createdBy, lastUpdate, lastUpdateBy)
                                     VALUES (@customerName, @addressId, @active, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerName", customer.CustomerName);
                        cmd.Parameters.AddWithValue("@addressId", customer.AddressId);
                        cmd.Parameters.AddWithValue("@active", customer.IsActive);
                        cmd.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@createdBy", App.CurrentUser.UserName);
                        cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            // Insert failed, throw an exception.
                            throw new ApplicationException("Unable to add new customer.");
                        }
                        // Retrieve and return the newly generated customer ID.
                        long newId = cmd.LastInsertedId;
                        return (int)newId;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Add customer"), ex);
            }
        }

        /// <summary>
        /// Updates an existing customer in the database.
        /// </summary>
        /// <param name="customer">Customer object with updated values</param>
        /// <returns>True if successful, else false</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the update fails or a database error occurs
        /// </exception>
        public bool EditCustomer(Customer customer)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Update the customer record with the provided values.
                    string query = @"UPDATE customer SET 
                                     customerName = @customerName,
                                     addressId = @addressId,
                                     active = @active,
                                     lastUpdate = @lastUpdate,
                                     lastUpdateBy = @lastUpdateBy
                                     WHERE customerId = @customerId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerName", customer.CustomerName);
                        cmd.Parameters.AddWithValue("@addressId", customer.AddressId);
                        cmd.Parameters.AddWithValue("@active", customer.IsActive);
                        cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);
                        cmd.Parameters.AddWithValue("@customerId", customer.CustomerId);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            // Update failed, return false.
                            return false;
                        }
                        // Update succeeded, return true.
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Edit customer"), ex);
            }
        }

        /// <summary>
        /// Deletes a customer from the database by their customer ID.
        /// </summary>
        /// <param name="customerId">The ID of the customer to delete</param>
        /// <returns>True if deleted, false if not</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the delete fails or a database error occurs
        /// </exception>
        public bool DeleteCustomerById(int customerId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Delete the customer record with the specified customer ID.
                    string query = @"DELETE FROM customer WHERE customerId = @customerId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerId", customerId);
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
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Delete customer"), ex);
            }
        }

        /// <summary>
        /// Retrieves all customers with their related address, city, and country information.
        /// </summary>
        /// <returns>A list of all customers with flattened UI fields</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when a database error occurs
        /// </exception>
        public List<Customer> GetCustomers()
        {
            List<Customer> customers = new List<Customer>();
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Retrieve all customer records with related address, city, and country information.
                    string query = @"
                    SELECT 
                        c.customerId,
                        c.customerName,
                        c.addressId,
                        c.active,
                        c.createDate,
                        c.createdBy,
                        c.lastUpdate,
                        c.lastUpdateBy,
                        a.address,
                        a.address2,
                        a.postalCode,
                        a.phone,
                        ci.city,
                        co.country
                    FROM customer c
                    INNER JOIN address a ON c.addressId = a.addressId
                    INNER JOIN city ci ON a.cityId = ci.cityId
                    INNER JOIN country co ON ci.countryId = co.countryId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            // Map database fields to Customer object including flattened UI fields
                            var c = new Customer
                            {
                                CustomerId = Convert.ToInt32(rdr["customerId"]),
                                CustomerName = Convert.ToString(rdr["customerName"]),
                                AddressId = Convert.ToInt32(rdr["addressId"]),
                                IsActive = Convert.ToBoolean(rdr["active"]),
                                CreateDate = Convert.ToDateTime(rdr["createDate"]),
                                CreatedBy = Convert.ToString(rdr["createdBy"]),
                                LastUpdate = Convert.ToDateTime(rdr["lastUpdate"]),
                                LastUpdateBy = Convert.ToString(rdr["lastUpdateBy"]),

                                // flattened UI fields
                                Address = Convert.ToString(rdr["address"]),
                                Address2 = rdr["address2"] == DBNull.Value
                                           ? " "
                                           : Convert.ToString(rdr["address2"]),
                                PostalCode = Convert.ToString(rdr["postalCode"]),
                                PhoneNumber = Convert.ToString(rdr["phone"]),
                                City = Convert.ToString(rdr["city"]),
                                Country = Convert.ToString(rdr["country"])
                            };
                            // Add the customer to the list
                            customers.Add(c);
                        }
                    }
                }
                // Return the list of customers
                return customers;
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get customers"),ex);            }
        }

        /// <summary>
        /// Retrieves a customer by their customer ID.
        /// </summary>
        /// <param name="id">The ID of the customer to retrieve</param>
        /// <returns>Customer object if found, otherwise null</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when a database error occurs
        /// </exception>
        public Customer GetCustomerById(int id)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Retrieve the customer record with the specified customer ID.
                    string query = "SELECT * FROM customer WHERE customerId = @customerId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerId", id);
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                // Map database fields to Customer object
                                Customer c = new Customer
                                {
                                    CustomerId = Convert.ToInt32(rdr["customerId"]),
                                    CustomerName = Convert.ToString(rdr["customerName"]),
                                    AddressId = Convert.ToInt32(rdr["addressId"]),
                                    IsActive = Convert.ToBoolean(rdr["active"]),
                                    CreateDate = Convert.ToDateTime(rdr["createDate"]),
                                    CreatedBy = Convert.ToString(rdr["createdBy"]),
                                    LastUpdate = Convert.ToDateTime(rdr["lastUpdate"]),
                                    LastUpdateBy = Convert.ToString(rdr["lastUpdateBy"])
                                };
                                //Return the customer object
                                return c;
                            }
                            else
                            {
                                // Customer not found, return null.
                                return null;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get customer by ID"), ex);
            }
        }

        /// <summary>
        /// Retrieves the customer name for a given customer ID.
        /// </summary>
        /// <param name="id">The ID of the customer</param>
        /// <returns>Customer name if found</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when a database error occurs or customer is not found
        /// </exception>
        public string GetCustomerNameById(int id)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Retrieve the customer name for the specified customer ID.
                    string query = "SELECT customerName FROM customer WHERE customerId = @customerId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerId", id);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            // Return the customer name as a string.
                            return Convert.ToString(result);
                        }
                        else
                        {
                            // Customer not found, throw an exception.
                            throw new ApplicationException($"Customer with ID {id} not found.");
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get customer name by ID"), ex);
            }
        }
    }
}
