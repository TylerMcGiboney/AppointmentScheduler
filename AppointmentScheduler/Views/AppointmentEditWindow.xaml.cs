using System.Windows;
using AppointmentScheduler.ViewModels;

namespace AppointmentScheduler.Views
{
    public partial class AppointmentEditWindow : Window
    {
        public AppointmentEditWindow()
        {
            InitializeComponent();
        }

        public AppointmentEditWindow(AppointmentEditorViewModel vm)
            : this()
        {
            DataContext = vm;
        }
    }
}
