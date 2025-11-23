using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace AppointmentScheduler.Services
{
    /// <summary>
    /// Provides localization and time zone utilities for the application.
    /// Includes: Language selection and Time zone conversionss.
    /// </summary>
    public class LocalizationService
    {

        // Returns a list of supported languages.
        public static List<string> SupportedLanguages()
        {
            List<string> supportedLanguages = new List<string>();
            supportedLanguages.Add("English");
            supportedLanguages.Add("Spanish");
            return supportedLanguages;
        }

        // Gets the current UI culture language code.
        public string CurrentLanguageCode
        {
            get { return Thread.CurrentThread.CurrentUICulture.Name; }
        }

        // Changes the application's language based on the provided language code.
        public void ChangeLanguage(string languageCode)
        {
            var culture = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
        }

        /// <summary>
        /// Sets the UI language based on the user's local time zone.
        /// Currently treats time zones containing "Mexico" or "Central America"
        /// as Spanish; all others default to English.
        /// </summary>
        public void SetLanguageBasedOnLocation()
        {
            TimeZoneInfo userTimeZone = TimeZoneInfo.Local;

            if (userTimeZone.Id.Contains("Mexico") || userTimeZone.Id.Contains("Central America"))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("es-ES");
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");

            }
        }

        // Gets the display name of the user's local time zone.
        public string GetLocalTimeZoneDisplayName()
        {
            TimeZoneInfo userTimeZone = TimeZoneInfo.Local;
            return userTimeZone.StandardName;
        }

        /// <summary>
        /// Converts a UTC <see cref="DateTime"/> value to local time.
        /// If the kind is unspecified, it is treated as UTC.
        /// </summary>
        public DateTime ConvertUtcToLocal(DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Unspecified)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }

            return utcDateTime.ToLocalTime();
        }

        /// <summary>
        /// Converts a local <see cref="DateTime"/> value to UTC.
        /// If the kind is unspecified, it is treated as local.
        /// </summary>
        public DateTime ConvertLocalToUtc(DateTime localDateTime)
        {
            if (localDateTime.Kind == DateTimeKind.Unspecified)
            {
                localDateTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Local);
            }

            return localDateTime.ToUniversalTime();
        }

        /// <summary>
        /// Converts a local <see cref="DateTime"/> value to Eastern Standard Time (EST).
        /// </summary>
        public DateTime ConvertLocalToEst(DateTime localDateTime)
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTime(localDateTime, localZone, estZone);
        }

        /// <summary>
        /// Converts a UTC <see cref="DateTime"/> value to Eastern Standard Time (EST).
        /// If the kind is unspecified, it is treated as UTC.
        /// </summary>
        public DateTime ConvertUtcToEst(DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Unspecified)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }

            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, estZone);
        }

        /// <summary>
        /// Calculates the local-time window that corresponds to business hours
        /// of 9:00–17:00 in Eastern time for the given local date.
        /// </summary>
        /// <param name="localDate">
        /// A date in the user's local time zone. The time component is ignored.
        /// </param>
        /// <returns>
        /// A tuple containing the local-time start and end values that map to
        /// 9:00 and 17:00 EST for that date.
        /// </returns>
        public (DateTime LocalStart, DateTime LocalEnd) GetLocalBusinessWindowForDate(DateTime localDate)
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

            var localNoon = new DateTime(localDate.Year, localDate.Month, localDate.Day, 12, 0, 0, DateTimeKind.Unspecified);
            var estNoon = TimeZoneInfo.ConvertTime(localNoon, localZone, estZone);
            var estDate = estNoon.Date;

            // 9:00 and 17:00 EST on that EST date
            var estStart = new DateTime(estDate.Year, estDate.Month, estDate.Day, 9, 0, 0, DateTimeKind.Unspecified);
            var estEnd = new DateTime(estDate.Year, estDate.Month, estDate.Day, 17, 0, 0, DateTimeKind.Unspecified);

            // Convert those to local time
            var localStart = TimeZoneInfo.ConvertTime(estStart, estZone, localZone);
            var localEnd = TimeZoneInfo.ConvertTime(estEnd, estZone, localZone);

            return (localStart, localEnd);
        }
    }
}
