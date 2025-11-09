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
    public class CustomerRepository
    {
        public int AddCustomer(Customer customer)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
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
                            throw new ApplicationException("Unable to add new customer.");
                        }
                        long newId = cmd.LastInsertedId;
                        return (int)newId;
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Add customer"), ex);
            }
        }

        public bool EditCustomer(Customer customer)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
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
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Edit customer"), ex);
            }
        }

        public bool DeleteCustomerById(int customerId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = @"DELETE FROM customer WHERE customerId = @customerId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerId", customerId);
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
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Delete customer"), ex);
            }
        }

        public List<Customer> GetCustomers()
        {
            List<Customer> customers = new List<Customer>();
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM customer";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
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
                            customers.Add(c);
                        }
                    }
                }
                return customers;
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get customers"), ex);
            }
        }

        public Customer GetCustomerById(int id)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM customer WHERE customerId = @customerId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerId", id);
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
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
                                return c;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get customer by ID"), ex);
            }
        }
    }
}
