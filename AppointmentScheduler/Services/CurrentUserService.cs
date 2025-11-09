using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Services
{
    public interface ICurrentUserService
    {
        string UserName { get; }
        void Set(string userName);
    }

    public sealed class CurrentUserService : ICurrentUserService
    {
        public string UserName { get; private set; } = "unknown";

        public void Set(string userName)
        {
            UserName = userName ?? "unknown";
        }
    }
}
