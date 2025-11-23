using MySql.Data.MySqlClient;

namespace AppointmentScheduler.Services
{
    /// <summary>
    /// Provides standardized, user-friendly messages for MySQL exceptions.
    /// Converts low-level MySQL error codes into meaningful context-aware messages.
    /// </summary>
    public static class ExceptionHandler
    {
        /// <summary>
        /// Generates a friendly error message based on the MySqlException number.
        /// Includes a contextual phrase describing the operation being performed.
        /// </summary>
        /// <param name="ex">The MySqlException caught during the database operation.</param>
        /// <param name="context">
        /// A short description of what the application was attempting to do 
        /// (e.g., "adding address", "updating customer").
        /// </param>
        /// <returns>A formatted, human-readable error message.</returns>
        public static string GetMessage(MySqlException ex, string context)
        {
            string message;

            // Map well-known MySQL error codes to descriptive messages.
            switch (ex.Number)
            {
                case 1062:
                    message = "Duplicate entry error. One or more values must be unique.";
                    break;

                case 1451:
                    message = "This record cannot be deleted or updated because related records exist.";
                    break;

                case 1452:
                    message = "Foreign key constraint failed. The referenced record may not exist.";
                    break;

                case 1048:
                    message = $"A required field is missing (MySQL Error {ex.Number}). Details: {ex.Message}";
                    break;

                default:
                    message = $"A database error occurred (MySQL Error {ex.Number}). Details: {ex.Message}";
                    break;
            }

            // If context is provided, prepend it for clarity.
            if (!string.IsNullOrWhiteSpace(context))
            {
                message = $"{context}: {message}";
            }

            return message;
        }
    }
}
