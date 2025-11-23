using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Models
{
    /// <summary>
    /// Represents a physical address associated with a customer.
    /// </summary>
    public class Address
    {
        //Primary key of the address record.
        public int AddressId { get; set; }

        public string Address1 { get; set; }

        //Optional second address line. The data can not be null in the database so it is initialized to an empty string.
        public string Address2 { get; set; } = string.Empty;

        //Foreign key referencing the City record.
        public int CityId { get;set; }
        public string Zip { get; set; }
        public string PhoneNumber { get;set; }

        //Date and time the address record was created. Stored in UTC.
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }

        //Date and time the address record was last updated. Stored in UTC.
        public DateTime LastUpdate { get; set; }
        public string LastUpdateBy { get;set; }
    }
}
