using System.Collections.Generic;
using System.Windows;
using AppointmentScheduler.Services;

namespace AppointmentScheduler.Views
{
    public partial class CustomerAppointmentsReportWindow : Window
    {
        public CustomerAppointmentsReportWindow(IEnumerable<CustomerAppointmentReport> items)
        {
            InitializeComponent();
            ReportDataGrid.ItemsSource = items;
        }
    }
}
