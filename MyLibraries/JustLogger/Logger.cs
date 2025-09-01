namespace JustLogger
{
    using Segment = (string content, bool isProp);

    /// <summary>
    /// Abstract class for logging messages.
    /// </summary>
    public abstract class Logger
    {
        #region Constants
        protected const LogLevel DEFAULT_LOG_LEVEL = LogLevel.Warning;
        private const bool DEFAULT_LOG_LABELS_ENABLED = true;
        private const char MESSAGE_INSERT_START = '{';
        private const char MESSAGE_INSERT_END = '}';

        #endregion

        #region Variables & Properties
        /// <summary>
        /// The minimum log level that will be output.
        /// </summary>
        private LogLevel _logLevel = DEFAULT_LOG_LEVEL;

        /// <summary>
        /// Whether log level labels are enabled.
        /// </summary>
        private volatile bool _labelsEnabled = DEFAULT_LOG_LABELS_ENABLED;

        /// <summary>
        /// The lock object for the logger (locking is disabled by default).
        /// </summary>
        private object _logLock = new();

        /// <summary>
        /// Whether the logger is locked.
        /// </summary>
        private volatile bool _isLockEnabled = false;

        /// <summary>
        /// Whether the logger is locked.
        /// </summary>
        public bool IsLockEnabled => _isLockEnabled;

        /// <summary>
        /// The minimum log level that will be output.
        /// </summary>
        public LogLevel LogLevel => _logLevel;
        #endregion

        #region Data Members Public Interface
        /// <summary>
        /// Set the minimum log level that will be output.
        /// </summary>
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

        /// <summary>
        /// Enable or disable locking of the logger.
        /// </summary>
        /// <param name="isLockEnabled">True to enable locking.</param>
        public Logger SetLockBehavior(bool isLockEnabled)
        {
            _isLockEnabled = true;
            return this;
        }

        /// <summary>
        /// Acquires a lock on the logger.
        /// </summary>
        /// <returns>A lock handle.</returns>
        public LockHandle AcquireLock() => new LockHandle(_logLock!);
        #endregion

        #region Logging Methods
        public abstract void Clear();

        /// <summary>
        /// Logs a message without a label (shown unless log level is set to <see cref="LogLevel.None"/>)
        /// </summary>
        public void Interface(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.UserInterface)) return;
            ProcessMessage(message, properties);
        }
        /// <summary>
        /// Logs an information message.
        /// </summary>
        public void Info(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Information)) return;
            ProcessMessage(message, LogLevel.Information, properties);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public void Warning(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Warning)) return;
            ProcessMessage(message, LogLevel.Warning, properties);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        public void Error(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Error)) return;
            ProcessMessage(message, LogLevel.Error, properties);
        }

        /// <summary>
        /// Logs an error with an exception.
        /// </summary>
        public void Error(Exception ex, string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Error)) return;
            ProcessMessage($"{message} Exception thrown: {ex.Message}", LogLevel.Error, properties);
        }

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        public void Fatal(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Fatal)) return;
            ProcessMessage(message, LogLevel.Fatal, properties);
        }

        /// <summary>
        /// Logs a fatal message with an exception.
        /// </summary>
        public void Fatal(Exception ex, string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Fatal)) return;
            ProcessMessage($"{message} Exception thrown: {ex.Message}", LogLevel.Fatal, properties);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        public void Debug(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Debug)) return;
            ProcessMessage(message, LogLevel.Debug, properties);
        }

        /// <summary>
        /// Logs a verbose message.
        /// </summary>
        public void Verbose(string message, params object[] properties)
        {
            if (!IsLevelEnabled(LogLevel.Verbose)) return;
            ProcessMessage(message, LogLevel.Verbose, properties);
        }

        private bool IsLevelEnabled(LogLevel level) => level <= _logLevel;
        #endregion

        #region Log Processing
        /// <summary>
        /// Implement in derived classes to write out the message.
        /// </summary>
        protected abstract void Write(Segment[] segmentedMessage);

        /// <summary>
        /// Prepares and sends the log message to the implementation's Write method.
        /// </summary>
        private void ProcessMessage(string message, LogLevel level, params object[] properties) =>
            ProcessMessage(_labelsEnabled ? $"{GetLabel(level)} {message}" : message, properties);

        private void ProcessMessage(string message, params object[] properties)
        {
            var segmentedMessage = GetSegmentedMessage(message, properties);

            if (_isLockEnabled)
            {
                using var _ = AcquireLock();
                Write(segmentedMessage);
            }
            else
                Write(segmentedMessage);
        }

        /// <summary>
        /// Splits message into segments based and replaces inserts with property string representations.
        /// </summary>
        protected static Segment[] GetSegmentedMessage(ReadOnlySpan<char> message, params object[] properties)
        {
            var segments = new List<Segment>();

            // Index of current property to be inserted as a segment
            int propIndex = 0;
            int currentPos = 0;

            while (currentPos < message.Length)
            {
                /* Slice off the end of the message & find the first character of a property 
                 * insert (ex. { of {MyProperty}}) */
                int startPos = message.Slice(currentPos).IndexOf(MESSAGE_INSERT_START);

                // If no property was in the slice then stop evaluating message
                if (startPos == -1)
                {
                    if (currentPos < message.Length)
                        segments.Add(new Segment(message.Slice(currentPos).ToString(), false));
                    break;
                }

                // Make startPos relative to the original message
                startPos += currentPos;

                // If current position exceeds the startPos, the segment is ready so add it to the list
                if (startPos > currentPos)
                    segments.Add(new Segment(message.Slice(currentPos, startPos - currentPos).ToString(), false));

                // Find the end of the property insert (ex. } of {MyProperty}})
                int endPos = message.Slice(startPos + 1).IndexOf(MESSAGE_INSERT_END);

                if (endPos == -1)
                    throw new FormatException("Unclosed placeholder");

                // Insert the property string representation as a segment
                if (propIndex < properties.Length)
                    segments.Add(new Segment(properties[propIndex]?.ToString() ?? "", true));

                // Move up the property, endPos, and currentPos for the next iteration
                propIndex++;
                endPos += startPos + 1;
                currentPos = endPos + 1;
            }

            return segments.ToArray();
        }

        /// <summary>
        /// Returns the label for a log level, e.g., [Info]:
        /// </summary>
        private string GetLabel(LogLevel level) => $"[{level}]:";
        #endregion
    }

    /// <summary>
    /// Handles the locking of the logger.
    /// </summary>
    public class LockHandle : IDisposable
    {
        private readonly object _lockObj;

        /// <summary>
        /// Initializes a new instance of the <see cref="LockHandle"/> class.
        /// </summary>
        /// <param name="lockObj">The object to lock on.</param>
        public LockHandle(object lockObj)
        {
            _lockObj = lockObj;
            Monitor.Enter(_lockObj);
        }

        /// <summary>
        /// Releases the lock on the object.
        /// </summary>
        public void Dispose() =>
            Monitor.Exit(_lockObj);
    }

}


