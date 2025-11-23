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
    /// <summary>
    /// Service for handling alerts related to appointments.
    /// </summary>
    public class AlertService
    {
        /// <summary>
        /// Gets a list of appointments scheduled to start within the next 15 minutes for the current user.
        /// </summary>
        /// <returns>A list of appointments within 15 minutes</returns>
        public List<Appointment> GetAppointmentWithin15Minutes()
        {
            AppointmentRepository appointmentRepository = new AppointmentRepository();

            // Get all appointments for the current user
            List<Appointment> appointments = appointmentRepository.GetAppointmentsByUserId(App.CurrentUser.UserId);

            // Get current time and time 15 minutes from now
            DateTime now = DateTime.UtcNow;
            DateTime endTime = now.AddMinutes(15);

            List<Appointment> upcomingAppointments = new List<Appointment>();

            // Check each appointment to see if it starts within the next 15 minutes
            foreach (var appointment in appointments)
            {
                if (appointment.Start >= now && appointment.Start <= endTime)
                {
                    // Add to the list of upcoming appointments
                    upcomingAppointments.Add(appointment);
                }
            }
            // Return the list of upcoming appointments
            return upcomingAppointments;
        }
    }
}
