using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentScheduler.Models;
using AppointmentScheduler.Services;
using AppointmentScheduler.Repositories;

namespace AppointmentScheduler.Services
{
    public class ReportService
    {
        public List<AppointmentTypeReport> appointmentTypesByMonth()
        {
            AppointmentRepository appointmentRepositry = new AppointmentRepository();
            List<Appointment> appointments = appointmentRepositry.GetAppointments();

            var reportData = appointments
                .GroupBy(a => new { a.Start.Year, a.Start.Month})
                .SelectMany(monthGroup => monthGroup
                    .GroupBy(a => a.Type)
                    .Select(typeGroup => new AppointmentTypeReport
                    {
                        Month = new DateTime(monthGroup.Key.Year, monthGroup.Key.Month, 1)
                                    .ToString("MMMM yyyy"),
                        Type = typeGroup.Key,
                        Count = typeGroup.Count()
                    })
                )
                .OrderBy(r => DateTime.ParseExact(r.Month, "MMMM yyyy", null))
                .ThenBy(r => r.Type)
                .ToList();
            return reportData;
        }

        public List<UserAppointmentReport> appointmentsByUser()
        {
            AppointmentRepository appointmentRepository = new AppointmentRepository();
            LocalizationService localizationService = new LocalizationService();
            List<Appointment> appointments = appointmentRepository.GetAppointments();

            CustomerRepository customerRepository = new CustomerRepository();
            UserRepository userRepository = new UserRepository();

            List<UserAppointmentReport> schedule = appointments
                .Select(a => new UserAppointmentReport
                {
                    UserName = userRepository.GetUserNameByUserId(a.UserId),
                    AppointmentId = a.AppointmentId,
                    CustomerName = customerRepository.GetCustomerNameById(a.CustomerId),
                    StartLocal = localizationService.ConvertUtcToLocal(a.Start),
                    EndLocal = localizationService.ConvertUtcToLocal(a.End),
                    Type = a.Type
                })
                .OrderBy(r => r.UserName)
                .ThenBy(r => r.StartLocal)
                .ToList();
            return schedule;
        }

        public List<CustomerAppointmentReport> AppointmentsByCustomer()
        {
            CustomerRepository customerRepository = new CustomerRepository();
            List<Customer> customers = customerRepository.GetCustomers();
            AppointmentRepository appointmentRepository = new AppointmentRepository();
            List<Appointment> appointments = appointmentRepository.GetAppointments();
            var reportData = customers
                .Select(c => new CustomerAppointmentReport
                {
                    CustomerName = c.CustomerName,
                    AppointmentCount = appointments.Count(a => a.CustomerId == c.CustomerId)
                })
                .OrderByDescending(r => r.AppointmentCount)
                .ThenBy(r => r.CustomerName)
                .ToList();
            return reportData;
        }


    }

    public class AppointmentTypeReport
    {
        public string Month { get; set; }
        public string Type { get; set; }
        public int Count { get; set; }
    }

    public class UserAppointmentReport
    {
        public string UserName { get; set; }
        public int AppointmentId { get; set; }
        public string CustomerName { get; set; }
        public DateTime StartLocal { get; set; }
        public DateTime EndLocal { get; set; }
        public string Type { get; set; }
    }

    public class CustomerAppointmentReport
    {
        public string CustomerName { get; set; }
        public int AppointmentCount { get; set; }
    }
}
