using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Models
{
    public class Address
    {
        int AddressId { get; set; }
        string Address1 { get; set; }
        string Address2 { get;set; }
        int CityId { get;set; }
        string Zip { get; set; }
        string PhoneNumber { get;set; }
        DateTime CreateDate { get; set; }
        string CreatedBy { get; set; }
        DateTime LastUpdate { get; set; }
        string LastUpdateBy { get;set; }
    }
}
