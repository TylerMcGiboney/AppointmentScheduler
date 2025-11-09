using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Models
{
    public class Country
    {
        int CountryId { get; set; }
        string CountryName { get; set; }
        DateTime CreateDate { get; set; }
        string CreatedBy { get; set; }
        DateTime LastUpdate { get; set; }
        string LastUpdateBy { get; set; }
    }
}
