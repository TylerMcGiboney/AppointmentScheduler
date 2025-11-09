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
    public class CityRepository
    {
        public int AddCity(City city)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

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
                            throw new ApplicationException("Unable to add new city.");
                        }

                        long newId = cmd.LastInsertedId;
                        return (int)newId;
                    }
                }
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Add city"), ex);
            }
        }

        public City GetByNameAndCountry(string cityName, int countryId)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

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
                throw new ApplicationException(Services.ExceptionHandler.GetMessage(ex, "Get city by name and country"),ex);
            }
        }

        public int GetOrCreateCity(string cityName, string countryName)
        {
            CountryRepository countryRepo = new CountryRepository();
            
            string normalizedCityName = cityName.Trim();
            string normalizedCountryName = countryName.Trim();

            int countryId = countryRepo.GetOrCreateCountry(normalizedCountryName);


            City existingCity = GetByNameAndCountry(normalizedCityName, countryId);

            if (existingCity != null)
            {
                return existingCity.CityId;
            }
            else
            {
                City newCity = new City();
                newCity.CityName = normalizedCityName;
                newCity.CountryId = countryId;

                return AddCity(newCity);
            }
        }


    }
}

