using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentScheduler.Services
{
    public static class ExceptionHandler
    {
        public static string GetMessage(MySqlException ex, string context)
        {
            string message;

            if (ex.Number == 1062)
                message = $"Duplicate entry error occurred while {context}. Please ensure all unique fields are unique.";
            else if (ex.Number == 1451)
                message = $"Cannot delete or update because of related records while {context}. Please remove related records first.";
            else if (ex.Number == 1452)
                message = $"Foreign key constraint fails while {context}. Please ensure all referenced records exist.";
            else if (ex.Number == 1048)
                message = $"A required field is missing (Error {ex.Number}) while {context}: {ex.Message}";
            else
                message = $"A database error occurred (Error {ex.Number}) while {context}: {ex.Message}";

            if (!string.IsNullOrEmpty(context))
                message = context = ": " + message;

            return message;

        }
    }
}
