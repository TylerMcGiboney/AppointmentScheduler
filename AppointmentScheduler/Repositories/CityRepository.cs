using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using AppointmentScheduler.Models;
using AppointmentScheduler.Services;
using MySql.Data.MySqlClient;

namespace AppointmentScheduler.Repositories
{
    /// <summary>
    /// Provides data access operations for the City entity.
    /// </summary>
    public class CityRepository
    {
        /// <summary>
        /// Adds a new city to the database and returns the newly created city ID.
        /// </summary>
        /// <param name="city">City object with values to insert</param>
        /// <returns>Newly created city ID</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when the insert fails or a database error occurs
        /// </exception>
        public int AddCity(City city)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Insert a new city row and let the DB generate the primary key.
                    string query = @"INSERT INTO city 
                                     (city, countryId, createDate, createdBy, lastUpdate, lastUpdateBy)
                                     VALUES (@city, @countryId, @createDate, @createdBy, @lastUpdate, @lastUpdateBy)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@city", city.CityName);
                        cmd.Parameters.AddWithValue("@countryId", city.CountryId);
                        cmd.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@createdBy", App.CurrentUser.UserName);
                        cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);
                        
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            // Insert failed, throw an exception.
                            throw new ApplicationException("Unable to add new city.");
                        }
                        // Retrieve and return the newly generated city ID.
                        long newId = cmd.LastInsertedId;
                        return (int)newId;
                    }
                }
            }
            catch (MySqlException ex)
            {
                // Handle database exceptions and wrap them in an ApplicationException.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Add city"), ex);
            }
        }

        /// <summary>
        /// Retrieves a city by its name and country ID.
        /// </summary>
        /// <param name="cityName">Name of the city</param>
        /// <param name="countryId">ID of the country</param>
        /// <returns>City object if found, otherwise null</returns>
        /// <exception cref="ApplicationException">
        /// Thrown when a database error occurs
        /// </exception>
        public City GetByNameAndCountry(string cityName, int countryId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    // Query to find the city by name and country ID.
                    string query = @"SELECT cityId, city, countryId, createDate, createdBy, lastUpdate, lastUpdateBy
                                 FROM city
                                 WHERE city = @city AND countryId = @countryId
                                 LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@city", cityName);
                        cmd.Parameters.AddWithValue("@countryId", countryId);

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                // Map database fields to City object and return it.
                                return new City
                                {
                                    CityId = Convert.ToInt32(rdr["cityId"]),
                                    CityName = Convert.ToString(rdr["city"]),
                                    CountryId = Convert.ToInt32(rdr["countryId"]),
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
                // Handle database exceptions and wrap them in an ApplicationException.
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get city by name and country"), ex);
            }
        }

        /// <summary>
        /// Retrieves the city ID for a given city and country name, creating the city if it does not exist.
        /// </summary>
        /// <param name="cityName">Name of the city</param>
        /// <param name="countryName">Name of the country</param>
        /// <returns>City ID</returns>
        public int GetOrCreateCity(string cityName, string countryName)
        {
            CountryRepository countryRepo = new CountryRepository();

            string normalizedCityName = cityName.Trim();
            string normalizedCountryName = countryName.Trim();

            // Ensure the country exists and get its ID, if not create new County.
            int countryId = countryRepo.GetOrCreateCountry(normalizedCountryName);
            // Check if the city already exists for the given country.
            City existingCity = GetByNameAndCountry(normalizedCityName, countryId);

            if (existingCity != null)
            {
                // City exists, return its ID.
                return existingCity.CityId;
            }
            else
            {
                // City does not exist, create a new one.
                City newCity = new City();
                newCity.CityName = normalizedCityName;
                newCity.CountryId = countryId;

                return AddCity(newCity);
            }
        }
    }
}

