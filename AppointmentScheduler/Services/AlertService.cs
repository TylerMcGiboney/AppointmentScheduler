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
    public class AlertService
    {
        public List<Appointment> GetAppointmentWithin15Minutes()
        {
            AppointmentRepository appointmentRepository = new AppointmentRepository();
            List<Appointment> appointments = appointmentRepository.GetAppointmentsByUserId(App.CurrentUser.UserId);

            DateTime now = DateTime.UtcNow;
            DateTime endTime = now.AddMinutes(15);

            List<Appointment> upcomingAppointments = new List<Appointment>();

            foreach(var appointment in appointments)
            {
                if (appointment.Start >= now && appointment.Start <= endTime)
                {
                    upcomingAppointments.Add(appointment);
                }
            }
            return upcomingAppointments;
        }
    }
}
