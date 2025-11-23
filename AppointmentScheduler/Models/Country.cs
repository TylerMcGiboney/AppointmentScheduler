using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Models
{
    /// <summary>
    /// Represents a country in the system.
    /// </summary>
    public class Country
    {
        // Primary key of the country record.
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        // Date and time the country record was created. Stored in UTC.
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }

        // Date and time the country record was last updated. Stored in UTC.
        public DateTime LastUpdate { get; set; }
        public string LastUpdateBy { get; set; }
    }
}
