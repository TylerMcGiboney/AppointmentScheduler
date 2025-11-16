using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AppointmentScheduler.ViewModels
{
    public class NavigationItem
    {
        public string Title { get; set; }
        public string Icon { get; set; }   // Segoe MDL2 glyph
        public object ViewModel { get; set; }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<NavigationItem> NavigationItems { get; }

        private NavigationItem _selectedNavigationItem;
        public NavigationItem SelectedNavigationItem
        {
            get => _selectedNavigationItem;
            set
            {
                if (_selectedNavigationItem != value)
                {
                    _selectedNavigationItem = value;
                    OnPropertyChanged(nameof(SelectedNavigationItem));
                    CurrentViewModel = value?.ViewModel;
                }
            }
        }

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                if (_currentViewModel != value)
                {
                    _currentViewModel = value;
                    OnPropertyChanged(nameof(CurrentViewModel));
                }
            }
        }

        public MainWindowViewModel()
        {
            NavigationItems = new ObservableCollection<NavigationItem>
            {
                new NavigationItem
                {
                    Title = "Dashboard",
                    Icon = "\uE80F", // Home icon
                    ViewModel = new DashboardViewModel()
                },
                new NavigationItem
                {
                    Title = "Calendar",
                    Icon = "\uE787", // Calendar-ish
                    ViewModel = new CalendarViewModel()
                },
                new NavigationItem
                {
                    Title = "Appointments",
                    Icon = "\uE8A5", // Calendar-ish
                    ViewModel = new AppointmentsViewModel()
                },
                new NavigationItem
                {
                    Title = "Customers",
                    Icon = "\uE716", // Contact
                    ViewModel = new CustomersViewModel()
                }
            };

            SelectedNavigationItem = NavigationItems.FirstOrDefault();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
