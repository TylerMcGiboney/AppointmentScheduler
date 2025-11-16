using System.ComponentModel;

namespace AppointmentScheduler.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private string _welcomeMessage = "Welcome to the Dashboard";

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set
            {
                _welcomeMessage = value;
                OnPropertyChanged(nameof(WelcomeMessage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
