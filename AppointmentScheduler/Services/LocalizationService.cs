using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace AppointmentScheduler.Services
{
    public class LocalizationService
    {
      
        public static List<string> SupportedLanguages()
        {
            List<string> supportedLanguages = new List<string>();
            supportedLanguages.Add("en-US");
            supportedLanguages.Add("es-ES");
            return supportedLanguages;
        }

     
        public string CurrentLanguageCode
        {
            get { return Thread.CurrentThread.CurrentUICulture.Name; }
        }

 
        public void ChangeLanguage(string languageCode)
        {
            List<string> supported = SupportedLanguages();
            if (!supported.Contains(languageCode))
            {
                throw new NotSupportedException(
                    string.Format("The language code '{0}' is not supported.", languageCode));
            }

            CultureInfo culture = new CultureInfo(languageCode);

            // Used for resource lookups (Strings.resx, Strings.es.resx)
            Thread.CurrentThread.CurrentUICulture = culture;

            // Optional, but useful for date/number formatting
            Thread.CurrentThread.CurrentCulture = culture;
        }

      
        public string GetLocalTimeZoneDisplayName()
        {
            return TimeZoneInfo.Local.DisplayName;
        }

        public DateTime ConvertUtcToLocal(DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Unspecified)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }

            return utcDateTime.ToLocalTime();
        }

    
        public DateTime ConvertLocalToUtc(DateTime localDateTime)
        {
            if (localDateTime.Kind == DateTimeKind.Unspecified)
            {
                localDateTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Local);
            }

            return localDateTime.ToUniversalTime();
        }

        public DateTime ConvertLocalToEst(DateTime localDateTime)
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;
            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTime(localDateTime, localZone, estZone);
        }

        public DateTime ConvertUtcToEst(DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Unspecified)
            {
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
            }

            TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, estZone);
        }
    }
}
