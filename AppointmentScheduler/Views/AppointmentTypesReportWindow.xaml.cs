using System.Collections.Generic;
using System.Windows;
using AppointmentScheduler.Services;

namespace AppointmentScheduler.Views
{
    public partial class AppointmentTypesReportWindow : Window
    {
        public AppointmentTypesReportWindow(IEnumerable<AppointmentTypeReport> items)
        {
            InitializeComponent();
            ReportDataGrid.ItemsSource = items;
        }
    }
}
