using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Models
{
    /// <summary>
    /// Represents a city in the system.
    /// </summary>
    public class City
    {
        // Primary key of the city record.
        public int CityId { get; set; }
        public string CityName { get; set; }

        // Foreign key referencing the Country record.
        public int CountryId { get; set; }

        // Date and time the city record was created. Stored in UTC.
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }

        // Date and time the city record was last updated. Stored in UTC.
        public DateTime LastUpdate { get; set; }
        public string LastUpdateBy { get; set; }

    }
}
