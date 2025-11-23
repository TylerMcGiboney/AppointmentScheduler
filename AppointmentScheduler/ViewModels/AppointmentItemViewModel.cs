using System;
using AppointmentScheduler.Models;
using AppointmentScheduler.Repositories;
using AppointmentScheduler.Services;

namespace AppointmentScheduler.ViewModels
{
    /// <summary>
    /// ViewModel representing a single appointment row/item for display in the UI.
    /// Wraps an <see cref="Appointment"/> model and exposes
    /// display-ready properties (including local times and customer name).
    /// </summary>
    public class AppointmentItemViewModel
    {
        public int AppointmentId { get; }

        public int CustomerId { get; }
        public string CustomerName { get; }   

        public string Title { get; }
        public string Description { get; }
        public string Location { get; }
        public string Contact { get; }

        public DateTime StartLocal { get; }        
        public DateTime EndLocal { get; }          

        public DateTime CreateDate { get; }  
        public string CreatedBy { get; }
        public DateTime LastUpdate { get; }   
        public string LastUpdatedBy { get; }

        /// <summary>
        /// Creates a new view model for displaying an appointment in the UI.
        /// Assumes all DateTime values in <paramref name="model"/> are stored in UTC
        /// and converts them to local time for display.
        /// </summary>
        /// <param name="model">The underlying appointment model from the database.</param>
        /// <param name="localizationService">
        /// Service used to convert UTC timestamps to the user's local time zone.
        /// </param>
        public AppointmentItemViewModel(Appointment model, LocalizationService localizationService)
        {
            // services
            var customerRepo = new CustomerRepository();

            AppointmentId = model.AppointmentId;
            CustomerId = model.CustomerId;
            CustomerName = customerRepo.GetCustomerNameById(model.CustomerId);

            Title = model.Title;
            Description = model.Description;
            Location = model.Location;
            Contact = model.Contact;

            
            StartLocal = localizationService.ConvertUtcToLocal(model.Start);
            EndLocal = localizationService.ConvertUtcToLocal(model.End);

            CreateDate = localizationService.ConvertUtcToLocal(model.CreateDate);
            CreatedBy = model.CreatedBy;
            LastUpdate = localizationService.ConvertUtcToLocal(model.LastUpdate);
            LastUpdatedBy = model.LastUpdateBy;
        }
    }
}
