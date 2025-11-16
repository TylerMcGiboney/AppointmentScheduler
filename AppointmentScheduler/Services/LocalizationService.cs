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
            supportedLanguages.Add("English");
            supportedLanguages.Add("Spanish");
            return supportedLanguages;
        }

     
        public string CurrentLanguageCode
        {
            get { return Thread.CurrentThread.CurrentUICulture.Name; }
        }

 
        public void ChangeLanguage(string languageCode)
        {
            var culture = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
        }

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

      
        public string GetLocalTimeZoneDisplayName()
        {
            TimeZoneInfo userTimeZone = TimeZoneInfo.Local;
            return userTimeZone.StandardName;
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
