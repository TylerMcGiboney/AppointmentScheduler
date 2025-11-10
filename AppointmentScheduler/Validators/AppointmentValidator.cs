using System;
using System.Collections.Generic;
using AppointmentScheduler.Models;
using AppointmentScheduler.Repositories;
using AppointmentScheduler.Services;

namespace AppointmentScheduler.Validators
{
    public class AppointmentValidator
    {
        public static bool IsEndTimeAfterStartTime(DateTime start, DateTime end)
        {
            return end > start;
        }

        public static bool StartAndEndTimeOnSameDay(DateTime start, DateTime end)
        {
            return start.Date == end.Date;
        }

        public static bool AppointmentInPast(DateTime start, DateTime end)
        {
            DateTime now = DateTime.Now;
            return end <= now;
        }

        public static bool AppointmentInBusinessHours(DateTime start, DateTime end)
        {
            int businessHourStart = 9;   // 9:00
            int businessHourEnd = 17;    // 17:00 (5 PM)

            LocalizationService localizationService = new LocalizationService();

            // Convert from local time into EST for business-hour checking
            DateTime appointmentStartEst = localizationService.ConvertLocalToEst(start);
            DateTime appointmentEndEst = localizationService.ConvertLocalToEst(end);

            // No weekends
            if (appointmentStartEst.DayOfWeek == DayOfWeek.Saturday ||
                appointmentStartEst.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            // Must start at or after opening
            if (appointmentStartEst.Hour < businessHourStart)
            {
                return false;
            }

            // End must not be after closing (17:00). Allow exactly 17:00, but not later.
            if (appointmentEndEst.Hour > businessHourEnd ||
               (appointmentEndEst.Hour == businessHourEnd && appointmentEndEst.Minute > 0))
            {
                return false;
            }

            return true;
        }


        /// Returns true if the current user has any overlapping appointment
        /// with the given start and end times.
        /// Assumes start/end are LOCAL times from the UI, and existing appointments
        /// in the database are stored in UTC.
        public static bool UserOverlappingAppointment(DateTime start, DateTime end)
        {
            AppointmentRepository appointmentRepository = new AppointmentRepository();
            LocalizationService localizationService = new LocalizationService();

            // Adjust this property name if your User model is different.
            List<Appointment> existingAppointments =
                appointmentRepository.GetAppointmentsByUserId(App.CurrentUser.Id);

            foreach (var appointment in existingAppointments)
            {
                // Convert stored UTC times to local before comparison
                DateTime existingStartLocal = localizationService.ConvertUtcToLocal(appointment.Start);
                DateTime existingEndLocal = localizationService.ConvertUtcToLocal(appointment.End);

                // Overlap rule: (newStart < existingEnd) && (newEnd > existingStart)
                if (start < existingEndLocal && end > existingStartLocal)
                {
                    return true;
                }
            }

            return false;
        }

        /// Returns true if the given customer has any overlapping appointment
        /// with the given start and end times.
        /// Assumes start/end are LOCAL times from the UI, and existing appointments
        /// in the database are stored in UTC.
        public static bool CustomerOverlappingAppointment(int customerId, DateTime start, DateTime end)
        {
            AppointmentRepository appointmentRepository = new AppointmentRepository();
            LocalizationService localizationService = new LocalizationService();

            List<Appointment> existingAppointments =
                appointmentRepository.GetAppointmentsByCustomerId(customerId);

            foreach (var appointment in existingAppointments)
            {
                DateTime existingStartLocal = localizationService.ConvertUtcToLocal(appointment.Start);
                DateTime existingEndLocal = localizationService.ConvertUtcToLocal(appointment.End);

                if (start < existingEndLocal && end > existingStartLocal)
                {
                    return true;
                }
            }

            return false;
        }

        /// Runs all validation rules on the appointment using LOCAL times.
        /// Call this with local start/end from the UI.
        /// If this returns true, you can then convert start/end to UTC and save to the database.
        public static bool IsValidAppointment(int customerId, DateTime start, DateTime end)
        {
            if (!IsEndTimeAfterStartTime(start, end))
                return false;

            if (!StartAndEndTimeOnSameDay(start, end))
                return false;

            if (AppointmentInPast(start, end))
                return false;

            if (!AppointmentInBusinessHours(start, end))
                return false;

            if (UserOverlappingAppointment(start, end))
                return false;

            if (CustomerOverlappingAppointment(customerId, start, end))
                return false;

            return true;
        }
    }
}
