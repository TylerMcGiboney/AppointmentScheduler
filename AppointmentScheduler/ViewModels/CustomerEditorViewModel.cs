using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using AppointmentScheduler.Commands;
using AppointmentScheduler.Models;
using AppointmentScheduler.Repositories;

namespace AppointmentScheduler.ViewModels
{
    /// <summary>
    /// ViewModel for adding or editing a customer.
    /// </summary>
    public class CustomerEditorViewModel : INotifyPropertyChanged
    {
        private readonly Customer _originalCustomer;
        private readonly CustomerRepository _customerRepo = new CustomerRepository();
        private readonly CityRepository _cityRepo = new CityRepository();
        private readonly CountryRepository _countryRepo = new CountryRepository();
        private readonly AddressRepository _addressRepo = new AddressRepository();

        // ---- UI fields (flattened) ----
        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set { _customerName = value; OnPropertyChanged(); }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged(); }
        }

        // Backing field for Address2 to ensure it's never null
        private string _address2 = " ";
        public string Address2
        {
            get { return _address2 ?? " "; }
            set { _address2 = value ?? " "; OnPropertyChanged(); }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set { _city = value; OnPropertyChanged(); }
        }

        private string _postalCode;
        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; OnPropertyChanged(); }
        }

        private string _country;
        public string Country
        {
            get { return _country; }
            set { _country = value; OnPropertyChanged(); }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged(); }
        }

        // ---- UI text ----
        public bool IsEditMode { get; private set; }
        public string WindowTitle { get { return IsEditMode ? "Edit Customer" : "Add Customer"; } }
        public string HeaderText { get { return WindowTitle; } }
        public string PrimaryButtonText { get { return IsEditMode ? "Save Changes" : "Add Customer"; } }

        // ---- Commands ----
        public ICommand SaveCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// Constructs the ViewModel for editing or creating a customer.
        /// Populates UI fields from the given customer (if any),
        /// and sets defaults for a new customer.
        /// </summary>
        public CustomerEditorViewModel(Customer customer, bool isEditMode)
        {
            _originalCustomer = customer ?? new Customer();
            IsEditMode = isEditMode;

            // Initialize all fields, falling back to empty strings or safe defaults.
            CustomerName = _originalCustomer.CustomerName ?? string.Empty;
            PhoneNumber = _originalCustomer.PhoneNumber ?? string.Empty;
            Address = _originalCustomer.Address ?? string.Empty;
            Address2 = _originalCustomer.Address2 ?? " ";
            City = _originalCustomer.City ?? string.Empty;
            PostalCode = _originalCustomer.PostalCode ?? string.Empty;
            Country = _originalCustomer.Country ?? string.Empty;

            // Set IsActive - default to true for new customers
            IsActive = IsEditMode ? _originalCustomer.IsActive : true;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        /// <summary>
        /// Validates input fields, normalizes country/city/address,
        /// and then either updates an existing customer or creates a new one.
        /// Shows message boxes for any validation or persistence errors.
        /// </summary>
        private void Save(object parameter)
        {
            // ---- Validation ----
            if (string.IsNullOrWhiteSpace(CustomerName))
            {
                ShowError("Please enter a customer name.");
                return;
            }
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                ShowError("Please enter a phone number.");
                return;
            }

            if(!PhoneNumber.All(c => char.IsDigit(c) || c == '-'))
            {
                ShowError("Phone number can only contain numbers and dashes.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Address))
            {
                ShowError("Please enter an address.");
                return;
            }
            if (string.IsNullOrWhiteSpace(City))
            {
                ShowError("Please enter a city.");
                return;
            }
            if (string.IsNullOrWhiteSpace(PostalCode))
            {
                ShowError("Please enter a ZIP/postal code.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Country))
            {
                ShowError("Please enter a country.");
                return;
            }

            string trimmedCity = City.Trim();
            string trimmedCountry = Country.Trim();

            // ---- Country / City (normalized tables) ----

            // Ensure country exists (or create)
            int countryId = _countryRepo.GetOrCreateCountry(trimmedCountry);

            // Ensure city exists (or create) in that country
            int cityId = _cityRepo.GetOrCreateCity(trimmedCity, trimmedCountry);

            // ---- Address ----
            Address address = new Address();
            address.Address1 = Address.Trim();
            // Ensure Address2 is never null - always use empty string
            address.Address2 = string.IsNullOrWhiteSpace(Address2) ? " " : Address2.Trim();
            address.CityId = cityId;
            address.Zip = PostalCode.Trim();
            address.PhoneNumber = PhoneNumber.Trim();

            int addressId;

            if (IsEditMode && _originalCustomer.AddressId > 0)
            {
                // Update existing address
                address.AddressId = _originalCustomer.AddressId;
                address.LastUpdate = DateTime.UtcNow;
                address.LastUpdateBy = App.CurrentUser.UserName;

                bool addrUpdated = _addressRepo.EditAddress(address);
                if (!addrUpdated)
                {
                    ShowError("Failed to update address.");
                    return;
                }
                addressId = _originalCustomer.AddressId;
            }
            else
            {
                // Create new address
                address.CreateDate = DateTime.UtcNow;
                address.CreatedBy = App.CurrentUser.UserName;
                address.LastUpdate = DateTime.UtcNow;
                address.LastUpdateBy = App.CurrentUser.UserName;

                addressId = _addressRepo.AddAddress(address);
                if (addressId <= 0)
                {
                    ShowError("Failed to create address.");
                    return;
                }
            }

            // ---- Customer ----
            _originalCustomer.CustomerName = CustomerName.Trim();
            _originalCustomer.AddressId = addressId;
            _originalCustomer.IsActive = IsActive;
            _originalCustomer.PhoneNumber = PhoneNumber.Trim();
            _originalCustomer.Address = Address.Trim();
            // Ensure Address2 is never null in Customer object
            _originalCustomer.Address2 = string.IsNullOrWhiteSpace(Address2) ? " " : Address2.Trim();
            _originalCustomer.City = City.Trim();
            _originalCustomer.PostalCode = PostalCode.Trim();
            _originalCustomer.Country = Country.Trim();
            _originalCustomer.LastUpdate = DateTime.UtcNow;
            _originalCustomer.LastUpdateBy = App.CurrentUser.UserName;

            bool success;

            if (IsEditMode)
            {
                // Update existing customer
                success = _customerRepo.EditCustomer(_originalCustomer);
            }
            else
            {
                // Create new customer
                _originalCustomer.CreateDate = DateTime.UtcNow;
                _originalCustomer.CreatedBy = App.CurrentUser.UserName;

                int newId = _customerRepo.AddCustomer(_originalCustomer);
                success = newId > 0;
                if (success)
                {
                    _originalCustomer.CustomerId = newId;
                }
            }

            if (!success)
            {
                ShowError("Failed to save customer. Please try again.");
                return;
            }
            // Close the window on success
            CloseWindow(parameter, true);
        }

        private void Cancel(object parameter)
        {
            CloseWindow(parameter, false);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Invalid Customer", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void CloseWindow(object parameter, bool result)
        {
            Window window = parameter as Window;
            if (window != null)
            {
                window.DialogResult = result;
                window.Close();
            }
        }
    }
}