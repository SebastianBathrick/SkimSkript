namespace JustLogger
{
    /// <summary> Represents the logging levels used in the application. </summary>
    public enum LogLevel
    {
        /// <summary>
        /// No log messages whatsoever.
        /// </summary>
        None = 0,

        /// <summary>
        /// Logs to the user interface (No log level label).
        /// </summary>
        UserInterface = 1,

        /// <summary>
        /// Logs a more severe error that will/could cause the application to crash.
        /// </summary>
        Fatal = 2,

        /// <summary>
        /// Logs an error that will/could cause the application to crash.
        /// </summary>
        Error = 3,

        /// <summary>
        /// Logs a warning that indicates a potential issue.
        /// </summary>
        Warning = 4,
        /// <summary>
        /// Logs a general message or non-critical status update.
        /// </summary>
        Information = 5,

        /// <summary>
        /// Infrequent logs with low-level debugging information.
        /// </summary>
        Debug = 6,

        /// <summary>
        /// Frequent logs showing very low-level debugging information.
        Verbose = 7,
    }
}
