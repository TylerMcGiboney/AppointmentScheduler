using System;
using System.Collections.Generic;
using System.Linq;
using AppointmentScheduler.Models;
using AppointmentScheduler.Repositories;

namespace AppointmentScheduler.Services
{
    /// <summary>
    /// Provides reporting operations for appointments, including:
    /// - Appointment counts by type and month
    /// - Appointment schedules by user
    /// - Appointment counts by customer
    /// </summary>
    public class ReportService
    {
        /// <summary>
        /// Builds a report of appointment counts grouped by month and type.
        /// </summary>
        /// <returns>
        /// A list of <see cref="AppointmentTypeReport"/> entries,
        /// ordered by month and then by type.
        /// </returns>
        public List<AppointmentTypeReport> AppointmentTypesByMonth()
        {
            AppointmentRepository appointmentRepository = new AppointmentRepository();

            // Load all appointments
            List<Appointment> appointments = appointmentRepository.GetAppointments();

            // Build a list of AppointmentTypeReport objects that show, for each month, how many appointments there were for each appointment type.
            var reportData = appointments

                // 1) Group all appointments by YEAR and MONTH of their Start timestamp.
                .GroupBy(a => new { a.Start.Year, a.Start.Month })

                // 2) For each month group, further group by appointment Type.
                //    We use SelectMany to flatten the results into a single sequence of AppointmentTypeReport rows.
                .SelectMany(monthGroup => monthGroup

                    // Inside a single month (e.g., January 2025), group by appointment Type.
                    // Example:
                    //   - "Planning"  → all planning appointments in January 2025
                    //   - "Follow-up" → all follow-up appointments in January 2025
                    .GroupBy(a => a.Type)

                    // 3) For each (Month, Type) combination, project into an AppointmentTypeReport.
                    .Select(typeGroup => new AppointmentTypeReport
                    {
                        // Month label shown as a friendly string "MMMM yyyy"
                        // We reconstruct a DateTime from the year/month keys
                        Month = new DateTime(monthGroup.Key.Year, monthGroup.Key.Month, 1)
                                    .ToString("MMMM yyyy"),

                        // The appointment type for this group (e.g., "Planning", "Consultation").
                        Type = typeGroup.Key,

                        // Number of appointments that fall into this (Month, Type) group.
                        Count = typeGroup.Count()
                    })
                )

                //Order the final flat list by the actual month value (chronologically), then alphabetically by Type within each month.
                .OrderBy(r => DateTime.ParseExact(r.Month, "MMMM yyyy", null))
                .ThenBy(r => r.Type)

                //Materialize the sequence into a List<AppointmentTypeReport>.
                .ToList();

            // Return the fully built, sorted report.
            return reportData;
        }

        /// <summary>
        /// Builds a schedule-style report of appointments grouped by user,
        /// including local start/end times and customer names.
        /// </summary>
        /// <returns>
        /// A list of <see cref="UserAppointmentReport"/> entries,
        /// ordered by user name and start time.
        /// </returns>
        public List<UserAppointmentReport> AppointmentsByUser()
        {
            AppointmentRepository appointmentRepository = new AppointmentRepository();

            // Service for converting times from UTC to local.
            LocalizationService localizationService = new LocalizationService();

            // Load all appointments.
            List<Appointment> appointments = appointmentRepository.GetAppointments();

            // Repositories to resolve customer and user names from their IDs.
            CustomerRepository customerRepository = new CustomerRepository();
            UserRepository userRepository = new UserRepository();

            // Build a "schedule" view per user:
            // each row represents one appointment, enriched with:
            // - user name
            // - customer name
            // - local start/end time
            // - appointment type
            List<UserAppointmentReport> schedule = appointments
                .Select(a => new UserAppointmentReport
                {
                    // Look up the user name associated with the appointment's UserId.
                    UserName = userRepository.GetUserNameByUserId(a.UserId),

                    // Unique identifier of the appointment itself.
                    AppointmentId = a.AppointmentId,

                    // Look up the customer name associated with the appointment's CustomerId.
                    CustomerName = customerRepository.GetCustomerNameById(a.CustomerId),

                    // Convert the appointment Start from UTC to the user's local time zone.
                    StartLocal = localizationService.ConvertUtcToLocal(a.Start),

                    // Same conversion for the End time.
                    EndLocal = localizationService.ConvertUtcToLocal(a.End),

                    // Category/type of the appointment (e.g., "Planning", "Presentation").
                    Type = a.Type
                })

                // Order the final report alphabetically by user name,
                // then chronologically by local start time within each user.
                .OrderBy(r => r.UserName)
                .ThenBy(r => r.StartLocal)
                .ToList();

            return schedule;
        }

        /// <summary>
        /// Builds a report of how many appointments each customer has.
        /// </summary>
        /// <returns>
        /// A list of <see cref="CustomerAppointmentReport"/> entries,
        /// ordered by appointment count (descending) and then by customer name.
        /// </returns>
        public List<CustomerAppointmentReport> AppointmentsByCustomer()
        {
            CustomerRepository customerRepository = new CustomerRepository();

            // Load all customers.
            List<Customer> customers = customerRepository.GetCustomers();

            // Load all appointments.
            AppointmentRepository appointmentRepository = new AppointmentRepository();
            List<Appointment> appointments = appointmentRepository.GetAppointments();

            // For each customer, count how many appointments reference that customer.
            var reportData = customers
                .Select(c => new CustomerAppointmentReport
                {
                    // The customer's display name.
                    CustomerName = c.CustomerName,

                    // Count of appointments where the appointment's CustomerId matches this customer.
                    AppointmentCount = appointments.Count(a => a.CustomerId == c.CustomerId)
                })

                // Order first by the number of appointments (descending),
                // so the customers with the most appointments appear at the top.
                // Then order by customer name alphabetically as a tiebreaker.
                .OrderByDescending(r => r.AppointmentCount)
                .ThenBy(r => r.CustomerName)
                .ToList();

            return reportData;
        }
    }

    /// <summary>
    /// Represents the number of appointments for a given type within a given month.
    /// </summary>
    public class AppointmentTypeReport
    {
        // Month label formatted as "MMMM yyyy" (e.g., "January 2025").
        public string Month { get; set; }

        // Appointment type (e.g., "Planning", "Consultation").
        public string Type { get; set; }

        // Count of appointments for the given month and type.
        public int Count { get; set; }
    }

    /// <summary>
    /// Represents a user’s appointment entry for scheduling/reporting views.
    /// </summary>
    public class UserAppointmentReport
    {
        // Name of the user who owns the appointment.
        public string UserName { get; set; }

        // Unique ID of the appointment.
        public int AppointmentId { get; set; }

        // Name of the customer associated with the appointment.
        public string CustomerName { get; set; }

        // Appointment start time in the user's local time zone.
        public DateTime StartLocal { get; set; }

        // Appointment end time in the user's local time zone.
        public DateTime EndLocal { get; set; }

        // Type or category of the appointment.
        public string Type { get; set; }
    }

    /// <summary>
    /// Represents the number of appointments associated with a given customer.
    /// </summary>
    public class CustomerAppointmentReport
    {
        // Customer's name.
        public string CustomerName { get; set; }

        // Total number of appointments for that customer.
        public int AppointmentCount { get; set; }
    }
}
