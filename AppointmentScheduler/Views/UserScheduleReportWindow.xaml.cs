using System.Collections.Generic;
using System.Windows;
using AppointmentScheduler.Services;

namespace AppointmentScheduler.Views
{
    public partial class UserScheduleReportWindow : Window
    {
        public UserScheduleReportWindow(IEnumerable<UserAppointmentReport> items)
        {
            InitializeComponent();
            ReportDataGrid.ItemsSource = items;
        }
    }
}
