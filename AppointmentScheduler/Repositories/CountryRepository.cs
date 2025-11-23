using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentScheduler.Models;
using AppointmentScheduler.Services;
using MySql.Data.MySqlClient;
using ZstdSharp;

namespace AppointmentScheduler.Repositories
{
    /// <summary>
    /// Provides data access operations for the Country entity.
    /// </summary>
    public class CountryRepository
    {
        /// <summary>
        /// Retrieves all countries from the database.
        /// </summary>
        /// <returns>A list of all countries</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when a database error occurs
        /// </exception>
        public List<Country> GetAllCountries()
        {
            List<Country> countries = new List<Country>();
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Retrieve all country records
                    string query = "SELECT * FROM country";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            // Map database fields to Country object
                            Country c = new Country
                            {
                                CountryId = Convert.ToInt32(rdr["countryId"]),
                                CountryName = Convert.ToString(rdr["country"]),
                                CreateDate = Convert.ToDateTime(rdr["createDate"]),
                                CreatedBy = Convert.ToString(rdr["createdBy"]),
                                LastUpdate = Convert.ToDateTime(rdr["lastUpdate"]),
                                LastUpdateBy = Convert.ToString(rdr["lastUpdateBy"])
                            };
                            // Add the country to the list
                            countries.Add(c);
                        }
                    }
                }
                // Return the list of countries
                return countries;
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get countries"), ex);
            }
        }

        /// <summary>
        /// Adds a new country to the database and returns the newly created country ID.
        /// </summary>
        /// <param name="country">Country object with values to insert</param>
        /// <returns>Newly created country ID</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the insert fails or a database error occurs
        /// </exception>
        public int AddCountry(Country country)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Insert a new country row and let the DB generate the primary key.
                    string query = "INSERT INTO country (country,createDate,createdBy,lastUpdate,lastUpdateBy)" +
                                    "VALUES (@country,@createDate,@createdBy,@lastUpdate,@lastUpdateBy)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@country", country.CountryName);
                    cmd.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@createdBy", App.CurrentUser.UserName);
                    cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        // Insert failed, throw an exception.
                        throw new Exception("Failed to add country.");
                    }
                    // Retrieve and return the newly generated country ID.
                    long newId = cmd.LastInsertedId;
                    return (int)newId;
                }
            }
            catch (MySqlException ex)
            {
                // Handle database exceptions and wrap them in an ApplicationException.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Add country"), ex);
            }
        }

        /// <summary>
        /// Retrieves a country by its name.
        /// </summary>
        /// <param name="name">Name of the country</param>
        /// <returns>Country object if found, otherwise null</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when a database error occurs
        /// </exception>
        public Country GetByName(string name)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Query to find country by name
                    string query = "SELECT * FROM country WHERE country = @country LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@country", name);

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                // Map database fields to Country object and returns it
                                return new Country
                                {
                                    CountryId = Convert.ToInt32(rdr["countryId"]),
                                    CountryName = Convert.ToString(rdr["country"]),
                                    CreateDate = Convert.ToDateTime(rdr["createDate"]),
                                    CreatedBy = Convert.ToString(rdr["createdBy"]),
                                    LastUpdate = Convert.ToDateTime(rdr["lastUpdate"]),
                                    LastUpdateBy = Convert.ToString(rdr["lastUpdateBy"])
                                };
                            }
                        }
                    }
                }
                // No rows found
                return null;
            }
            catch (MySqlException ex)
            {
                // Wrap and rethrow database exceptions with additional context.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Look up country by name"), ex);
            }
        }

        /// <summary>
        /// Retrieves the country ID for a given country name, creating the country if it does not exist.
        /// </summary>
        /// <param name="name">Name of the country</param>
        /// <returns>Country ID</returns>
        public int GetOrCreateCountry(String name)
        {
            string normalizedName = name.Trim();
            Country existingCountry = GetByName(normalizedName);

            if (existingCountry != null)
            {
                return existingCountry.CountryId;
            }
            else
            {
                Country newCountry = new Country();
                newCountry.CountryName = normalizedName;

                int newId = AddCountry(newCountry);
                return newId;
            }
        }
    }
}
