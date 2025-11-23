using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Models
{
    /// <summary>
    /// Represents a customer in the scheduling system.
    /// </summary>
    public class Customer
    {
        // Backing field for Address2 to ensure it's never null
        private string _address2 = string.Empty;

        // Primary key of the customer record.
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        // Foreign key referencing the Address record.
        public int AddressId { get; set; }
        public bool IsActive { get; set; }

        // Date and time the customer record was created. Stored in UTC.
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }

        // Date and time the customer record was last updated. Stored in UTC.
        public DateTime LastUpdate { get; set; }
        public string LastUpdateBy { get; set; }

        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        // Optional Address2 property ensured it's never null to avoid null reference issues.
        public string Address2
        {
            get { return _address2 ?? " "; }
            set { _address2 = value ?? " "; }
        }

        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        // Constructor to initialize default values
        public Customer()
        {
            Address2 = string.Empty;
            CustomerName = string.Empty;
            PhoneNumber = string.Empty;
            Address = string.Empty;
            City = string.Empty;
            PostalCode = string.Empty;
            Country = string.Empty;
            CreatedBy = string.Empty;
            LastUpdateBy = string.Empty;
        }
    }
}