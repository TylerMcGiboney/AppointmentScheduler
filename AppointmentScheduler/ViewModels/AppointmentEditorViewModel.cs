using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using AppointmentScheduler.Commands;
using AppointmentScheduler.Models;
using AppointmentScheduler.Repositories;
using AppointmentScheduler.Services;
using AppointmentScheduler.Validators;

namespace AppointmentScheduler.ViewModels
{
    /// <summary>
    /// ViewModel for adding/editing an appointment.
    /// </summary>
    public class AppointmentEditorViewModel : INotifyPropertyChanged
    {
        //The appointment being edited (or a new one if adding)
        private readonly Appointment _originalAppointment;

        //Used for converting bertween local and UTC times
        private readonly LocalizationService _localizationService = new LocalizationService();


        private readonly CustomerRepository _custRepo = new CustomerRepository();
        private readonly AppointmentRepository _apptRepo = new AppointmentRepository();

        // List of customers to populate ComboBox
        public ObservableCollection<Customer> Customers { get; }

        // ---- Selected Customer ----
        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get => _selectedCustomer;
            set { _selectedCustomer = value; OnPropertyChanged(); }
        }

        // ---- Appointment fields ----
        private string _title;
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private string _location;
        public string Location
        {
            get => _location;
            set { _location = value; OnPropertyChanged(); }
        }

        private string _contact;
        public string Contact
        {
            get => _contact;
            set { _contact = value; OnPropertyChanged(); }
        }

        private string _type;
        public string Type
        {
            get => _type;
            set { _type = value; OnPropertyChanged(); }
        }

        private string _url;
        public string Url
        {
            get => _url;
            set { _url = value; OnPropertyChanged(); }
        }

        // ---- Time pickers: Start ----

        private DateTime? _startDate;
        /// <summary>
        /// Local date of appointment start (no time component).
        /// Setting this will regenerate HourOptions12.
        /// </summary>
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged();
                    if (value.HasValue)
                    {
                        UpdateTimeOptionsForDate(value.Value);
                    }
                }
            }
        }

        //Hour options for 12-hour time picker
        private ObservableCollection<int> _hourOptions12;
        public ObservableCollection<int> HourOptions12
        {
            get => _hourOptions12;
            set { _hourOptions12 = value; OnPropertyChanged(); }
        }

        // Minute options for time picker (0, 5, 10, ..., 55)
        public ObservableCollection<int> MinuteOptions { get; }
        public ObservableCollection<string> AmPmOptions { get; }

        // Selected start time components
        // Selected hour
        private int _startHour12;
        public int StartHour12
        {
            get => _startHour12;
            set { _startHour12 = value; OnPropertyChanged(); }
        }
        // Selected minute
        private int _startMinute;
        public int StartMinute
        {
            get => _startMinute;
            set { _startMinute = value; OnPropertyChanged(); }
        }
        // Selected AM/PM
        private string _startAmPm;
        public string StartAmPm
        {
            get => _startAmPm;
            set { _startAmPm = value; OnPropertyChanged(); }
        }

        // ---- Time pickers: End ----

        private DateTime? _endDate;
        /// <summary>
        /// Local date of appointment end.
        /// Must match StartDate for a valid appointment.
        /// </summary>
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged();
                    if (value.HasValue)
                    {
                        UpdateTimeOptionsForDate(value.Value);
                    }
                }
            }
        }

        // Selected end time components
        // Selected hour
        private int _endHour12;
        public int EndHour12
        {
            get => _endHour12;
            set { _endHour12 = value; OnPropertyChanged(); }
        }

        // Selected minute
        private int _endMinute;
        public int EndMinute
        {
            get => _endMinute;
            set { _endMinute = value; OnPropertyChanged(); }
        }

        // Selected AM/PM
        private string _endAmPm;
        public string EndAmPm
        {
            get => _endAmPm;
            set { _endAmPm = value; OnPropertyChanged(); }
        }

        // ---- UI text ----

        // True if editing an existing appointment; false if adding a new one
        public bool IsEditMode { get; }

        // Window title, header, and button text
        public string WindowTitle => IsEditMode ? "Edit Appointment" : "Add Appointment";
        public string HeaderText => WindowTitle;
        public string PrimaryButtonText => IsEditMode ? "Save Changes" : "Add Appointment";

        // ---- Commands ----
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        //Raise property changed events
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// IMPORTANT: appointment.Start / End need to be local when passed in.
        /// </summary>
        public AppointmentEditorViewModel(Appointment appointment, bool isEditMode)
        {
            _originalAppointment = appointment ?? new Appointment();
            IsEditMode = isEditMode;

            // Load customers for selection
            Customers = new ObservableCollection<Customer>(_custRepo.GetCustomers());

            //Initialize time picker options
            HourOptions12 = new ObservableCollection<int>();
            MinuteOptions = new ObservableCollection<int>(new[] { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55 });
            AmPmOptions = new ObservableCollection<string>(new[] { "AM", "PM" });

            // Populate fields from the original appointment
            Title = _originalAppointment.Title;
            Description = _originalAppointment.Description;
            Location = _originalAppointment.Location;
            Contact = _originalAppointment.Contact;
            Type = _originalAppointment.Type;
            Url = _originalAppointment.Url;

            // Initalize selected customer based on Appointment.CustomerId
            if (Customers.Any())
            {
                SelectedCustomer =
                    Customers.FirstOrDefault(c => c.CustomerId == _originalAppointment.CustomerId)
                    ?? Customers.FirstOrDefault();
            }

            // If editing, populate date/time pickers from existing appointment
            if (IsEditMode && _originalAppointment.Start != default && _originalAppointment.End != default)
            {
                // Local times already
                var localStart = _originalAppointment.Start;
                var localEnd = _originalAppointment.End;

                StartDate = localStart.Date;
                EndDate = localEnd.Date;

                UpdateTimeOptionsForDate(StartDate.Value);

                SetStartFromLocalDateTime(localStart);
                SetEndFromLocalDateTime(localEnd);
            }
            else
            {
                // New appointment defaults
                var today = DateTime.Today;
                StartDate = today;
                EndDate = today;

                UpdateTimeOptionsForDate(today);

                // Default time window (9:00–9:30 AM)
                StartHour12 = 9;
                StartMinute = 0;
                StartAmPm = "AM";

                EndHour12 = 9;
                EndMinute = 30;
                EndAmPm = "AM";
            }

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        //// <summary>
        /// Builds the 12-hour options list for a given date.
        /// Business-hour enforcement is not done here; it's handled by the AppointmentValidator.
        /// </summary>
        private void UpdateTimeOptionsForDate(DateTime localDate)
        {
            if (HourOptions12 == null)
                HourOptions12 = new ObservableCollection<int>();

            HourOptions12.Clear();
            for (int h = 1; h <= 12; h++)
                HourOptions12.Add(h);

            OnPropertyChanged(nameof(HourOptions12));
        }

        /// <summary>
        /// Converts a 24-hour clock hour to (hour, AM/PM) in 12-hour format.
        /// </summary>
        private static void From24To12(int hour24, out int hour12, out string amPm)
        {
            amPm = hour24 >= 12 ? "PM" : "AM";
            hour12 = hour24 % 12;
            if (hour12 == 0) hour12 = 12;
        }

        /// <summary>
        /// Converts a 12-hour clock hour + AM/PM into a 24-hour clock hour.
        /// </summary>
        private static int To24(int hour12, string amPm)
        {
            int h = hour12 % 12;
            if (string.Equals(amPm, "PM", StringComparison.OrdinalIgnoreCase))
                h += 12;
            return h;
        }

        /// <summary>
        /// Sets the start time picker fields (hour, minute, AM/PM) based on a local DateTime.
        /// </summary>
        private void SetStartFromLocalDateTime(DateTime localStart)
        {
            From24To12(localStart.Hour, out int hour12, out string amPm);

            if (!HourOptions12.Contains(hour12))
                HourOptions12.Add(hour12);

            StartHour12 = hour12;
            StartAmPm = amPm;

            int minute = localStart.Minute;
            // Select to nearest available minute option if needed
            if (!MinuteOptions.Contains(minute))
            {
                minute = MinuteOptions.OrderBy(m => Math.Abs(m - minute)).First();
            }
            StartMinute = minute;
        }

        /// <summary>
        /// Sets the end time picker fields (hour, minute, AM/PM) based on a local DateTime.
        /// </summary>
        private void SetEndFromLocalDateTime(DateTime localEnd)
        {
            From24To12(localEnd.Hour, out int hour12, out string amPm);

            if (!HourOptions12.Contains(hour12))
                HourOptions12.Add(hour12);

            EndHour12 = hour12;
            EndAmPm = amPm;

            int minute = localEnd.Minute;
            if (!MinuteOptions.Contains(minute))
            {
                minute = MinuteOptions.OrderBy(m => Math.Abs(m - minute)).First();
            }
            EndMinute = minute;
        }

        
        /// <summary>
        /// Executes when the Save command is triggered.
        /// Performs basic required-field checks, then calls AppointmentValidator
        /// to enforce business rules, and finally saves the appointment if valid.
        /// </summary>
        private void Save(object obj)
        {
            // Basic required-field validation
            if (SelectedCustomer == null)
            {
                ShowError("Please select a customer.");
                return;
            }

            //
            if (string.IsNullOrWhiteSpace(Title))
            {
                ShowError("Please enter a title for the appointment.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Description))
            {
                ShowError("Please enter a description for the appointment.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Location))
            {
                ShowError("Please enter a location for the appointment.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Contact))
            {
                ShowError("Please enter a contact for the appointment.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Type))
            {
                ShowError("Please enter a type for the appointment.");
                return;
            }
            
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                ShowError("Please select both start and end dates.");
                return;
            }

            // build local DateTime from date + time pickers
            int startHour24 = To24(StartHour12, StartAmPm);
            int endHour24 = To24(EndHour12, EndAmPm);

            DateTime startLocal = new DateTime(
                StartDate.Value.Year,
                StartDate.Value.Month,
                StartDate.Value.Day,
                startHour24,
                StartMinute,
                0,
                DateTimeKind.Local);

            DateTime endLocal = new DateTime(
                EndDate.Value.Year,
                EndDate.Value.Month,
                EndDate.Value.Day,
                endHour24,
                EndMinute,
                0,
                DateTimeKind.Local);

            int userId = App.CurrentUser.UserId;
            int customerId = SelectedCustomer.CustomerId;
            int? apptId = IsEditMode ? _originalAppointment.AppointmentId : (int?)null;

            // Use the detailed validator to enforce all business rules and obtain
            // a user-friendly error message if validation fails.
            ValidationResult validation = AppointmentValidator.ValidateAppointmentWithDetails(
                userId,
                customerId,
                startLocal,
                endLocal,
                apptId);

            if (!validation.IsValid)
            {
                ShowError(validation.ErrorMessage ?? "The appointment is invalid.");
                return;
            }

            // Convert to UTC for DB storage
            _originalAppointment.Start = _localizationService.ConvertLocalToUtc(startLocal);
            _originalAppointment.End = _localizationService.ConvertLocalToUtc(endLocal);

            _originalAppointment.CustomerId = SelectedCustomer.CustomerId;
            _originalAppointment.CustomerName = SelectedCustomer.CustomerName;
            _originalAppointment.Title = Title;
            _originalAppointment.Description = Description;
            _originalAppointment.Location = Location;
            _originalAppointment.Contact = Contact;
            _originalAppointment.Type = Type;
            _originalAppointment.Url = Url;
            _originalAppointment.LastUpdate = DateTime.UtcNow;
            _originalAppointment.LastUpdateBy = App.CurrentUser.UserName;

            bool success;

            //Decide whether to add or edit based on IsEditMode
            if (IsEditMode)
            {
                success = _apptRepo.EditAppointment(_originalAppointment);
            }
            else
            {
                int newId = _apptRepo.AddAppointment(_originalAppointment);
                success = newId > 0;
                if (success)
                    _originalAppointment.AppointmentId = newId;
            }

            if (!success)
            {
                ShowError("Failed to save appointment. Please try again.");
                return;
            }

            // Close the window with a successful result
            CloseWindow(obj, true);
        }

        //Executes when the Cancel command is triggered and closes without saving.
        private void Cancel(object obj)
        {
            CloseWindow(obj, false);
        }

        // Shows a message box with the specified error message.
        private void ShowError(string message)
        {
            MessageBox.Show(message, "Invalid Appointment", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        // Closes the window passed as parameter with the specified dialog result.
        private void CloseWindow(object parameter, bool result)
        {
            if (parameter is Window window)
            {
                window.DialogResult = result;
                window.Close();
            }
        }
    }
}
