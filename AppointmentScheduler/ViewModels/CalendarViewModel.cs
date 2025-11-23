using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AppointmentScheduler.Models;
using AppointmentScheduler.Repositories;
using AppointmentScheduler.Services;

namespace AppointmentScheduler.ViewModels
{
    /// <summary>
    /// ViewModel for the calendar view.
    /// 
    /// Responsibilities:
    /// - Track the currently selected calendar date.
    /// - Load all appointments for the current user that fall on that date.
    /// - Expose those appointments as a collection of <see cref="AppointmentItemViewModel"/>
    ///   with times converted from UTC to local.
    /// </summary>
    public class CalendarViewModel : INotifyPropertyChanged
    {
        
        private readonly AppointmentRepository _appointmentRepository;
        private readonly LocalizationService _localizationService;

        /// <summary>
        /// Appointments that occur on the currently selected date (local time),
        /// wrapped in <see cref="AppointmentItemViewModel"/> instances for display.
        /// </summary>
        public ObservableCollection<AppointmentItemViewModel> AppointmentsForSelectedDate { get; }
            = new ObservableCollection<AppointmentItemViewModel>();

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                    LoadAppointmentsForSelectedDate();
                }
            }
        }

        public CalendarViewModel()
        {
            _appointmentRepository = new AppointmentRepository();
            _localizationService = new LocalizationService();
            SelectedDate = DateTime.Today;
            
        }

        /// <summary>
        /// Loads all appointments for the currently logged-in user that occur
        /// on <see cref="SelectedDate"/> (in local time).
        /// 
        /// Steps:
        /// - Clear the existing appointments collection.
        /// - Get all appointments for the current user from the repository (UTC times).
        /// - Convert each appointment's start time from UTC to local.
        /// - Filter by date (local date == SelectedDate).
        /// - Wrap each filtered appointment in an AppointmentItemViewModel and add it
        ///   to <see cref="AppointmentsForSelectedDate"/>.
        /// </summary>
        public void LoadAppointmentsForSelectedDate()
        {
            AppointmentsForSelectedDate.Clear();

            var user = App.CurrentUser;
            if (user == null) return;

            var appts = _appointmentRepository.GetAppointmentsByUserId(user.UserId);

            var targetDate = SelectedDate.Date;

            var filtered = appts.Where(a =>
            {
                var localStartDate = _localizationService.ConvertUtcToLocal(a.Start).Date;
                return localStartDate == targetDate;
            });

            foreach (var appt in filtered)
            {
                var vmItem = new AppointmentItemViewModel(appt, _localizationService);
                AppointmentsForSelectedDate.Add(vmItem);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

