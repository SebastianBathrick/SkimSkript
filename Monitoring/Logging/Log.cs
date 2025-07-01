namespace SkimSkript.Logging
{
    public static class Log
    {
        private static Logger? _instance;

        public static bool IsLoggerSet => _instance != null;

        public static Logger SetLogger(Logger logger) => _instance = logger;

        public static Logger SetLockBehavior(bool isLockBehavior) => _instance!.SetLockBehavior(isLockBehavior);

        public static void UserInterface(string message, params object[] properties) =>
            _instance?.UserInterface(message, properties);

        public static void Info(string message, params object[] properties) =>
            _instance?.Info(message, properties);

        public static void Warning(string message, params object[] properties) =>
            _instance?.Warning(message, properties);

        public static void Error(string message, params object[] properties) =>
            _instance?.Error(message, properties);

        public static void Debug(string message, params object[] properties) =>
            _instance?.Debug(message, properties);

        public static void Verbose(string message, params object[] properties) =>
            _instance?.Verbose(message, properties);

        public static void Error(Exception ex, string message, params object[] properties) =>
            _instance?.Error(ex, message, properties);

        public static void Fatal(string message, params object[] properties) =>
            _instance?.Fatal(message, properties);

        public static void Fatal(Exception ex, string message, params object[] properties) =>
            _instance?.Fatal(ex, message, properties);
    }
}
