using System.Windows.Controls;
using AppointmentScheduler.ViewModels;

namespace AppointmentScheduler.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();
        }
    }
}
