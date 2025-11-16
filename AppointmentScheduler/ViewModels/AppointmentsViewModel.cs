using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AppointmentScheduler.ViewModels
{
    public class AppointmentsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Appointments { get; set; }

        public AppointmentsViewModel()
        {
            Appointments = new ObservableCollection<string>
            {
                "Meeting with John",
                "Design Review",
                "Project Demo"
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
