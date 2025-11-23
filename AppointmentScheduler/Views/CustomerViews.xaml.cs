using System;
using System.Windows;
using System.Windows.Controls;
using AppointmentScheduler.Models;
using AppointmentScheduler.Repositories;
using AppointmentScheduler.Services;
using AppointmentScheduler.ViewModels;

namespace AppointmentScheduler.Views
{
    /// <summary>
    /// Code-behind for Customers View
    /// </summary>
    public partial class CustomerViews : UserControl
    {
        // Initialize and set DataContext to CustomersViewModel.
        public CustomerViews()
        {
            InitializeComponent();
            DataContext = new CustomersViewModel();
        }

        // Creates new Customer object and opens CustomerEditWindow in Add mode.
        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            Customer newCustomer = new Customer();
            newCustomer.CustomerName = string.Empty;
            newCustomer.AddressId = 0;
            newCustomer.IsActive = true;
            newCustomer.CreateDate = DateTime.UtcNow;
            newCustomer.CreatedBy = App.CurrentUser.UserName;
            newCustomer.LastUpdate = DateTime.UtcNow;
            newCustomer.LastUpdateBy = App.CurrentUser.UserName;

            // Initialize address fields with empty strings to avoid null issues
            newCustomer.PhoneNumber = string.Empty;
            newCustomer.Address = string.Empty;
            newCustomer.Address2 = string.Empty;
            newCustomer.City = string.Empty;
            newCustomer.PostalCode = string.Empty;
            newCustomer.Country = string.Empty;

            CustomerEditorViewModel vm = new CustomerEditorViewModel(newCustomer, false);

            CustomerEditWindow window = new CustomerEditWindow(vm);
            window.Owner = Window.GetWindow(this);

            bool? result = window.ShowDialog();

            // If the user clicked Save, add the new customer to the list after converting the create and update to local time.
            if (result == true)
            {
                LocalizationService loc = new LocalizationService();
                newCustomer.CreateDate = loc.ConvertUtcToLocal(newCustomer.CreateDate);
                newCustomer.LastUpdate = loc.ConvertUtcToLocal(newCustomer.LastUpdate);

                CustomersViewModel listVm = DataContext as CustomersViewModel;
                if (listVm != null)
                {
                    listVm.CustomerList.Add(newCustomer);
                }
            }
        }

        // Opens the CustomerEditWindow in Edit mode for the selected customer.
        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            Customer selected = CustomerDataGrid.SelectedItem as Customer;
            if (selected == null)
            {
                MessageBox.Show("Please select a customer to edit.",
                                "No Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            // Create a copy so changes can be canceled without affecting original.
            Customer editableCopy = new Customer();
            editableCopy.CustomerId = selected.CustomerId;
            editableCopy.CustomerName = selected.CustomerName;
            editableCopy.AddressId = selected.AddressId;
            editableCopy.IsActive = selected.IsActive;
            editableCopy.CreateDate = selected.CreateDate;
            editableCopy.CreatedBy = selected.CreatedBy;
            editableCopy.LastUpdate = selected.LastUpdate;
            editableCopy.LastUpdateBy = selected.LastUpdateBy;

            // Copy all address fields
            editableCopy.PhoneNumber = selected.PhoneNumber ?? string.Empty;
            editableCopy.Address = selected.Address ?? string.Empty;
            editableCopy.Address2 = selected.Address2 ?? string.Empty;
            editableCopy.City = selected.City ?? string.Empty;
            editableCopy.PostalCode = selected.PostalCode ?? string.Empty;
            editableCopy.Country = selected.Country ?? string.Empty;

            CustomerEditorViewModel vm = new CustomerEditorViewModel(editableCopy, true);

            CustomerEditWindow window = new CustomerEditWindow(vm);
            window.Owner = Window.GetWindow(this);

            bool? result = window.ShowDialog();

            // If the user clicked Save, update the selected customer in the list after converting the create and update to local time.
            if (result == true)
            {
                // Update the local time for display
                LocalizationService loc = new LocalizationService();
                editableCopy.CreateDate = loc.ConvertUtcToLocal(editableCopy.CreateDate);
                editableCopy.LastUpdate = loc.ConvertUtcToLocal(editableCopy.LastUpdate);

                CustomersViewModel listVm = DataContext as CustomersViewModel;
                if (listVm != null)
                {
                    int index = listVm.CustomerList.IndexOf(selected);
                    if (index >= 0)
                    {
                        listVm.CustomerList[index] = editableCopy;
                    }
                }
            }
        }

        // Deletes the selected customer after confirming and checking for existing appointments.
        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            var selected = CustomerDataGrid.SelectedItem as Customer;
            if (selected == null)
            {
                MessageBox.Show("Please select a customer to delete.",
                                "No Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                string.Format("Are you sure you want to delete customer '{0}' (ID {1})?",
                              selected.CustomerName, selected.CustomerId),
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            var apptRepo = new AppointmentRepository();
            var apptsForCustomer = apptRepo.GetAppointmentsByCustomerId(selected.CustomerId);

            if (apptsForCustomer != null && apptsForCustomer.Count > 0)
            {
                MessageBox.Show(
                    string.Format("This customer has {0} appointment(s) in the system and cannot be deleted. Please delete those appointments first.",
                                  apptsForCustomer.Count),
                    "Delete Customer",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var custRepo = new CustomerRepository();

            try
            {
                bool deleted = custRepo.DeleteCustomerById(selected.CustomerId);

                if (deleted)
                {
                    var listVm = DataContext as CustomersViewModel;
                    if (listVm != null)
                        listVm.CustomerList.Remove(selected);

                    MessageBox.Show("Customer deleted successfully.",
                                    "Deletion Successful",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to delete the customer. Please try again.",
                                    "Deletion Failed",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            catch (ApplicationException ex)
            {
                // Fall-back: if some other FK hits, still show a friendly message
                MessageBox.Show(ex.Message,
                                "Delete Customer",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
        }
    }
}