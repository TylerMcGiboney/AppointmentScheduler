using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentScheduler.Models;

namespace AppointmentScheduler.Repositories
{
    public class CustomerRepository
    {
        public bool AddCustomer(Customer customer)
        {
            //Get a connection to the database using DataBase Service
            //Creat SQL query that sets customer attributes to the corresponding database properties.
            //Insert customer into database, need to implement "using" to close database connection
            //wrap it in a try catch block, somehow create accurate catches to support sql exceptions, need to learn this
            //if inserted return true, else return false.
        }
    }
}
