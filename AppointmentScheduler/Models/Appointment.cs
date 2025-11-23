using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Models
{
    /// <summary>
    /// Represents an appointment in the scheduling system.
    /// </summary>
    public class Appointment
    {
        // Backing field for Url property to ensure it is never null.
        private string _url = " ";

        // Primary key of the appointment record.
        public int AppointmentId { get; set; }

        //Foreign key referencing the Customer record.
        public int CustomerId { get; set; }

        //Foreign key referencing the User record.
        public int UserId { get; set; }


        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Contact { get; set; }
        public string Type { get; set; }

        // URL associated with the appointment. Never null; defaults to a single space if not set.
        public string Url 
        {
            get => _url ?? " ";
            set => _url = value ?? " ";
        }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        // Date and time the appointment record was created. Stored in UTC.
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }

        // Date and time the appointment record was last updated. Stored in UTC.
        public DateTime LastUpdate { get; set; }
        public string LastUpdateBy { get; set; }

        public string CustomerName { get; set; }

        public Appointment()
        {
            Title = string.Empty;
            Description = string.Empty;
            Location = string.Empty;
            Contact = string.Empty;
            Type = string.Empty;
            Url = " "; // goes through property and normalizes
            CustomerName = string.Empty;
            CreatedBy = string.Empty;
            LastUpdateBy = string.Empty;
        }
    }
}
