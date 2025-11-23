using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using AppointmentScheduler.Commands;
using AppointmentScheduler.Models;
using AppointmentScheduler.Repositories;
using AppointmentScheduler.Services;

namespace AppointmentScheduler.ViewModels
{
    /// <summary>
    /// ViewModel for the dashboard view.
    /// </summary>
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly AppointmentRepository _appointmentRepo = new AppointmentRepository();
        private readonly CustomerRepository _customerRepo = new CustomerRepository();
        private readonly LocalizationService _localizationService = new LocalizationService();
        private readonly ReportService _reportService = new ReportService();

        // --- Header text ---
        public string WelcomeText => "Welcome, " + App.CurrentUser.UserName;
        public string TodayText => DateTime.Now.ToString("dddd, MMM d");
        public string TimeZoneText => "Time zone: " + TimeZoneInfo.Local.DisplayName;

        // --- Upcoming appointment card ---
        private string _upcomingAppointmentSummary;
        public string UpcomingAppointmentSummary
        {
            get => _upcomingAppointmentSummary;
            set { _upcomingAppointmentSummary = value; OnPropertyChanged(); }
        }

        private string _upcomingAppointmentDetails;
        public string UpcomingAppointmentDetails
        {
            get => _upcomingAppointmentDetails;
            set { _upcomingAppointmentDetails = value; OnPropertyChanged(); }
        }

        private string _upcomingAppointmentSecondary;
        public string UpcomingAppointmentSecondary
        {
            get => _upcomingAppointmentSecondary;
            set { _upcomingAppointmentSecondary = value; OnPropertyChanged(); }
        }

        private string _upcomingCountdownText;
        public string UpcomingCountdownText
        {
            get => _upcomingCountdownText;
            set { _upcomingCountdownText = value; OnPropertyChanged(); }
        }

        // --- Quick stats ---
        private int _todayAppointmentsCount;
        public int TodayAppointmentsCount
        {
            get => _todayAppointmentsCount;
            set { _todayAppointmentsCount = value; OnPropertyChanged(); }
        }

        private int _weekAppointmentsCount;
        public int WeekAppointmentsCount
        {
            get => _weekAppointmentsCount;
            set { _weekAppointmentsCount = value; OnPropertyChanged(); }
        }

        private int _totalCustomersCount;
        public int TotalCustomersCount
        {
            get => _totalCustomersCount;
            set { _totalCustomersCount = value; OnPropertyChanged(); }
        }

        // --- Reports: dynamic grid ---

        /// <summary>
        /// Items shown in the dashboard's report grid.
        /// This is a generic collection so the grid can bind to different report types.
        /// </summary>
        private IEnumerable<object> _currentReportItems;
        public IEnumerable<object> CurrentReportItems
        {
            get => _currentReportItems;
            set { _currentReportItems = value; OnPropertyChanged(); }
        }

        private string _currentReportTitle = "No report selected";
        public string CurrentReportTitle
        {
            get => _currentReportTitle;
            set { _currentReportTitle = value; OnPropertyChanged(); }
        }

        private string _currentReportSubtitle = "Choose a report on the left to see its data here.";
        public string CurrentReportSubtitle
        {
            get => _currentReportSubtitle;
            set { _currentReportSubtitle = value; OnPropertyChanged(); }
        }

        private string _currentReportHint = "Reports are based on all appointments in the system.";
        public string CurrentReportHint
        {
            get => _currentReportHint;
            set { _currentReportHint = value; OnPropertyChanged(); }
        }

        // --- Commands ---
        public ICommand RefreshCommand { get; }
        public ICommand TypesByMonthReportCommand { get; }
        public ICommand ConsultantScheduleReportCommand { get; }
        public ICommand CustomerAppointmentsReportCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// Constructs the DashboardViewModel and initializes commands and dashboard data.
        /// </summary>
        public DashboardViewModel()
        {
            RefreshCommand = new RelayCommand(Refresh);
            TypesByMonthReportCommand = new RelayCommand(TypesByMonthReport);
            ConsultantScheduleReportCommand = new RelayCommand(ConsultantScheduleReport);
            CustomerAppointmentsReportCommand = new RelayCommand(CustomerAppointmentsReport);

            LoadDashboard();
        }

        /// <summary>
        /// Loads all dashboard data: quick stats and upcoming appointment card.
        /// </summary>
        private void LoadDashboard()
        {
            LoadStats();
            LoadUpcomingAppointment();
        }


        /// <summary>
        /// Calculates and populates quick stats:
        /// - TodayAppointmentsCount: appointments today (local date).
        /// - WeekAppointmentsCount: appointments in the next 7 days.
        /// - TotalCustomersCount: count of active customers.
        /// </summary>
        private void LoadStats()
        {
            var customers = _customerRepo.GetCustomers();
            TotalCustomersCount = customers.Count(c => c.IsActive);

            int userId = App.CurrentUser.UserId;
            var userAppointments = _appointmentRepo.GetAppointmentsByUserId(userId);

            DateTime nowLocal = DateTime.Now;
            DateTime today = nowLocal.Date;
            DateTime tomorrow = today.AddDays(1);
            DateTime weekEnd = today.AddDays(7);

            int todayCount = 0;
            int weekCount = 0;

            foreach (var appt in userAppointments)
            {
                DateTime localStart = _localizationService.ConvertUtcToLocal(appt.Start);

                if (localStart >= today && localStart < tomorrow)
                    todayCount++;

                if (localStart >= today && localStart < weekEnd)
                    weekCount++;
            }

            TodayAppointmentsCount = todayCount;
            WeekAppointmentsCount = weekCount;
        }

        /// <summary>
        /// Loads the upcoming appointment within the next 15 minutes
        /// </summary>
        private void LoadUpcomingAppointment()
        {
            int userId = App.CurrentUser.UserId;
            var userAppointments = _appointmentRepo.GetAppointmentsByUserId(userId);

            DateTime nowLocal = DateTime.Now;
            DateTime windowEndLocal = nowLocal.AddMinutes(15);

            var upcoming = userAppointments
                .Select(a => new
                {
                    Appointment = a,
                    LocalStart = _localizationService.ConvertUtcToLocal(a.Start),
                    LocalEnd = _localizationService.ConvertUtcToLocal(a.End)
                })
                .Where(x => x.LocalStart >= nowLocal && x.LocalStart <= windowEndLocal)
                .OrderBy(x => x.LocalStart)
                .FirstOrDefault();

            if (upcoming == null)
            {
                UpcomingAppointmentSummary = "No upcoming appointments";
                UpcomingAppointmentDetails = "You do not have any appointments in the next 15 minutes.";
                UpcomingAppointmentSecondary = "Use the calendar and appointments pages to plan your day.";
                UpcomingCountdownText = "--";
            }
            else
            {
                var appt = upcoming.Appointment;
                DateTime localStart = upcoming.LocalStart;
                DateTime localEnd = upcoming.LocalEnd;

                string title = string.IsNullOrWhiteSpace(appt.Title) ? "Appointment" : appt.Title;

                UpcomingAppointmentSummary =
                    $"{title} at {localStart:t}";

                UpcomingAppointmentDetails =
                    $"Starts at {localStart:f} and ends at {localEnd:t}.";

                UpcomingAppointmentSecondary =
                    "This card only shows appointments starting within the next 15 minutes.";

                double minutes = (localStart - nowLocal).TotalMinutes;
                int rounded = (int)Math.Round(minutes);
                if (rounded < 0) rounded = 0;

                UpcomingCountdownText = rounded.ToString();
            }
        }

        /// <summary>
        /// Refreshes all dashboard data.
        /// </summary>
        private void Refresh(object obj)
        {
            LoadDashboard();
        }


        /// <summary>
        /// Loads the "Appointment Types by Month" report into the dashboard's report grid.
        /// </summary>
        private void TypesByMonthReport(object obj)
        {
            var items = _reportService.AppointmentTypesByMonth();
            CurrentReportItems = items.Cast<object>().ToList();
            CurrentReportTitle = "Appointment Types by Month";
            CurrentReportSubtitle = "Shows the number of appointments per type for each month.";
            CurrentReportHint = "Data is grouped by the appointment's start date (UTC).";
        }

        /// <summary>
        /// Loads the "Consultant Schedule" report into the dashboard's report grid.
        /// </summary>
        private void ConsultantScheduleReport(object obj)
        {
            var items = _reportService.AppointmentsByUser();
            CurrentReportItems = items.Cast<object>().ToList();
            CurrentReportTitle = "Consultant Schedule (Appointments by User)";
            CurrentReportSubtitle = "Shows each user's appointments with local start/end times.";
            CurrentReportHint = "Sorted by user and appointment start time.";
        }

        /// <summary>
        /// Loads the "Appointments by Customer" report into the dashboard's report grid.
        /// </summary>
        private void CustomerAppointmentsReport(object obj)
        {
            var items = _reportService.AppointmentsByCustomer();
            CurrentReportItems = items.Cast<object>().ToList();
            CurrentReportTitle = "Appointments by Customer";
            CurrentReportSubtitle = "Shows how many appointments each customer has.";
            CurrentReportHint = "Ordered by highest appointment count, then customer name.";
        }
    }
}
