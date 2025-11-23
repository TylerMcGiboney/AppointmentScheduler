using System.Windows;
using AppointmentScheduler.ViewModels;

namespace AppointmentScheduler.Views
{
    public partial class CustomerEditWindow : Window
    {
        public CustomerEditWindow()
        {
            InitializeComponent();
        }

        public CustomerEditWindow(CustomerEditorViewModel vm)
            : this()
        {
            DataContext = vm;
        }
    }
}
