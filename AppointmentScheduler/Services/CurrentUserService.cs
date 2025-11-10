using AppointmentScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AppointmentScheduler.Services
{
    public interface ICurrentUserService
    {
        User CurrentUser { get; }
        bool IsLoggedIn { get; }
        string UserName { get; }
        int UserId { get; }

        void Set(User user);
        void Clear();
       
    }

    public sealed class CurrentUserService : ICurrentUserService
    {
        public User CurrentUser { get; private set; }
        public bool IsLoggedIn => CurrentUser != null;
        public string UserName => CurrentUser?.UserName ?? "unknown";
        public int UserId => CurrentUser.UserId;

        public void Set(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            user.Password = null;

            CurrentUser = user;
        }

        public void Clear()
        {
            CurrentUser = null;
        }

        
    }
}
