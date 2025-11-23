using System;
using AppointmentScheduler.Repositories;
using AppointmentScheduler.Services;

namespace AppointmentScheduler.Validators
{
    /// <summary>
    /// Validates appointment scheduling based on business rules.
    /// </summary>
    public static class AppointmentValidator
    {
        /// <summary>
        /// Validates an appointment based on business rules.
        /// startLocal / endLocal are LOCAL times (user's machine timezone).
        /// DB stores UTC. Overlaps are checked in UTC.
        /// Business hours are 9-5 Eastern Time (handles DST automatically).
        /// </summary>
        public static bool IsValidAppointment(
            int userId,
            int customerId,
            DateTime startLocal,
            DateTime endLocal,
            int? appointmentId = null)
        {
            var locService = new LocalizationService();

            // Convert to UTC for certain validations
            DateTime startUtc = locService.ConvertLocalToUtc(startLocal);
            DateTime endUtc = locService.ConvertLocalToUtc(endLocal);
            DateTime nowUtc = DateTime.UtcNow;

            // 1. End must be after start
            if (endLocal <= startLocal)
                return false;

            // 2. Not in the past (compare in UTC to handle timezone correctly)
            if (startUtc < nowUtc)
                return false;

            // 3. Same local day (in user's timezone)
            if (startLocal.Date != endLocal.Date)
                return false;

            // 4. Within business hours (9-5 ET Monday-Friday)
            if (!IsWithinBusinessHoursEastern(startLocal, endLocal, locService))
                return false;

            // 5. No overlapping appointments for this USER (any customer)
            if (HasOverlappingAppointmentsForUser(userId, startUtc, endUtc, appointmentId))
                return false;

            // 6. No overlapping appointments for this CUSTOMER (any user)
            if (HasOverlappingAppointmentsForCustomer(customerId, startUtc, endUtc, appointmentId))
                return false;

            return true;
        }

        /// <summary>
        /// Checks if the appointment falls within business hours (9-5 ET, Monday-Friday).
        /// Properly handles DST by using "Eastern Standard Time" which Windows automatically
        /// adjusts to EDT when appropriate.
        /// </summary>
        private static bool IsWithinBusinessHoursEastern(
            DateTime startLocal,
            DateTime endLocal,
            LocalizationService locService)
        {
            // Convert local times to UTC first, then to Eastern Time
            DateTime startUtc = locService.ConvertLocalToUtc(startLocal);
            DateTime endUtc = locService.ConvertLocalToUtc(endLocal);

            // Now convert to Eastern Time (this handles DST automatically)
            DateTime startEastern = locService.ConvertUtcToEst(startUtc);
            DateTime endEastern = locService.ConvertUtcToEst(endUtc);

            // Check if appointment is on a weekend
            if (startEastern.DayOfWeek == DayOfWeek.Saturday ||
                startEastern.DayOfWeek == DayOfWeek.Sunday)
                return false;

            // Edge case: appointment might span to next day in Eastern time
            if (endEastern.DayOfWeek == DayOfWeek.Saturday ||
                endEastern.DayOfWeek == DayOfWeek.Sunday)
                return false;

            // Check if the appointment crosses midnight in Eastern time
            if (startEastern.Date != endEastern.Date)
                return false;

            // Build the business hours for that specific date in Eastern time
            var easternDate = startEastern.Date;
            DateTime businessOpenEastern = new DateTime(
                easternDate.Year, easternDate.Month, easternDate.Day,
                9, 0, 0, DateTimeKind.Unspecified);
            DateTime businessCloseEastern = new DateTime(
                easternDate.Year, easternDate.Month, easternDate.Day,
                17, 0, 0, DateTimeKind.Unspecified);

            // Check if appointment is within business hours
            if (startEastern.TimeOfDay < businessOpenEastern.TimeOfDay)
                return false;
            if (endEastern.TimeOfDay > businessCloseEastern.TimeOfDay)
                return false;

            return true;
        }

        /// <summary>
        /// Checks for overlapping appointments for a specific user.
        /// Uses UTC times for comparison since DB stores UTC.
        /// </summary>
        private static bool HasOverlappingAppointmentsForUser(
            int userId,
            DateTime candidateStartUtc,
            DateTime candidateEndUtc,
            int? currentAppointmentId)
        {
            var repo = new AppointmentRepository();
            var allForUser = repo.GetAppointmentsByUserId(userId);

            foreach (var appt in allForUser)
            {
                // Skip the appointment we are currently editing
                if (currentAppointmentId.HasValue &&
                    appt.AppointmentId == currentAppointmentId.Value)
                {
                    continue;
                }

                DateTime existingStartUtc = appt.Start; // Already UTC from DB
                DateTime existingEndUtc = appt.End;     // Already UTC from DB

                // Check for overlap: two intervals overlap if start1 < end2 AND start2 < end1
                bool overlaps =
                    candidateStartUtc < existingEndUtc &&
                    candidateEndUtc > existingStartUtc;

                if (overlaps)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks for overlapping appointments for a specific customer.
        /// Uses UTC times for comparison since DB stores UTC.
        /// </summary>
        private static bool HasOverlappingAppointmentsForCustomer(
            int customerId,
            DateTime candidateStartUtc,
            DateTime candidateEndUtc,
            int? currentAppointmentId)
        {
            var repo = new AppointmentRepository();
            var allForCustomer = repo.GetAppointmentsByCustomerId(customerId);

            foreach (var appt in allForCustomer)
            {
                // Skip the appointment we are currently editing
                if (currentAppointmentId.HasValue &&
                    appt.AppointmentId == currentAppointmentId.Value)
                {
                    continue;
                }

                DateTime existingStartUtc = appt.Start; // Already UTC from DB
                DateTime existingEndUtc = appt.End;     // Already UTC from DB

                // Check for overlap: two intervals overlap if start1 < end2 AND start2 < end1
                bool overlaps =
                    candidateStartUtc < existingEndUtc &&
                    candidateEndUtc > existingStartUtc;

                if (overlaps)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Alternative validation method that provides detailed error messages.
        /// Useful for giving users specific feedback about why validation failed.
        /// </summary>
        public static ValidationResult ValidateAppointmentWithDetails(
            int userId,
            int customerId,
            DateTime startLocal,
            DateTime endLocal,
            int? appointmentId = null)
        {
            var result = new ValidationResult { IsValid = true };
            var locService = new LocalizationService();

            DateTime startUtc = locService.ConvertLocalToUtc(startLocal);
            DateTime endUtc = locService.ConvertLocalToUtc(endLocal);
            DateTime nowUtc = DateTime.UtcNow;

            // 1. End must be after start
            if (endLocal <= startLocal)
            {
                result.IsValid = false;
                result.ErrorMessage = "Appointment end time must be after start time.";
                return result;
            }

            // 2. Not in the past
            if (startUtc < nowUtc)
            {
                result.IsValid = false;
                result.ErrorMessage = "Cannot schedule appointments in the past.";
                return result;
            }

            // 3. Same local day
            if (startLocal.Date != endLocal.Date)
            {
                result.IsValid = false;
                result.ErrorMessage = "Appointment must start and end on the same day.";
                return result;
            }

            // 4. Business hours check
            DateTime startEastern = locService.ConvertUtcToEst(startUtc);
            DateTime endEastern = locService.ConvertUtcToEst(endUtc);

            if (startEastern.DayOfWeek == DayOfWeek.Saturday ||
                startEastern.DayOfWeek == DayOfWeek.Sunday)
            {
                result.IsValid = false;
                result.ErrorMessage = "Appointments cannot be scheduled on weekends.";
                return result;
            }

            if (startEastern.Date != endEastern.Date)
            {
                result.IsValid = false;
                result.ErrorMessage = "Appointment crosses midnight in Eastern Time.";
                return result;
            }

            var businessOpen = new TimeSpan(9, 0, 0);
            var businessClose = new TimeSpan(17, 0, 0);

            if (startEastern.TimeOfDay < businessOpen || endEastern.TimeOfDay > businessClose)
            {
                result.IsValid = false;
                result.ErrorMessage = $"Appointments must be within business hours (9:00 AM - 5:00 PM Eastern Time). " +
                                    $"Your appointment would be {startEastern:h:mm tt} - {endEastern:h:mm tt} ET.";
                return result;
            }

            // 5. User overlap check
            if (HasOverlappingAppointmentsForUser(userId, startUtc, endUtc, appointmentId))
            {
                result.IsValid = false;
                result.ErrorMessage = "This time slot conflicts with another appointment for this user.";
                return result;
            }

            // 6. Customer overlap check
            if (HasOverlappingAppointmentsForCustomer(customerId, startUtc, endUtc, appointmentId))
            {
                result.IsValid = false;
                result.ErrorMessage = "This time slot conflicts with another appointment for this customer.";
                return result;
            }

            return result;
        }
    }

    /// <summary>
    /// Result object for validation with detailed error messages.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
}