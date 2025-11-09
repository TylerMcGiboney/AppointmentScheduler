using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Models
{
    public class Customer
    {
        int CustomerId { get; set; }
        string CustomerName { get; set; }
        int AddressId { get; set; }
        bool IsActive { get; set; }
        DateTime CreateDate { get; set; }
        string CreatedBy { get; set; }
        DateTime LastUpdate { get; set; }
        string LastUpdateBy { get; set; }

    }
}
