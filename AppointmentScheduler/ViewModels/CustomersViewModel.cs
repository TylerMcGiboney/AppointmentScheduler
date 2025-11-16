using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AppointmentScheduler.ViewModels
{
    public class CustomersViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Customers { get; set; }

        public CustomersViewModel()
        {
            Customers = new ObservableCollection<string>
            {
                "Acme Corp",
                "Contoso Ltd",
                "Northwind Traders"
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
