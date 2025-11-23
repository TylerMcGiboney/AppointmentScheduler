using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Models
{
    /// <summary>
    /// Represents a user in the scheduling system.
    /// </summary>
    public class User
    {
        // Primary key of the user record.
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }

        // Date and time the user record was created. Stored in UTC.
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }

        // Date and time the user record was last updated. Stored in UTC.
        public DateTime LastUpdate { get; set; }
        public string LastUpdateBy { get; set; }

    }
}
