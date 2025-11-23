using System.Collections.ObjectModel;
using AppointmentScheduler.Models;
using AppointmentScheduler.Repositories;
using AppointmentScheduler.Services;

namespace AppointmentScheduler.ViewModels
{
    /// <summary>
    /// Viewmodel for dsisplaying a list of customers in the UI.
    /// </summary>
    public class CustomersViewModel
    {
        public ObservableCollection<Customer> CustomerList { get; }

        public CustomersViewModel()
        {
            var repo = new CustomerRepository();
            var customers = repo.GetCustomers();

            var loc = new LocalizationService();

            CustomerList = new ObservableCollection<Customer>();

            foreach (var c in customers)
            {
                // If CreateDate/LastUpdate are stored as UTC, convert for display
                c.CreateDate = loc.ConvertUtcToLocal(c.CreateDate);
                c.LastUpdate = loc.ConvertUtcToLocal(c.LastUpdate);
                CustomerList.Add(c);
            }
        }
    }
}
