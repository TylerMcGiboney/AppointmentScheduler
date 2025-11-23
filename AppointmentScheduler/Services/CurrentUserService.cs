using AppointmentScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppointmentScheduler.Services
{
    /// <summary>
    /// Service to manage the current logged-in user.
    /// </summary>
    public interface ICurrentUserService
    {
        // The currently logged-in user.
        User CurrentUser { get; }

        // Indicates if a user is currently logged in.
        bool IsLoggedIn { get; }

        // The username of the current user.
        string UserName { get; }

        // The user ID of the current user.
        int UserId { get; }

        // Sets the current user.
        void Set(User user);

        // Clears the current user.
        void Clear();
       
    }

    /// <summary>
    /// Implementation of ICurrentUserService to manage the current user.
    /// </summary>
    public sealed class CurrentUserService : ICurrentUserService
    {
        // The currently logged-in user.
        public User CurrentUser { get; private set; }

        // Indicates if a user is currently logged in.
        public bool IsLoggedIn => CurrentUser != null;

        // The username of the current user.
        public string UserName => CurrentUser?.UserName ?? "unknown";

        // The user ID of the current user.
        public int UserId => CurrentUser.UserId;

        // Sets the current user.
        public void Set(User user)
        {
            
            if (user == null)
                throw new ArgumentNullException(nameof(user)); // Ensure user is not null

            // Remove password for security reasons
            user.Password = null;

            // Set the current user
            CurrentUser = user;
        }

        public void Clear()
        {
            // Clear the current user
            CurrentUser = null;
        }

        
    }
}
