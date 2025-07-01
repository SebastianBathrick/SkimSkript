namespace SkimSkript.Logging
{
    using Segment = (string content, bool isProp);

    public abstract class Logger
    {
        #region Constants
        protected const LogLevel DEFAULT_LOG_LEVEL = LogLevel.Warning;
        private const bool DEFAULT_LOG_LABELS_ENABLED = true;
        private const char MESSAGE_INSERT_START = '{';
        private const char MESSAGE_INSERT_END = '}';

        #endregion

        #region Variables & Properties
        private LogLevel _logLevel = DEFAULT_LOG_LEVEL;
        private volatile bool _labelsEnabled = DEFAULT_LOG_LABELS_ENABLED;

        private object _logLock = new();
        private volatile bool _isLockEnabled = false;

        public bool IsLockEnabled => _isLockEnabled;
        public LogLevel LogLevel => _logLevel;
        #endregion

        #region Data Members Public Interface
        /// <summary>Returns true if the provided log level is enabled for output.</summary>
        public bool IsLevelEnabled(LogLevel level) => level <= _logLevel;

        /// <summary>Set the minimum log level that will be output.</summary>
        /// <param _name="level">The minimum level.</param>
        public Logger SetMinimumLogLevel(LogLevel level)
        {
            _logLevel = level;
            return this;
        }

        /// <summary>Enable or disable log level labels in output.</summary>
        /// <param _name="isEnabled">True to enable labels.</param>
        public Logger ToggleLogLabels(bool isEnabled = false)
        {
            _labelsEnabled = isEnabled;
            return this;
        }

        public Logger SetLockBehavior(bool isLockEnabled)
        {
            _isLockEnabled = true;
            return this;
        }

        public LockHandle AcquireLock() => new LockHandle(_logLock!);
        #endregion

        #region Logging Methods
        public abstract void Clear();   

        public void UserInterface(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.UserInterface)) return;
            ProcessMessage(message, properties);
        }

        /// <summary>Logs an information message.</summary>
        public void Info(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Information)) return;
            ProcessMessage(message, LogLevel.Information, properties);
        }

        /// <summary>Logs a warning message.</summary>
        public void Warning(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Warning)) return;
            ProcessMessage(message, LogLevel.Warning, properties);
        }

        /// <summary>Logs an error message.</summary>
        public void Error(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Error)) return;
            ProcessMessage(message, LogLevel.Error, properties);
        }

        /// <summary>Logs an error with an exception.</summary>
        public void Error(Exception ex, string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Error)) return;
            ProcessMessage($"{message} Exception thrown: {ex.Message}", LogLevel.Error, properties);
        }

        /// <summary>Logs a fatal message.</summary>
        public void Fatal(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Fatal)) return;
            ProcessMessage(message, LogLevel.Fatal, properties);
        }

        /// <summary>Logs a fatal message with an exception.</summary>
        public void Fatal(Exception ex, string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Fatal)) return;
            ProcessMessage($"{message} Exception thrown: {ex.Message}", LogLevel.Fatal, properties);
        }

        /// <summary>Logs a debug message.</summary>
        public void Debug(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Debug)) return;
            ProcessMessage(message, LogLevel.Debug, properties);
        }

        /// <summary>Logs a verbose message.</summary>
        public void Verbose(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Verbose)) return;
            ProcessMessage(message, LogLevel.Verbose, properties);
        }
        #endregion

        #region Log Processing
        /// <summary>Implement in derived classes to write out the message.</summary>
        protected abstract void Write(Segment[] segmentedMessage);

        /// <summary>Prepares and sends the log message to the implementation's Write method.</summary>
        private void ProcessMessage(string message, LogLevel level, params object[] properties) =>
            ProcessMessage(_labelsEnabled ? $"{GetLabel(level)} {message}" : message, properties);

        private void ProcessMessage(string message, params object[] properties)
        {
            var segmentedMessage = GetSegmentedMessage(message, properties);

            if(_isLockEnabled)
            {
                using var _ = AcquireLock();
                Write(segmentedMessage);
            }
            else
                Write(segmentedMessage);
        }

        /// <summary>Splits message into segments based on where property inserts are.</summary>
        protected static Segment[] GetSegmentedMessage(ReadOnlySpan<char> message, params object[] properties)
        {
            var segments = new List<Segment>();
            int propIndex = 0;
            int currentPos = 0;

            while (currentPos < message.Length)
            {
                int startPos = message.Slice(currentPos).IndexOf(MESSAGE_INSERT_START);
                if (startPos == -1)
                {
                    if (currentPos < message.Length)
                        segments.Add(new Segment(message.Slice(currentPos).ToString(), false));
                    break;
                }

                startPos += currentPos;

                if (startPos > currentPos)
                    segments.Add(new Segment(message.Slice(currentPos, startPos - currentPos).ToString(), false));

                int endPos = message.Slice(startPos + 1).IndexOf(MESSAGE_INSERT_END);
                if (endPos == -1)
                    throw new FormatException("Unclosed placeholder");

                endPos += startPos + 1;

                if (propIndex < properties.Length)
                    segments.Add(new Segment(properties[propIndex]?.ToString() ?? "", true));

                propIndex++;
                currentPos = endPos + 1;
            }

            return segments.ToArray();
        }

        /// <summary>Returns the label for a log level, e.g., [Info]:</summary>
        private string GetLabel(LogLevel level) => $"[{level}]:";
        #endregion
    }

    public class LockHandle : IDisposable
    {
        private readonly object _lockObj;

        public LockHandle(object lockObj)
        {
            _lockObj = lockObj;
            Monitor.Enter(_lockObj);
        }

        public void Dispose() =>
            Monitor.Exit(_lockObj);
    }

}


