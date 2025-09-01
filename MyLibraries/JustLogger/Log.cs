namespace JustLogger
{
    public static class Log
    {
        private static Logger? _instance;

        public static Logger Logger =>
            _instance ?? throw new InvalidOperationException("Static Logger instance is not set");

        public static bool IsLoggerSet => _instance != null;

        /// <summary>
        /// Sets the logger to use.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <returns>The logger that was set.</returns>
        public static Logger SetLogger(Logger logger) => _instance = logger;

        /// <summary>
        /// Sets the lock behavior of the logger.
        /// </summary>
        /// <param name="isLockBehavior">Whether to lock the logger.</param>
        /// <returns>The static instance of the logger.</returns>
        public static Logger SetLockBehavior(bool isLockBehavior) =>
            Logger.SetLockBehavior(isLockBehavior);

        /// <summary>
        /// Logs a message to the user interface (ignores log level).
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="properties">The properties to log.</param>
        public static void Interface(string message, params object[] properties) =>
            Logger.Interface(message, properties);

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="properties">The properties to log.</param>
        public static void Info(string message, params object[] properties) =>
            Logger.Info(message, properties);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="properties">The properties to log.</param>
        public static void Warning(string message, params object[] properties) =>
            Logger.Warning(message, properties);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="properties">The properties to log.</param>
        public static void Error(string message, params object[] properties) =>
            Logger.Error(message, properties);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="properties">The properties to log.</param>
        public static void Debug(string message, params object[] properties) =>
            Logger.Debug(message, properties);

        /// <summary>
        /// Logs a verbose message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="properties">The properties to log.</param>
        public static void Verbose(string message, params object[] properties) =>
            Logger.Verbose(message, properties);

        /// <summary>
        /// Logs an error message with an exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="properties">The properties to log.</param>
        public static void Error(Exception ex, string message, params object[] properties) =>
            Logger.Error(ex, message, properties);

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="properties">The properties to log.</param>
        public static void Fatal(string message, params object[] properties) =>
            Logger.Fatal(message, properties);

        /// <summary>
        /// Logs a fatal message with an exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="message">The message to log.</param>
        ///
        public static void Fatal(Exception ex, string message, params object[] properties) =>
            Logger.Fatal(ex, message, properties);
    }
}
