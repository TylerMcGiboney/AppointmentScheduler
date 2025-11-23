using AppointmentScheduler.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AppointmentScheduler.ViewModels
{
    /// <summary>
    /// Represents one navigation item in the application's main sidebar.
    /// </summary>
    public class NavigationItem
    {
        public string Title { get; set; }
        public string Icon { get; set; }   // Segoe MDL2 glyph
        public object ViewModel { get; set; }
    }

    /// <summary>
    /// ViewModel for the main window, managing navigation and current user context.
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public User CurrentUser { get; }
        public ObservableCollection<NavigationItem> NavigationItems { get; }

        // Selected navigation item and corresponding ViewModel
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

        // Current ViewModel displayed in the main content area
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

        // Initialize the navigation system and defaults to dashboard
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
