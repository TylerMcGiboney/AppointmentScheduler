using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Models
{
    public class User
    {
        int Id { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        bool IsActive { get; set; }
        DateTime CreateDate { get; set; }
        string CreatedBy { get; set; }
        DateTime LastUpdate { get; set; }
        string LastUpdateBy { get; set; }

    }
}
