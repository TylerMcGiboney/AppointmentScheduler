using AppointmentScheduler.Models;
using AppointmentScheduler.Repositories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AppointmentScheduler.Services;

namespace AppointmentScheduler.ViewModels
{
    /// <summary>
    /// ViewModel responsible for loading and exposing the list of
    /// appointments belonging to the currently logged-in user.
    /// 
    /// Converts all UTC timestamps (stored in DB) into local time
    /// before presenting them to the UI. Wraps each appointment
    /// in an <see cref="AppointmentItemViewModel"/> so the UI always
    /// binds to display-ready values.
    /// </summary>
    public class AppointmentsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Appointment> _userAppointments;

        public ObservableCollection<Appointment> UserAppointments
        {
            get => _userAppointments;
            set { _userAppointments = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Loads all appointments for the current user.
        /// Appointments are stored in the DB in UTC and converted to local time
        /// through the <see cref="LocalizationService"/> before being wrapped in
        /// child view models.
        /// </summary>
        public AppointmentsViewModel()
        {
            var repo = new AppointmentRepository();
            var custRepo = new CustomerRepository();
            

            List<Appointment> utcAppointments = repo.GetAppointmentsByUserId(App.CurrentUser.UserId) ?? new List<Appointment>();

            UserAppointments = new ObservableCollection<Appointment>();

            foreach (var app in utcAppointments)
            {
                app.Start = new LocalizationService().ConvertUtcToLocal(app.Start);
                app.End = new LocalizationService().ConvertUtcToLocal(app.End);
                app.CreateDate = new LocalizationService().ConvertUtcToLocal(app.CreateDate);
                app.LastUpdate = new LocalizationService().ConvertUtcToLocal(app.LastUpdate);
                app.CustomerName = custRepo.GetCustomerNameById(app.CustomerId);
                UserAppointments.Add(app);
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

