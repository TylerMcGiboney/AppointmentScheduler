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
    public class CountryRepository
    {
        public List<Country> GetAllCountries()
        {
            List<Country> countries = new List<Country>();
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM country";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read()){
                            Country c = new Country
                            {
                                CountryId = Convert.ToInt32(rdr["countryId"]),
                                CountryName = Convert.ToString(rdr["country"]),
                                CreateDate = Convert.ToDateTime(rdr["createDate"]),
                                CreatedBy = Convert.ToString(rdr["createdBy"]),
                                LastUpdate = Convert.ToDateTime(rdr["lastUpdate"]),
                                LastUpdateBy = Convert.ToString(rdr["lastUpdateBy"])
                            };
                            countries.Add(c);
                        }
                    }
                }
                return countries;
            }
            catch(MySqlException ex)
            {
                throw new ApplicationException(ExceptionHandler.GetMessage(ex, "Get countries"), ex);
            }   
        }

        public int AddCountry(Country country)
        {
            try {
                using(MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    string query = "INSERT INTO country (country,createDate,createdBy,lastUpdate,lastUpdateBy)" +
                                    "VALUES (@country,@createDate,@createdBy,@lastUpdate,@lastUpdateBy)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@country", country.CountryName);
                    cmd.Parameters.AddWithValue("@createDate", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@createdBy", App.CurrentUser.UserName);
                    cmd.Parameters.AddWithValue("@lastUpdate", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@lastUpdateBy", App.CurrentUser.UserName);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if(rowsAffected == 0)
                    {
                        throw new Exception("Failed to add country.");
                    }

                    long newId = cmd.LastInsertedId;
                    return (int)newId;
                }
            }
            catch(MySqlException ex)
            {
                throw new ApplicationException(ExceptionHandler.GetMessage(ex, "Add country"), ex);
            }
        }

        public Country GetByName(string name)
        {
            try
            {
                using (MySqlConnection conn = DatabaseService.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT * FROM country WHERE country = @country LIMIT 1";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@country", name);

                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
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
                return null;
            }
            catch (MySqlException ex)
            {
                throw new ApplicationException(ExceptionHandler.GetMessage(ex, "Look up country by name"), ex);
            }
        }

        public int GetOrCreateCountry(String name)
        {
            string normalizedName = name.Trim();
            Country existingCountry = GetByName(normalizedName);

            if(existingCountry != null)
            {
                return existingCountry.CountryId;
            }
            else
            {
                Country newCountry = new Country
                {
                    CountryName = normalizedName
                };

                int newId = AddCountry(newCountry);
                return newId;
                
            }
        }
    }
}
