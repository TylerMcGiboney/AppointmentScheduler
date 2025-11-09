using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Models
{
    public class Appointment
    {
        int AppointmentId { get; set; }
        int CustomerId { get; set; }
        int UserId { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Location { get; set; }
        string Contact { get; set; }
        string Type { get; set; }
        string Url { get; set; }
        DateTime Start { get; set; }
        DateTime End { get; set; }
        DateTime CreateDate { get; set; }
        string CreatedBy { get; set; }
        DateTime LastUpdate { get; set; }
        string LastUpdateBy { get; set; }

    }
}
