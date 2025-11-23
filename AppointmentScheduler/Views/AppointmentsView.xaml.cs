using System;
using System.Windows;
using System.Windows.Controls;
using AppointmentScheduler.Models;
using AppointmentScheduler.Services;
using AppointmentScheduler.Repositories;
using AppointmentScheduler.ViewModels;

namespace AppointmentScheduler.Views
{
    /// <summary>
    /// Interaction logic for AppointmentsView.xaml
    /// </summary>
    public partial class AppointmentsView : UserControl
    {
        // Initialize and set DataContext to AppointmentsViewModel.
        public AppointmentsView()
        {
            InitializeComponent();
            DataContext = new AppointmentsViewModel();
        }

        // Handle Add Appointment button click and make sure at least one customer exists before adding appointment.
        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            var customerRepo = new CustomerRepository();
            var customers = customerRepo.GetCustomers();

            if (customers == null || customers.Count == 0)
            {
                MessageBox.Show("You must create a customer before adding an appointment.",
                                "No Customers",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }

            // New appointment defaulted with UTC timestamps.
            var newAppointment = new Appointment
            {
                UserId = App.CurrentUser.UserId,
                CustomerId = customers[0].CustomerId,
                Title = string.Empty,
                Description = string.Empty,
                Location = string.Empty,
                Contact = string.Empty,
                Type = string.Empty,
                Url = string.Empty,
                CreateDate = DateTime.UtcNow,
                CreatedBy = App.CurrentUser.UserName,
                LastUpdate = DateTime.UtcNow,
                LastUpdateBy = App.CurrentUser.UserName
                // Start/End will be set by the editor and stored as UTC
            };

            // Create ViewModel and open editor/add window.
            var vm = new AppointmentEditorViewModel(newAppointment, isEditMode: false);

            var window = new AppointmentEditWindow(vm)
            {
                Owner = Window.GetWindow(this)
            };

            bool? result = window.ShowDialog();

            if (result == true)
            {
                // The VM has saved Start/End as UTC. Convert to local for display.
                var loc = new LocalizationService();

                newAppointment.Start = loc.ConvertUtcToLocal(newAppointment.Start);
                newAppointment.End = loc.ConvertUtcToLocal(newAppointment.End);
                newAppointment.CreateDate = loc.ConvertUtcToLocal(newAppointment.CreateDate);
                newAppointment.LastUpdate = loc.ConvertUtcToLocal(newAppointment.LastUpdate);

                // Add the new appointment to the list in the ViewModel.
                if (DataContext is AppointmentsViewModel listVm)
                {
                    listVm.UserAppointments.Add(newAppointment);
                }
            }
        }

        // Opens the selected appointment in the AppointmentEditWindow in edit mode.
        private void EditAppointment_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected appointment from the DataGrid.
            Appointment selected = AppointmentDataGrid.SelectedItem as Appointment;
            if (selected == null)
            {
                MessageBox.Show("Please select an appointment to edit.",
                                "No Selection",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                return;
            }


            // Make a copy so Cancel doesn't affect the grid row.
            var editableCopy = new Appointment
            {
                AppointmentId = selected.AppointmentId,
                CustomerId = selected.CustomerId,
                UserId = selected.UserId,
                Title = selected.Title,
                Description = selected.Description,
                Location = selected.Location,
                Contact = selected.Contact,
                Type = selected.Type,
                Url = selected.Url,
                Start = selected.Start,      // Local
                End = selected.End,        // Local
                CreateDate = selected.CreateDate, // Local
                CreatedBy = selected.CreatedBy,
                LastUpdate = selected.LastUpdate,
                LastUpdateBy = selected.LastUpdateBy
            };

            // Create ViewModel and open editor window in edit mode.
            var vm = new AppointmentEditorViewModel(editableCopy, isEditMode: true);

            var window = new AppointmentEditWindow(vm)
            {
                Owner = Window.GetWindow(this)
            };

            bool? result = window.ShowDialog();

            if (result == true)
            {
                // The VM has converted editableCopy.Start/End to UTC for DB.
                // Convert them back to local for display in the grid.
                var loc = new LocalizationService();

                editableCopy.Start = loc.ConvertUtcToLocal(editableCopy.Start);
                editableCopy.End = loc.ConvertUtcToLocal(editableCopy.End);
                editableCopy.CreateDate = loc.ConvertUtcToLocal(editableCopy.CreateDate);
                editableCopy.LastUpdate = loc.ConvertUtcToLocal(editableCopy.LastUpdate);

                if (DataContext is AppointmentsViewModel listVm)
                {
                    int index = listVm.UserAppointments.IndexOf(selected);
                    if (index >= 0)
                    {
                        listVm.UserAppointments[index] = editableCopy;
                    }
                }
            }
        }

        // Deletes the selected appointment after user confirmation.
        private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            Appointment selectedAppointment = AppointmentDataGrid.SelectedItem as Appointment;
            if (selectedAppointment == null)
                return;

            var repo = new AppointmentRepository();

            // Confirm deletion with the user.
            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete Appointment ID {selectedAppointment.AppointmentId} with '{selectedAppointment.CustomerName}'?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                bool deleteSuccess = repo.DeleteAppointmentById(selectedAppointment.AppointmentId);

                if (deleteSuccess)
                {
                    // Remove from the ViewModel's collection to update the UI.
                    if (DataContext is AppointmentsViewModel viewModel)
                    {
                        viewModel.UserAppointments.Remove(selectedAppointment);
                    }

                    MessageBox.Show(
                        $"Appointment ID {selectedAppointment.AppointmentId} deleted successfully.",
                        "Deletion Successful",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        "Failed to delete the appointment. Please try again.",
                        "Deletion Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(
                    "Deletion cancelled.",
                    "Cancelled",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}
