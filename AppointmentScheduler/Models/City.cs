using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Models
{
    public class City
    {
        int CityId { get; set; }
        string CityName { get; set; }
        int CountryId { get; set; }
        DateTime CreateDate { get; set; }
        string CreatedBy { get; set; }
        DateTime LastUpdate { get; set; }
        string LastUpdateBy { get; set; }

    }
}
